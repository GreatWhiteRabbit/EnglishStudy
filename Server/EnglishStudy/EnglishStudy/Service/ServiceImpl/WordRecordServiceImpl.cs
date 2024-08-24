using EnglishStudy.DTO;
using EnglishStudy.Entity;
using EnglishStudy.Entity.ChildEntity;
using EnglishStudy.Utils;
using System.Text.Json;

namespace EnglishStudy.Service.ServiceImpl {
    public class WordRecordServiceImpl : IWordRecordService {

        private MyDbContext dbContext;

        private RedisHelper redisHelper = new RedisHelper();

        public WordRecordServiceImpl(MyDbContext dbContext) {
            this.dbContext = dbContext;
        }

        public int AddRecord(int UserId, int Type,int LastId, WordRecordDetail wordRecordDetail) {
            WordRecord wordRecord = new WordRecord();
            wordRecord.UserId = UserId;
            wordRecord.WordType = Type;
            wordRecord.Time = DateTime.Now;
            wordRecord.Total = wordRecordDetail.List.Count;
            // 将wordRecordDetail转换为json
            string json = MySerializeObject(wordRecordDetail);
            // 将json存储到detail中
            wordRecord.Detail = json;
            // 将wordRecord保存到MySQL中
            dbContext.WordRecords.Add(wordRecord);
            // 将数据存储到wordPostion中
            // 先从MySQL中查找有没有记录
            var result = dbContext.WordPositions
                .Where(item => item.UserId == UserId
                && item.Type == Type && item.DeleteSign == 0)
                .SingleOrDefault();
            if(result == null) {
                // 如果记录为空，说明还没有开始记忆单词，将新的记录添加到
                // MySQL中
                WordPosition wordPosition = new WordPosition();
                wordPosition.Type = Type;
                wordPosition.LastWordId = LastId;
                wordPosition.UserId = UserId;
                dbContext.WordPositions.Add(wordPosition);
            }
            // result不为空，说明已经有记录了，那么修改lastId的值
            else {
                result.LastWordId = LastId;
            }
            // 执行保存操作
            dbContext.SaveChanges();
            // 返回id，如果保存成功了那么id不为0
            return wordRecord.RecordId;
        }

        public List<int> GetTotalList(int UserId, int Type, int size = 10) {
           
            // 根据recordId降序获取近size次total
            var list = dbContext.WordRecords
                .Where(item => item.UserId == UserId
                && item.Delete_sign == 0 && item.WordType == Type)
                .OrderByDescending(item => item.RecordId)
                .Take(size)
                .Select(item => item.Total)
                .ToList();
            // 将结果返回
            return list;
        }

        public List<WordDTO>  GetLastRecord(int UserId,int type) {
            // 按照降序获取最近一次记录
            var result = dbContext.WordRecords
                .Where(item => item.UserId == UserId
                && item.WordType == type && item.Delete_sign == 0)
                .OrderByDescending(item => item.RecordId)
                .Select(item => item.Detail)
                .Take(1)
                .ToList();
            // 创建返回值对象，只返回单词、单词音标、单词中文意思就行了
            List<WordDTO> wordDTOs = new List<WordDTO>();
            // 如果没有获取到数据，直接返回
            if(result.Count() == 0) {
                return wordDTOs;
            }
            // 获取第一条也是唯一一条数据
            string json = result[0];
            // 将json转换为WordRecordDetail
            WordRecordDetail detail = MyDeserialize<WordRecordDetail>(json);
            // 根据单词在hash中查找单词的音标和中文意思
            foreach(var item in detail.List) {
               wordDTOs.Add(redisHelper.getHash<WordDTO>(MyConstant.Word_Hash, item));
            }
            // 返回wordDTOs
            return wordDTOs;
        }

        public bool UpdateLastId(int UserId,int type) {
            // 先根据userId和type查找数据
            var result = dbContext.WordPositions
                .Where(item => item.UserId == UserId
                && item.Type == type && item.DeleteSign == 0)
                .SingleOrDefault();
            // 如果没有获取到结果，直接返回false
            if (result == null) return false;
            // 修改result中的lastId的值
            result.LastWordId = 0;
            int count = dbContext.SaveChanges();
            return (count == 1);
        }

        public List<WordRecordDTO> GetWordList(int UserId, int type, int size = 20) {
            // 这里不想用到Redis了，主要是想不到好的redis结构
            int lastId = 0;
            // 先从position中查找lastId
            var result = dbContext.WordPositions
                .Where(item => item.UserId == UserId
                && item.Type == type && item.DeleteSign == 0)
                .Select(item => new { item.LastWordId })
                .SingleOrDefault();
            // 如果获取到了值，那么将lastId赋值
            if(result != null) lastId = result.LastWordId;
            // 根据lastId从word表中获取size个单词
            var words = dbContext.Words
                .Where(item => item.Type == type
                && item.WordId > lastId)
                .Take(size)
                .OrderBy(item => item.WordId)
                .Select(item => new {
                    item.WordId,
                    item.Paraphrase,
                    item.Phonetic,
                    item.Words
                }).ToList();
            // 将获取到的数据封装到返回对象中
            List<WordRecordDTO> wordRecordDTOs = new List<WordRecordDTO>();
            foreach (var word in words) {
                WordRecordDTO wordRecordDTO = new WordRecordDTO();
                wordRecordDTO.Words = word.Words;
                wordRecordDTO.Paraphrase = word.Paraphrase;
                wordRecordDTO.Phonetic = word.Phonetic;
                wordRecordDTO.Id = word.WordId;
                wordRecordDTOs.Add(wordRecordDTO);
            }
            // 将封装对象返回
            return wordRecordDTOs;
        }

        public WordPage getAllWord(int UserId, int type,int page,int size) {
            // 先根据用户id获取单词记忆的进度
            var position = dbContext.WordPositions
                .Where(item => item.UserId == UserId && item.Type == type)
                .Select(item => item.LastWordId)
                .SingleOrDefault();
            // 获取单词
            var result = dbContext.Words
                .Where(item => item.Type == type && item.WordId <= position)
                .Skip((page - 1) * size)
                .Take(size)
                .Select(item => new {
                    item.Words,
                    item.Paraphrase,
                    item.Phonetic
                })
                .ToList();
            WordPage wordPage = new WordPage();
            // 装载total
            if(type ==1) {
                wordPage.total = position;
            }
            else if(type == 2) {
                int cet4 = redisHelper.getStringObject<int>(MyConstant.CET_4_Word_Num);
                wordPage.total = position - cet4;
            }
            else if(type == 3) {
                int cet4 = redisHelper.getStringObject<int>(MyConstant.CET_4_Word_Num);
                int cet6 = redisHelper.getStringObject<int>(MyConstant.CET_6_Word_Num);
                wordPage.total = position - cet4-cet6;
            }
            for(int i = 0; i < result.Count; i++) {
                WordDTO dto = new WordDTO( result[i].Words, result[i].Paraphrase,result[i].Phonetic);
                wordPage.wordDTOs.Add(dto);
            }
            return wordPage;
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
