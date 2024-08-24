using EnglishStudy.DTO;
using EnglishStudy.Entity;
using EnglishStudy.Utils;
using NPOI.Util;
using System.Security.Cryptography.X509Certificates;

namespace EnglishStudy.Service.ServiceImpl {
    public class EveryDayEnglishRecordImpl : IEveryDayEnglishRecord {

        private MyDbContext dbContext;

        private RedisHelper redisHelper = new RedisHelper();

        public EveryDayEnglishRecordImpl(MyDbContext dbContext) {
            this.dbContext = dbContext;
        }

        public bool Add(int UserId, int EverydayId) {
            // 获取添加前的阅读总数
            int preCount = GetTotal(UserId);
            EveryDayEnglishRecord record = new EveryDayEnglishRecord();
            record.UserId = UserId;
            record.EverydayId = EverydayId;
            record.Time = DateTime.Now;
            dbContext.EveryDayEnglishRecords.Add(record);
            int count = dbContext.SaveChanges();
            if (count == 0) return false;
            // 修改阅读总数
            string key = MyConstant.UserEveryDayEnglishRecordTotal + "-" + UserId;
            int expireTime = 24 * 60 * 60; // 过期时间设置为1天
            redisHelper.setString(key, preCount + 1, expireTime);
            return true;
        }

        public int Delete(int UserId, List<int> IdList) {
            int preCount = GetTotal(UserId);
            var sqlResult = dbContext.EveryDayEnglishRecords
                .Where(item => IdList.Contains(item.RecordId))
                .ToList();
            // 将删除标记位设置为1
            for(int i = 0; i < sqlResult.Count; i++) {
                sqlResult[i].DeleteSign = 1;
            }
            int count = dbContext.SaveChanges();
            // 删除sortset中的数据
            if(count != 0) {
                redisHelper.setString(MyConstant.UserEveryDayEnglishRecordTotal + "-" + UserId, preCount - count);
                foreach(int item in IdList) {
                    // 根据score删除sortset中的数据，因为memberi是json数据
                    // 所以不能根据id删除，但是在存储数据时id和score是相等的
                    redisHelper.ZremoveByScore(MyConstant.UserEveryDayEnglishRecordSortSet + "-" + UserId, item,item);
                }  
            }
            return count;
        }

        // 代码写的有点乱
        // 不存在EveryDayEnglishImpl中根据页面获取数据存在的bug
        public EveryDayEnglishRecordDTOPage GetByPageSize(int UserId, int page = 1, int size = 10) {
            EveryDayEnglishRecordDTOPage recordDTOPage = new EveryDayEnglishRecordDTOPage();
            // 获取阅读总数
            int total = GetTotal(UserId);
            // total为0直接返回数据
            if (total == 0)
                return recordDTOPage;
            recordDTOPage.Total = total;
            // 从Redis中的sortset中获取数据
            string key = MyConstant.UserEveryDayEnglishRecordSortSet + "-" + UserId;
            int expireTime = 24 * 60 * 60; // 设置过期时间为1天
            var result = redisHelper.Zrange<EveryDayEnglishRecordDTO>(key, (page - 1) * size, page * size - 1);
            // 从Redis中能够查询到结果
            int count = result.Count;

            if (count > 0) {
                // 如果已经获取了所有的数据直接返回
                if (count == size) {
                    recordDTOPage.RecordList = result;
                    return recordDTOPage;
                }
                // count < size 
                // 将剩余的size-count条数据添加到进来，可能数据库中已经不存在
                // 剩余的size-count条数据了，为了防止在做delete操作时进行了
                // 删除sortset中的操作导致分页获取数据不足情况
                // 详见https://learn.microsoft.com/zh-cn/ef/core/querying/pagination偏移分页
                recordDTOPage.RecordList.AddRange(result);
                int lastId = result[count - 1].RecordId;
                /*先从record表中取出符合的数据创建一个新表
               然后将新表和每日英语表关联查询*/
                var sqlResult = (from record in (
                                from newRecord in dbContext.EveryDayEnglishRecords
                                where newRecord.DeleteSign == 0 && newRecord.RecordId > lastId
                                && newRecord.UserId == UserId
                                select new {
                                    newRecord.RecordId,
                                    newRecord.EverydayId,
                                    newRecord.Time
                                })
                                 join everyday in dbContext.EveryDayEnglishes
                                 on record.EverydayId equals everyday.Everyday_id
                                 
                                 select new {
                                     record.RecordId,
                                     record.EverydayId,
                                     record.Time,
                                     everyday.Title,
                                     everyday.TitleTranslations
                                 })
                                .Take(size - count)
                                .ToList();
                // 数据已经全部获取完了
                if (sqlResult.Count() == 0) {
                    return recordDTOPage;
                }
                // 将获取到的数据添加到Redis和返回类中
                List<EveryDayEnglishRecordDTO> list = new List<EveryDayEnglishRecordDTO>();
                List<int> score = new List<int>();
                foreach (var item in sqlResult) {
                    EveryDayEnglishRecordDTO everyDayEnglishRecordDTO = new EveryDayEnglishRecordDTO();
                    everyDayEnglishRecordDTO.RecordId = item.RecordId;
                    everyDayEnglishRecordDTO.EverydayId = item.EverydayId;
                    everyDayEnglishRecordDTO.Title = item.Title;
                    everyDayEnglishRecordDTO.TitleTranslations = item.TitleTranslations;
                    everyDayEnglishRecordDTO.Time = item.Time;
                    list.Add(everyDayEnglishRecordDTO);
                    score.Add(item.RecordId);
                }
                // 将list添加到sortset中
                redisHelper.Zadd(key, list, score);
                redisHelper.setExpire(key, expireTime);
                // 将数据添加到返回类
                recordDTOPage.RecordList.AddRange(list);
                return recordDTOPage;

            }
            // 如果Redis中没有数据，从MySQL中获取
            var newResult = (from record in (
                                   from newRecord in dbContext.EveryDayEnglishRecords
                                   where newRecord.DeleteSign == 0 && newRecord.UserId == UserId
                                   select new {
                                       newRecord.RecordId,
                                       newRecord.EverydayId,
                                       newRecord.Time
                                   })
                             join everyday in dbContext.EveryDayEnglishes
                             on record.EverydayId equals everyday.Everyday_id
                             select new {
                                 record.RecordId,
                                 record.EverydayId,
                                 record.Time,
                                 everyday.Title,
                                 everyday.TitleTranslations
                             })
                             .Skip((page - 1) * size)
                             .Take(size)
                             .ToList();
            // 将获取到的结果转换
            List<EveryDayEnglishRecordDTO> everyDayEnglishRecordDTOs = new List<EveryDayEnglishRecordDTO>();
            List<int> scoreList = new List<int>();
            foreach (var item in newResult) {
                EveryDayEnglishRecordDTO recordDTO = new EveryDayEnglishRecordDTO();
                recordDTO.RecordId = item.RecordId;
                recordDTO.EverydayId = item.EverydayId;
                recordDTO.Time = item.Time;
                recordDTO.Title = item.Title;
                recordDTO.TitleTranslations = item.TitleTranslations;
                everyDayEnglishRecordDTOs.Add(recordDTO);
                scoreList.Add(item.RecordId);
            }
            recordDTOPage.RecordList.AddRange(everyDayEnglishRecordDTOs);
            // 将结果存储到Redis
            redisHelper.Zadd(key, everyDayEnglishRecordDTOs, scoreList);
            redisHelper.setExpire(key, expireTime);
            return recordDTOPage;

        }

        public int GetTotal(int UserId) {
            int count = 0;
            // 从Redis中查找数据
            string key = MyConstant.UserEveryDayEnglishRecordTotal + "-" + UserId;
            if (redisHelper.exist(key)) {
                count = redisHelper.getStringObject<int>(key);
                return count;
            }
            // 从MySQL中查找
            count = dbContext.EveryDayEnglishRecords
                .Where(item => item.UserId == UserId && item.DeleteSign == 0)
                .Count();
            // 将获取到的数据存储到Redis中并且设置过期时间
            // 过期时间设置为1天
            int expireTime = 24 * 60 * 60;
            redisHelper.setString(MyConstant.UserEveryDayEnglishRecordTotal + "-" + UserId, count, expireTime);
            return count;
        }

        // 这里没有修改Redis中的数据，不太想重新修改代码了
        public bool UpdateTime(int UserId,int RecordId) {
            var sqlResult = dbContext.EveryDayEnglishRecords
                .Where(item => item.RecordId == RecordId)
                .SingleOrDefault();
            if (sqlResult == null) return false;
            // 判断外键是否被删除了
            var result = dbContext.EveryDayEnglishes
                .Where(item => item.Everyday_id == sqlResult.EverydayId && item.Delete_Sign == 0)
                .SingleOrDefault();
            if(result == null) {
                // 说明外键已经被删除了，那么删除此数据
                List<int> list = new List<int> {
                    RecordId
                };
                Delete(UserId, list);
                return false;
            }
            sqlResult.Time = DateTime.Now;
            return (dbContext.SaveChanges() == 1);
        }

        public int getRecordIdByUserIdAndEverydayId(int UserId, int EverydayId) {
            var recordId = dbContext.EveryDayEnglishRecords
                .Where(item => item.UserId == UserId && item.EverydayId == EverydayId
                && item.DeleteSign == 0)
                .Select(item => item.RecordId)
                .SingleOrDefault();
            return recordId;
        }
    }
}
