using EnglishStudy.DTO;
using EnglishStudy.Entity;
using EnglishStudy.Entity.ChildEntity;
using EnglishStudy.Utils;
using NPOI.Util;
using System.Text.Json;

namespace EnglishStudy.Service.ServiceImpl {
    public class PassageRecordServiceImpl : IPassageRecordService {

        private MyDbContext dbContext;

        private RedisHelper redisHelper = new RedisHelper();

        public PassageRecordServiceImpl(MyDbContext dbContext) {
            this.dbContext = dbContext;
        }


        public bool AddRecord(int UserId, int PassageId, List<AnswerDetail> answerDetails) {
            bool success = false;
            // 如果答案集合为空，直接返回
            if (answerDetails.Count == 0) return success;
            // 获取已经答题的总数
            int total = GetTotal(UserId);
            PassageRecord passageRecord = new PassageRecord();
            passageRecord.UserId = UserId;
            passageRecord.PassageId = PassageId;
            passageRecord.Time = DateTime.Now;

            // 将answerDetailList转换为json
            AnswerDetailList answerDetailList = new AnswerDetailList();
            answerDetailList.List = answerDetails;
            string json = MySerializeObject(answerDetailList);
            passageRecord.Detail = json;
            // 将数据保存到数据库中
            dbContext.PassageRecords.Add(passageRecord);
            int count = dbContext.SaveChanges();
            if (count == 1) {
                success = true;
                // 修改Redis中的total
                string key = MyConstant.UserPassageRecordTotal + "-" + UserId;
                redisHelper.setString(key, total + 1, 24 * 60 * 60);
            }
            return true;
        }

        public int DeleteBatch(int UserId, List<int> RecordIdList) {
            int count = 0;
            if (RecordIdList.Count == 0) return count;
            // 获取做题总数
            int total = GetTotal(UserId);
            // 从MySQL中执行删除操作
            var result = dbContext.PassageRecords
                .Where(item => RecordIdList.Contains(item.RecordId))
                .ToList();
            if (result.Count == 0) return count;
            for (int i = 0; i < result.Count; i++) {
                // 逻辑删除
                result[i].DeleteSign = 1;
            }
            // 执行删除操作
            count = dbContext.SaveChanges();
            // 修改Redis中的数据
            if (count != 0) {
                // 删除sortset中的数据
                foreach (var item in RecordIdList) {
                    // 根据score删除数据，score即记录id
                    redisHelper.ZremoveByScore(MyConstant.UserPassageRecordSortSet + "-" + UserId, item, item);
                }
                // 修改total数据
                redisHelper.setString(MyConstant.UserPassageRecordTotal + "-" + UserId, total-total, 24 * 60 * 60);
            }
            return count;
        }

        // 通过阅读理解id查看用户是否完成了此阅读理解
        public PassageRecordDTO GetRecordByPassageId(int UserId, int PassageId) {
            PassageRecordDTO passageRecordDTO = new PassageRecordDTO();
            // 先从MySQL中根据userId和passageId查找recordId
            // 这条EF语句非常耗时，我不懂
            var result = dbContext.PassageRecords
                .Where(item => item.UserId == UserId
                && item.PassageId == PassageId
                && item.DeleteSign == 0)
                .SingleOrDefault();
           
            // 如果用户还未做passageId代表的阅读理解，返回空值
            if (result == null) {
                return passageRecordDTO;
            }
            // 否则从Redis中sortset中查找记录
            string key = MyConstant.UserPassageRecordSortSet + "-" + UserId;
            // 从Redis中获取一条数据
            var sortResult = redisHelper.ZrangeByScore<PassageRecordDTO>(key, result.RecordId, result.RecordId);
            if(sortResult.Count > 0) {
                // 返回第一条数据，也最多只有一条数据
                return sortResult[0];
            }
            // 从MySQL中查询数据
            var title = dbContext.Passages
                .Where(item => item.PassageId == PassageId && item.DeleteSign == 0)
                .Select(item => new { item.Title })
                .SingleOrDefault();
            // MySQL中不存在passageId的数据
            if(title == null) {
                return passageRecordDTO;
            }
            passageRecordDTO.Time = result.Time;
            passageRecordDTO.RecordId = result.RecordId;
            passageRecordDTO.List = MyDeserialize<AnswerDetailList>(result.Detail);
            passageRecordDTO.PassageTitle = title.Title;
            passageRecordDTO.PassageId = result.PassageId;
            passageRecordDTO.Accuracy = result.Accuracy;
            // 将数据存储到Redis中
            redisHelper.Zadd(key, passageRecordDTO, passageRecordDTO.RecordId);
            return passageRecordDTO;
        }

        public PassageRecordDTOPage GetRecordByPageSize(int UserId, int page, int size) {
            int total = GetTotal(UserId);
            PassageRecordDTOPage recordPage = new PassageRecordDTOPage();
            // 如果没有数据直接返回
            if (total == 0) {
                return recordPage;
            }
            recordPage.Total = total;
            string key = MyConstant.UserPassageRecordSortSet + "-" + UserId;
            int expireTime = 24 * 60 * 60; // 设置1天的过期时间
            // 从Redis中获取数据
            var redisResult = redisHelper.Zrange<PassageRecordDTO>(key, (page - 1) * size, page * size - 1);
            // 如果从Redis中取到了值
            if (redisResult.Count > 0) {
                // 判断是否还有剩余的数据没有被加载到Redis中
                recordPage.List = redisResult;
                if (redisResult.Count == size) {
                    return recordPage;
                }
                 /*从运行结果来看，不加如下判断会执行无效的SQL代码
                 从而导致程序效率下降*/
                if(total == (page -1)*size + redisResult.Count) {
                    return recordPage;
                }
                // 还差size-redisResult.Count条数据从MySQL中获取
                int lastId = redisResult[redisResult.Count - 1].RecordId;
                // 先从record表中获取record记录组成一个新表，然后用新表和passage表
                // 进行join操作获取passageTitle
               
                var leftResult = (from newRecord in (
                 (from record in dbContext.PassageRecords
                  where record.RecordId > lastId && record.DeleteSign == 0 && record.UserId == UserId
                  select new {
                      record.RecordId,
                      record.PassageId,
                      record.Time,
                      record.Detail,
                      record.Accuracy
                  }).Take(size - redisResult.Count)
                 
                 )
                                  join passage in dbContext.Passages
                                  on newRecord.PassageId equals passage.PassageId
                                  select new {
                                      newRecord.RecordId,
                                      newRecord.PassageId,
                                      newRecord.Time,
                                      newRecord.Detail,
                                      passage.Title,
                                      newRecord.Accuracy
                                  }).ToList();
              
                // 将获取到的leftResult装填到返回类和Redis中
                if (leftResult.Count > 0) {
                    List<PassageRecordDTO> myList = new List<PassageRecordDTO>();
                    List<int> scores = new List<int>();
                    foreach (var result in leftResult) {
                        PassageRecordDTO recordDTO = new PassageRecordDTO();
                        recordDTO.RecordId = result.RecordId;
                        recordDTO.PassageId = result.PassageId;
                        recordDTO.Time = result.Time;
                        recordDTO.PassageTitle = result.Title;
                        recordDTO.Accuracy = result.Accuracy;
                        recordDTO.List = MyDeserialize<AnswerDetailList>(result.Detail);
                        myList.Add(recordDTO);
                        scores.Add(result.RecordId);
                    }
                    // 装填到返回类
                    recordPage.List.AddRange(myList);
                    // 装填到Redis中
                    redisHelper.Zadd(key, myList, scores);
                    redisHelper.setExpire(key, expireTime);
                }
                return recordPage;
            }
            // 从MySQL中获取值
            var sqlResult = (from newRecord in (
                from record in dbContext.PassageRecords
                where record.DeleteSign == 0 && record.UserId == UserId
                select new {
                    record.RecordId,
                    record.PassageId,
                    record.Time,
                    record.Detail,
                    record.Accuracy
                }
                )
                             join passage in dbContext.Passages
                             on newRecord.PassageId equals passage.PassageId
                             select new {
                                 newRecord.RecordId,
                                 newRecord.PassageId,
                                 newRecord.Time,
                                 newRecord.Detail,
                                 passage.Title,
                                 newRecord.Accuracy
                             })
                .Skip((page - 1) * size)
                .Take(size)
                .ToList();
            // 将获取到的leftResult装填到返回类和Redis中
            List<PassageRecordDTO> list = new List<PassageRecordDTO>();
            List<int> scoreList = new List<int>();
            foreach (var result in sqlResult) {
                PassageRecordDTO recordDTO = new PassageRecordDTO();
                recordDTO.RecordId = result.RecordId;
                recordDTO.PassageId = result.PassageId;
                recordDTO.Time = result.Time;
                recordDTO.PassageTitle = result.Title;
                recordDTO.Accuracy = result.Accuracy;
                recordDTO.List = MyDeserialize<AnswerDetailList>(result.Detail);
                list.Add(recordDTO);
                scoreList.Add(result.RecordId);
            }
            recordPage.List = list;
            redisHelper.Zadd(key, list, scoreList);
            redisHelper.setExpire(key, expireTime);
            return recordPage;
        }

        public int GetTotal(int UserId) {
            int total = 0;
            // 从Redis中查找
            string key = MyConstant.UserPassageRecordTotal + "-" + UserId;
            int expireTime = 24 * 60 * 60; // 过期时间为1天
            // Redis中存在从Redis中查找
            if (redisHelper.exist(key)) {
                total = redisHelper.getStringObject<int>(key);
                return total;
            }
            // 从MySQL中查找
            total = dbContext.PassageRecords
                .Where(item => item.UserId == UserId && item.DeleteSign == 0)
                .Count();
            // 将查找到的数据存储到Redis中
            redisHelper.setString(key, total, expireTime);
            return total;
        }

        public List<Double> GetAccuracyList(int UserId, int size) {
            List<Double> list = new List<Double>();
            var result = dbContext.PassageRecords
                .Where(item => item.UserId == UserId && item.DeleteSign == 0)
                .OrderByDescending(item => item.RecordId)
                .Take(size)
                .Select(item => item.Accuracy)
                .ToList();
            if (result.Count != 0) return result;
            return list;
        }

      

        public bool UpdateAccuracy(int UserId, int PassageId, double Accuracy) {
            var result = dbContext.PassageRecords
                .Where(item => item.UserId == UserId && item.PassageId == PassageId && item.DeleteSign == 0)
                .SingleOrDefault();
            if (result == null) return false;
            result.Accuracy = Accuracy;
            return (dbContext.SaveChanges() == 1);

        }

        /// <summary>
        /// 序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Object"></param>
        /// <returns></returns>
        private string MySerializeObject<T>(T Object) {
            string json = "";
            try {
                json = JsonSerializer.Serialize(Object);
                return json;
            }
            catch (Exception e) {
                Console.WriteLine(e.Message);
            }
            return json;
        }

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        private T MyDeserialize<T>(string json) {
            try {
                return JsonSerializer.Deserialize<T>(json);
            }
            catch (Exception e) {
                Console.WriteLine(e.Message);
            }

            return default(T);
        }
    }
}
