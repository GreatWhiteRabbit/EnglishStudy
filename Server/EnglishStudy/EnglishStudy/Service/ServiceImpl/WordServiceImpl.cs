using EnglishStudy.Entity;
using EnglishStudy.Utils;
using System.Collections.Generic;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using EnglishStudy.DTO;
using System.Text.RegularExpressions;

namespace EnglishStudy.Service.ServiceImpl
{
    public class WordServiceImpl : IWordService {

        private readonly MyDbContext dbContext;

        private RedisHelper redisHelper = new RedisHelper();

        public WordServiceImpl(MyDbContext dbContext) {
            this.dbContext = dbContext;
        }

        public int AddWord(List<Word> wordList, int type = 1) {
            int count = 0;
            List<WordDTO> wordDTOs = new List<WordDTO>();
            foreach (Word word in wordList) {
                // 从Redis中的hash查看是否存在重复的单词
                string key = MyConstant.Word_Hash;
                string filed = word.Words;
                // 当前单词不存在
                if(!redisHelper.hashExist(key,filed)) {
                    word.Type = type;
                    // 将单词添加到数据库中
                    dbContext.Words.Add(word);
                    // 将单词添加到wordDOTs集合中，方便后续存储到Redis
                    wordDTOs.Add(new WordDTO { 
                        Words = word.Words,
                        Paraphrase = word.Paraphrase,
                        Phonetic = word.Phonetic
                    });
                }
            }
            count = dbContext.SaveChanges();
            // 将新添加的单词存放到Redis中
            try {
                foreach (WordDTO wordDTO in wordDTOs) {

                    // 将单词存放到hash中
                    redisHelper.setHash(MyConstant.Word_Hash, wordDTO.Words, wordDTO);
                    // 将单词存放到list中
                    string key = MyConstant.Word_List_Init + "_" + type + "_" + wordDTO.Words[0];
                    redisHelper.listLeftPush(key, wordDTO.Words);
                }
            } catch (Exception ex) {
                Console.WriteLine(ex.ToString());
            }
            // 修改单词的总数
            try {
                if(type == MyConstant.CET_4_Word_Type) {
                  int sum =  redisHelper.getStringObject<int>(MyConstant.CET_4_Word_Num);
                    sum = sum + count;
                    redisHelper.setString(MyConstant.CET_4_Word_Num, sum);
                }
                else if(type == MyConstant.CET_6_Word_Type) {
                    int sum = redisHelper.getStringObject<int>(MyConstant.CET_6_Word_Num);
                    sum = sum + count;
                    redisHelper.setString(MyConstant.CET_6_Word_Num, sum);
                }
                else {
                    int sum = redisHelper.getStringObject<int>(MyConstant.GRE_Word_Num);
                    sum = sum + count;
                    redisHelper.setString(MyConstant.GRE_Word_Num, sum);
                }
            } catch(Exception ex) {
                Console.WriteLine(ex.ToString());
            }
            return count;
        }

        public int UpdateWord(List<Word> wordList, int type = 1) {
            int count = 0;
            // 不太会批量更新，那么迭代更新吧
            foreach (Word word in wordList) {
               var result = dbContext.Words.Where(item => item.WordId == word.WordId)
                    .Single();
                if (result != null)
                {
                    // 只需要修改单词音标和注释就行了，单词本身不要修改
                    result.Paraphrase = word.Paraphrase;
                    result.Phonetic = word.Phonetic;
                    count = count + dbContext.SaveChanges();
                    // 更新redis中的hash，list不需要更新，因为单词本身没有变化 
                    try {
                        // 修改Redis中的hash值
                        WordDTO wordDTO = new WordDTO();
                        wordDTO.Words = result.Words;
                        wordDTO.Paraphrase = result.Paraphrase;
                        wordDTO.Phonetic = result.Phonetic;
                        redisHelper.setHash(MyConstant.Word_Hash, wordDTO.Words, wordDTO);
                    } catch(Exception ex) {
                        Console.WriteLine(ex.Message);
                    }
                }
            }
            return count;
        }

        public async Task<int> AddWord(IFormFile file, int type = 1) {
            int count = 0;
            var filepath = file.FileName;
            // 通过文件扩展名判断文件是否为excel
            string extension = Path.GetExtension(filepath);
            // 不是excel文件直接返回
            if (!extension.Equals(".xls") && !extension.Equals(".xlsx")) {
                Console.WriteLine(filepath);
                Console.WriteLine("不是excel文件");
                return count;
            }
            // 解析文件
            List<Word> wordList = new List<Word>();
            HashSet<string> setString = new HashSet<string>();
            IWorkbook wk = null;
            try {
                FileStream fs = await ConvertIFormFileToFileStream(file);
                wk = new XSSFWorkbook(fs);
                ISheet sheet = wk.GetSheetAt(0);
                IRow row = sheet.GetRow(0);
                for (int i = 1; i <= sheet.LastRowNum; i++) {
                    row = sheet.GetRow(i);
                    if (row != null) {
                        Word word = new Word();
                        word.Type = type;
                        for (int j = 0; j < row.LastCellNum; j++) {
                            string value = row.GetCell(j).ToString();
                            if (j == 0) {
                                word.Words = value;
                                if (!setString.Add(word.Words)) break;
                            }
                            else if (j == 1) {
                                word.Phonetic = value;
                            }
                            else if (j == 2) {
                                word.Paraphrase = value;
                                wordList.Add(word);
                            }

                        }

                    }
                }

            }
            catch (Exception e) {
                Console.WriteLine(e.Message);
            }
            // 添加数据
            count =  AddWord(wordList,type);
            return count;
        }

        public int DeleteWord(List<Word> wordList, int type = 1) {
            int count = 0;
            // 先删除数据库中的数据
            foreach(Word word in wordList) {
                var result = dbContext.Words
                    .Where(item => item.WordId == word.WordId)
                    .Single(); 
                if (result != null) {
                    dbContext.Words.Remove(result);
                    count = count + dbContext.SaveChanges();
                    try {
                        // 删除Redis中的hash的数据
                        redisHelper.hashDelete(MyConstant.Word_Hash, result.Words);
                        // 删除redis中list的数据
                        string key = MyConstant.Word_List_Init + "_" + type + "_" + result.Words[0];
                        if(redisHelper.exist(key)) {
                            redisHelper.deleteKey(key);
                        }
                    } catch(Exception ex) {
                        Console.WriteLine(ex.Message);
                    }
                }
            }
            // 修改单词的总数
            try {
                if (type == MyConstant.CET_4_Word_Type) {
                    int sum = redisHelper.getStringObject<int>(MyConstant.CET_4_Word_Num);
                    sum = sum - count;
                    redisHelper.setString(MyConstant.CET_4_Word_Num, sum);
                }
                else if (type == MyConstant.CET_6_Word_Type) {
                    int sum = redisHelper.getStringObject<int>(MyConstant.CET_6_Word_Num);
                    sum = sum - count;
                    redisHelper.setString(MyConstant.CET_6_Word_Num, sum);
                }
                else {
                    int sum = redisHelper.getStringObject<int>(MyConstant.GRE_Word_Num);
                    sum = sum - count;
                    redisHelper.setString(MyConstant.GRE_Word_Num, sum);
                }
            }
            catch (Exception ex) {
                Console.WriteLine(ex.ToString());
            }
            return count;
        }

        public List<Word> GetWordList(int type = 1, int page = 1, int size = 20) {
            var position = (page - 1) * size;
            // 获取数据
            var result = dbContext.Words
                .OrderBy(item => item.WordId)
                .Where(item => item.Type == type)
                .Skip(position)
                .Take(size)
                .ToList();
            return result;
        }

        public async Task<WordPage> InitWordList(int type = 1, string init = "a", int page = 1, int size = 20) {
            // 先从Redis中读取数据
            WordPage wordPage = new WordPage();
            List<WordDTO> wordDTOs = new List<WordDTO>();
            string key = MyConstant.Word_List_Init + "_" + type + "_" + init;
            long length = redisHelper.getListLength(key);
            if (length != 0) {
                int start = (page - 1) * size, end = -1;
                // 获取的元素超出范围
                if (start > length) return wordPage;
                if (length < page * size) end = (int)(length - 1);
                else end = page * size - 1;
               // 获取单词列表
                List<string> stringList = redisHelper.getListRange<string>(key, start, end);
                string hashKey = MyConstant.Word_Hash;
                // 根据单词从hash中获取具体内容
                wordPage.wordDTOs = redisHelper.getHashList<WordDTO>(hashKey, stringList);
                wordPage.total = (int)length;
                return wordPage;
            }
            else {
                // 从MySQL中读取数据
                var result = (from item in dbContext.Words
                             where item.Words.StartsWith(init)
                             && item.Type == type
                             select new {
                                 item.Paraphrase,
                                 item.Words,
                                 item.Phonetic
                             }).ToList();
                // 用于返回数据
               await Task.Run(() => {
                    int length = result.Count(),start = (page - 1)*size;
                    if (length >= page * size) length = page * size;
                    else length = -1;
                    for (int i = start; i < length; i++) {
                        WordDTO wordDTO = new WordDTO();
                        wordDTO.Paraphrase = result.ElementAt(i).Paraphrase;
                        wordDTO.Phonetic = result.ElementAt(i).Phonetic;
                        wordDTO.Words = result.ElementAt(i).Words;
                        wordPage.wordDTOs.Add(wordDTO);
                    }
                    wordPage.total = (int)result.Count();
                    
                });
                // 用于数据存储
               await Task.Run(() => {
                   List<string> stringList = new List<string>();
                   foreach (var item in result) {
                       stringList.Add(item.Words);
                    }
                    // 将MySQL中的数据存储到Redis中
                    try {
                        redisHelper.listLeftPushAll(key, stringList);
                    }
                    catch (Exception ex) {
                        Console.WriteLine(ex.ToString());
                    }
                   
               });
                return wordPage;
            }

        }

        public bool PushAllWordIntoHash() {
            // 先从MySQL中取出数据
            var result = dbContext.Words
                .Select(item => new { item.Phonetic, item.Words, item.Paraphrase })
                .ToList();

            // 把获取到的数据以hash的形式存储到Redis中
            // 把数据转成对象
            List<WordDTO> wordDTOs = new List<WordDTO>();
            List<string> filedList = new List<string>();
            foreach (var item in result) {
                wordDTOs.Add(new WordDTO(item.Words, item.Paraphrase,item.Phonetic));
                filedList.Add(item.Words);
            }
            // 将集合存储到Redis中
            string key = MyConstant.Word_Hash;
            // 暂时不设置过期时间
            bool success = redisHelper.setHash(key, filedList, wordDTOs);
            return success;
        }


        public List<WordDTO> SearchByKeywords(string keywords, int page = 1, int size = 20) {
            // 如果keywords是单词，那么执行SearchByWords
            if(Regex.IsMatch(keywords, @"^[A-Za-z]+$")) {
                return SearchByWords(keywords, page, size);
            }
            // 否则执行SearchByParaphrase方法
            return SearchByParaphrase(keywords, page, size);
        }

        public List<WordDTO> SearchByWords(string words,int page = 1,int size = 20) {
            List<WordDTO> wordDTOs = new List<WordDTO>();
            // 从MySQL中根据单词进行like匹配，并且根据单词去除重复的
            var result = dbContext.Words
                .Where(item => item.Words != null
                && item.Words.Contains(words))
                .Skip((page - 1) * size)
                .Take(size)
                .Select(item => new {
                    item.Paraphrase,
                    item.Phonetic,
                    item.Words
                })
                .ToList();
            // 根据单词去重
          var distinct =  result.DistinctBy(item => item.Words);
            // 将获取到的值添加到转换为WordDTO并添加到wordDTOs中
            foreach(var item in distinct) {
                WordDTO wordDTO = new WordDTO();
                wordDTO.Words = item.Words;
                wordDTO.Paraphrase = item.Paraphrase;
                wordDTO.Phonetic = item.Phonetic;
                wordDTOs.Add(wordDTO);
            }
            // 将获取到的数据返回
            return wordDTOs;
        }

        public List<Word> AdminSearchByKeywords(string keywords, int page = 1, int size = 20) {
            // 如果keywords是单词，那么执行SearchByWords
            if (Regex.IsMatch(keywords, @"^[A-Za-z]+$")) {
                // 从MySQL中根据单词进行like匹配，并且根据单词去除重复的
                var result = dbContext.Words
                    .Where(item => item.Words != null
                    && item.Words.Contains(keywords))
                    .Skip((page - 1) * size)
                    .Take(size)
                     .ToList();
                // 根据单词去重
                var distinct = result.DistinctBy(item => item.Words);
                return distinct.ToList();
            }
            else {
                // 从MySQL中根据中文意思进行like匹配，并且根据单词去除重复的
                var result = dbContext.Words
                    .Where(item => item.Words != null
                    && item.Paraphrase.Contains(keywords))
                    .Skip((page - 1) * size)
                    .Take(size)
                   
                    .ToList();
                // 根据单词去重
                var distinct = result.DistinctBy(item => item.Words);
                return distinct.ToList();
            }
        }

        public List<WordDTO> SearchByParaphrase(string paraphrase, int page = 1, int size = 20) {
            List<WordDTO> wordDTOs = new List<WordDTO>();
            // 从MySQL中根据中文意思进行like匹配，并且根据单词去除重复的
            var result = dbContext.Words
                .Where(item => item.Words != null
                && item.Paraphrase.Contains(paraphrase))
                .Skip((page - 1) * size)
                .Take(size)
                .Select(item => new {
                    item.Paraphrase,
                    item.Phonetic,
                    item.Words
                })
                .ToList();
            // 根据单词去重
            var distinct = result.DistinctBy(item => item.Words);
            // 将获取到的值添加到转换为WordDTO并添加到wordDTOs中
            foreach (var item in distinct) {
                WordDTO wordDTO = new WordDTO();
                wordDTO.Words = item.Words;
                wordDTO.Paraphrase = item.Paraphrase;
                wordDTO.Phonetic = item.Phonetic;
                wordDTOs.Add(wordDTO);
            }
            // 将获取到的数据返回
            return wordDTOs;
        }


        public int WordSum(int type = 1) {
            int count;
            // 先从Redis中获取数据
            count = redisHelper.getStringObject<int>(type.ToString());
            if(count == 0) {
                // 从数据库中获取数据
               count = dbContext.Words.Where(item => item.Type == type).Count();
                if(count != 0) {
                    if(type == 1) {
                        redisHelper.setString(MyConstant.CET_4_Word_Num, count);
                    }
                    else if(type == 2) {
                        redisHelper.setString(MyConstant.CET_6_Word_Num, count);
                    }
                  else if(type ==3) {
                        redisHelper.setString(MyConstant.GRE_Word_Num, count);
                    }
                }
            }
            return count;
        }
        
        /// <summary>
        /// 将IFormFile转成FileStream
        /// </summary>
        /// <param name="formFile"></param>
        /// <returns></returns>
        private async Task<FileStream> ConvertIFormFileToFileStream(IFormFile formFile) {
            if (formFile == null && formFile.Length <= 0) {
                return null;
            }
            else {
                var filePath = Path.GetTempFileName();//获取一个临时文件路径

                using (var stream = new FileStream(filePath, FileMode.Create))//创建一个局部FileStream变量
                {
                    await formFile.CopyToAsync(stream);//异步将IFormFile文件流放到刚刚创建的临时文件里面
                }
                return new FileStream(filePath, FileMode.Open);//将转换好的文件打开并返回
            }
        }

        
    }
}
