using EnglishStudy.DTO;
using EnglishStudy.Entity;
using EnglishStudy.Entity.ChildEntity;
using EnglishStudy.Utils;
using NPOI.HSSF.Record;
using System.Text.Json;

namespace EnglishStudy.Service.ServiceImpl {
    public class ListeningPaperRecordServiceImpl : IListeningPaperRecordService {

        private MyDbContext dbContext;

        private RedisHelper redisHelper = new RedisHelper();

        private static readonly int oneDayExpireTime = 24 * 60 * 60;// 一天的过期时间
        
        public ListeningPaperRecordServiceImpl(MyDbContext dbContext) {
            this.dbContext = dbContext;
        }

        public List<Double> GetAccuracyList(int UserId, int size = 10) {
            List<Double> list = new List<Double>();
            var result = dbContext.ListeningPaperRecords
                .Where(item => item.UserId == UserId && item.DeleteSign == 0)
                .OrderByDescending(item => item.RecordId)
                .Take(size)
                .Select(item => item.Accuracy)
                .ToList();
            if (result.Count != 0) return result;
            return list;
        }

        public int AddListeningPaperRecord(int UserId, int ListeningPaperId, double Accuracy, List<AnswerDetail> List) {
            // 先获取记录总数
            int total = GetTotal(UserId);
            // 创建实体对象，并赋值
            ListeningPaperRecord listeningPaperRecord = new ListeningPaperRecord();
            listeningPaperRecord.UserId = UserId;
            listeningPaperRecord.ListeningPaperId = ListeningPaperId;
            listeningPaperRecord.Accuracy = Accuracy;
            listeningPaperRecord.Time = DateTime.Now;
            // 将List转换成json字符串
            AnswerDetailList answerDetailList = new AnswerDetailList();
            answerDetailList.List = List;
            string json = MySerializeObject(answerDetailList);
            // 将json字符串添加到实体对象中
            listeningPaperRecord.Detail = json;
            // 将实体对象保存到MySQL中
            dbContext.ListeningPaperRecords.Add(listeningPaperRecord);
            int count = dbContext.SaveChanges();
            // 如果保存到MySQL中执行成功
            if(count == 1) {
                // 将total加1,并且将数据保存到Redis中并设置过期时间
                string key = MyConstant.UserListingPaperRecordTotal + "-" + UserId;
                redisHelper.setString(key, total + 1, oneDayExpireTime);
                return listeningPaperRecord.RecordId;
            }
            return 0;

        }

        public int DeleteBatch(int UserId, List<int> RecordIdList) {
            // 获取删除之前记录总数
            int total = GetTotal(UserId);
            // 根据id获取要被删除的数据
            var resultList = dbContext.ListeningPaperRecords
                .Where(item => item.UserId == UserId && RecordIdList.Contains(item.RecordId))
                .ToList();
            // 将resultList中的删除标记位设置为1
            for(int i = 0; i < resultList.Count; i++) {
                resultList[i].DeleteSign = 1;
            }
            int count = dbContext.SaveChanges();
            // 如果修改操作执行成功
            if(count != 0) {
                // 删除sortset中的数据
                string sortKey = MyConstant.UserListeningPaperRecordSortSet + "-" + UserId;
               foreach(int item in RecordIdList) {
                    redisHelper.ZremoveByScore(sortKey, item, item);
                }
                // 修改total的值
                string totalKey = MyConstant.UserListingPaperRecordTotal + "-" + UserId;
                redisHelper.setString(totalKey, total - count, oneDayExpireTime);
            }
            // 返回修改成功的个数
            return count;
        }

        public int ExistRecord(int UserId, int ListeningPaperId) {
            // 从MySQL中查找数据
            var recordId = dbContext.ListeningPaperRecords
                .Where(item => item.UserId == UserId
                && item.ListeningPaperId == ListeningPaperId
                && item.DeleteSign == 0)
                .Select(item => new { item.RecordId })
                .FirstOrDefault();
            // 没有查找到数据，返回0
            if (recordId == null) return 0;
            // 返回recordId
            return recordId.RecordId;
            
        }

        public ListeningPaperRecordDTOPage GetByPageSize(int UserId, int page, int size) {
            // 首先获取做题记录总数
            int total = GetTotal(UserId);
            ListeningPaperRecordDTOPage listeningPaperRecordDTOPage = new ListeningPaperRecordDTOPage();
            if(total == 0) {
                // 如果total为0，直接返回空值
                return listeningPaperRecordDTOPage;
            }
            // 将total赋值给对象
            listeningPaperRecordDTOPage.Total = total;
            string key = MyConstant.UserListeningPaperRecordSortSet + "-" + UserId;
            // 从Redis中查找数据
            var redisResult = redisHelper.Zrange<ListeningPaperRecordDTO>(key, (page - 1) * size, page * size - 1);
            if(redisResult.Count != 0) {
                // 如果获取到了数据
                listeningPaperRecordDTOPage.List = redisResult;
                // 如果获取到了size个数据，直接返回结果
                if(redisResult.Count == size) {
                    return listeningPaperRecordDTOPage;
                }
                /*从运行结果来看，不加如下判断会执行无效的SQL代码
                从而导致程序效率下降*/
                if (total == (page - 1) * size + redisResult.Count) {
                    return listeningPaperRecordDTOPage;
                }
                // 如果应该要获取的数据还有一部分没有被加载到Redis中，
                // 那么从MySQL中获取并加载到Redis中
                int lastId = redisResult[redisResult.Count - 1].ListeningPaperRecordId;
                var result = (from record in dbContext.ListeningPaperRecords
                              join paper in dbContext.ListeningPapers
                              on record.ListeningPaperId equals paper.ListeningPaperId
                              where record.UserId == UserId && record.RecordId > lastId
                              && record.DeleteSign == 0 && paper.DeleteSign == 0
                              select new {
                                  record.RecordId,
                                  record.Accuracy,
                                  record.Detail,
                                  record.ListeningPaperId,
                                  record.Time,
                                  paper.PaperTitle
                              }).OrderBy(x => x.RecordId)
                              .Take(size - redisResult.Count)
                              .ToList();
                // 将获取到的数据添加到Redis和返回类中
                if(result.Count > 0) {
                    List<ListeningPaperRecordDTO> lists = new List<ListeningPaperRecordDTO>();
                    List<int> scores = new List<int>();
                    foreach(var record in result) {
                        ListeningPaperRecordDTO recordDTO = new ListeningPaperRecordDTO();
                        recordDTO.ListeningPaperRecordId = record.RecordId;
                        recordDTO.Accuracy = record.Accuracy;
                        recordDTO.Time = record.Time;
                        recordDTO.PaperTitle = record.PaperTitle;
                        recordDTO.ListeningPaperId = record.ListeningPaperId;
                        recordDTO.List = MyDeserialize<AnswerDetailList>(record.Detail);
                        lists.Add(recordDTO);
                        scores.Add(recordDTO.ListeningPaperRecordId);
                    }
                    // 将lists存储Redis中
                    redisHelper.Zadd(key,lists,scores);
                    // 将lists添加到返回对象中
                    listeningPaperRecordDTOPage.List.AddRange(lists);
                    // 将数据返回
                    return listeningPaperRecordDTOPage;
                }

            }
            // 从MySQL中获取数据
            var sqlResult = (from record in dbContext.ListeningPaperRecords
                             join paper in dbContext.ListeningPapers
                             on record.ListeningPaperId equals paper.ListeningPaperId
                             where record.UserId == UserId && record.DeleteSign == 0
                             && paper.DeleteSign == 0
                             select new {
                                 record.RecordId,
                                 record.ListeningPaperId,
                                 record.Accuracy,
                                 record.Time,
                                 record.Detail,
                                 paper.PaperTitle
                             }
                             ).Skip((page - 1) * size)
                             .Take(size)
                             .OrderBy(item => item.RecordId)
                             .ToList();
            // 如果数据不存在
            if(sqlResult.Count == 0) {
                return listeningPaperRecordDTOPage;
            }
            List<ListeningPaperRecordDTO> list = new List<ListeningPaperRecordDTO>();
            List<int> score = new List<int>();
            foreach(var record in sqlResult) {
                ListeningPaperRecordDTO recordDTO = new ListeningPaperRecordDTO();
                recordDTO.ListeningPaperRecordId = record.RecordId;
                recordDTO.Accuracy = record.Accuracy;
                recordDTO.Time = record.Time;
                recordDTO.PaperTitle = record.PaperTitle;
                recordDTO.ListeningPaperId = record.ListeningPaperId;
                recordDTO.List = MyDeserialize<AnswerDetailList>(record.Detail);
                list.Add(recordDTO);
                score.Add(recordDTO.ListeningPaperRecordId);
            }
            // 将数据存储到Redis中
            redisHelper.Zadd(key, list, score);
            redisHelper.setExpire(key, oneDayExpireTime);
            // 将数据添加到返回对象中
            listeningPaperRecordDTOPage.List.AddRange(list);
            // 返回数据
            return listeningPaperRecordDTOPage;
        }

        public ListeningPaperRecordDTO GetByRecordId(int UserId, int RecordId) {
            // 从Redis中获取数据
            string key = MyConstant.UserListeningPaperRecordSortSet + "-" + UserId;

            var redisResult = redisHelper.ZrangeByScore<ListeningPaperRecordDTO>(key, RecordId, RecordId);
            if(redisResult.Count != 0) {
                // 如果在Redis中查找到了数据，将获取到的数据返回
                return redisResult[0];
            }
            // 如果Redis中没有数据，MySQL中查找数据
            var sqlResult = (from record in dbContext.ListeningPaperRecords
                             join paper in dbContext.ListeningPapers
                             on record.ListeningPaperId equals paper.ListeningPaperId
                             where record.UserId == UserId && record.RecordId == RecordId
                             && record.DeleteSign == 0 && paper.DeleteSign == 0
                             select new {
                                 record.RecordId,
                                 record.Accuracy,
                                 record.Detail,
                                 record.ListeningPaperId,
                                 record.Time,
                                 paper.PaperTitle
                             }
                             ).SingleOrDefault();
            // 如果没有查找到数据，返回null
            if(sqlResult == null) {
                return null;
            }
            ListeningPaperRecordDTO listeningPaperRecordDTO = new ListeningPaperRecordDTO();
            // 将获取到的数据赋值给listeningPaperRecordDTO
            listeningPaperRecordDTO.ListeningPaperRecordId = sqlResult.RecordId;
            listeningPaperRecordDTO.ListeningPaperId = sqlResult.ListeningPaperId;
            listeningPaperRecordDTO.PaperTitle = sqlResult.PaperTitle;
            listeningPaperRecordDTO.Accuracy = sqlResult.Accuracy;
            listeningPaperRecordDTO.Time = sqlResult.Time;
            
            // 将json数据转换为对象
            listeningPaperRecordDTO.List = MyDeserialize<AnswerDetailList>(sqlResult.Detail);
            // 将listeningPaperRecordDTO存储到sortset中
            redisHelper.Zadd(key, listeningPaperRecordDTO, RecordId);
            // 设置过期时间
            redisHelper.setExpire(key, oneDayExpireTime);
            // 将数据返回
            return listeningPaperRecordDTO;
        }

        public ListeningPaperRecordDTO getByUserIdAndPaperId(int UserId, int paperId) {
            // 首先根据userId和paperId获取recordId
            var recordId = dbContext.ListeningPaperRecords
                .Where(item => item.UserId == UserId && item.ListeningPaperId == paperId
                && item.DeleteSign == 0)
                .Select(item => item.RecordId)
                .SingleOrDefault();
            if(recordId == 0) {
                return null;
            }

            // 从Redis中获取数据
            string key = MyConstant.UserListeningPaperRecordSortSet + "-" + UserId;
            var redisResult = redisHelper.ZrangeByScore<ListeningPaperRecordDTO>(key,recordId,recordId);
            if (redisResult.Count != 0) {
                // 如果在Redis中查找到了数据，将获取到的数据返回
                return redisResult[0];
            }
            // 如果Redis中没有数据，MySQL中查找数据
            var sqlResult = (from record in dbContext.ListeningPaperRecords
                             join paper in dbContext.ListeningPapers
                             on record.ListeningPaperId equals paper.ListeningPaperId
                             where record.UserId == UserId && record.ListeningPaperId == paperId
                             && record.DeleteSign == 0 && paper.DeleteSign == 0
                             select new {
                                 record.RecordId,
                                 record.Accuracy,
                                 record.Detail,
                                 record.ListeningPaperId,
                                 record.Time,
                                 paper.PaperTitle
                             }
                             ).SingleOrDefault();
            // 如果没有查找到数据，返回null
            if (sqlResult == null) {
                return null;
            }
            ListeningPaperRecordDTO listeningPaperRecordDTO = new ListeningPaperRecordDTO();
            // 将获取到的数据赋值给listeningPaperRecordDTO
            listeningPaperRecordDTO.ListeningPaperRecordId = sqlResult.RecordId;
            listeningPaperRecordDTO.ListeningPaperId = sqlResult.ListeningPaperId;
            listeningPaperRecordDTO.PaperTitle = sqlResult.PaperTitle;
            listeningPaperRecordDTO.Accuracy = sqlResult.Accuracy;
            listeningPaperRecordDTO.Time = sqlResult.Time;

            // 将json数据转换为对象
            listeningPaperRecordDTO.List = MyDeserialize<AnswerDetailList>(sqlResult.Detail);
            // 将listeningPaperRecordDTO存储到sortset中
            redisHelper.Zadd(key, listeningPaperRecordDTO, sqlResult.RecordId);
            // 设置过期时间
            redisHelper.setExpire(key, oneDayExpireTime);
            // 将数据返回
            return listeningPaperRecordDTO;
        }

        public int GetTotal(int UserId) {
           
            int total = 0;
            string key = MyConstant.UserListingPaperRecordTotal + "-" + UserId;
            // 先判断Redis中是否存在key
            if(redisHelper.exist(key)) {
                // key存在，从Redis中获取数据
                total = redisHelper.getStringObject<int>(key);
                return total;
            }
            // key不存在，从MySQL中获取数据，并且将获取到的数据存储到Redis中
            // 从MySQL中查找数据
            total = dbContext.ListeningPaperRecords
                .Where(item => item.UserId == UserId && item.DeleteSign == 0)
                .Count();
            // 将total存储到Redis中
            int expireTime = 24 * 60 * 60; // 过期时间设置为1天，以秒为单位
            redisHelper.setString(key, total, expireTime);
            // 返回数据
            return total;
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
