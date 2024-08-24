using EnglishStudy.DTO;
using EnglishStudy.Entity;
using EnglishStudy.Utils;
using Microsoft.EntityFrameworkCore;
using NPOI.SS.Formula.Functions;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace EnglishStudy.Service.ServiceImpl {
    public class PassageServiceImpl : IPassageService {

        private MyDbContext dbContext;

        private RedisHelper redisHelper = new RedisHelper();

        public PassageServiceImpl(MyDbContext dbContext) {
            this.dbContext = dbContext;
        }



        public async Task<bool> AddFile(IFormFile file,string title) {
            var filePath = file.FileName;
            // 通过文件扩展名判断文件是否为excel
            string extension = Path.GetExtension(filePath);
            // 不是excel文件直接返回
            if (!extension.Equals(".xls") && !extension.Equals(".xlsx")) {
                Console.WriteLine(filePath);
                Console.WriteLine("不是excel文件");
                return false;
            }
            Passage passage = new Passage();
            IWorkbook wk = null;

            
            try {
                FileStream fs = await ConvertIFormFileToFileStream(file);
                wk = new XSSFWorkbook(fs);
                // 读取第一个sheet
                ISheet sheet = wk.GetSheetAt(0);
                IRow row = sheet.GetRow(0);
                for (int i = 1; i <= sheet.LastRowNum; i++) {
                    row = sheet.GetRow(i);
                    if (row != null) {
                        for (int j = 0; j < row.LastCellNum; j++) {
                            string value = row.GetCell(j).ToString();
                           
                            if (value != null) {
                                if (j == 0) passage.Title = value;
                                else if(j == 1) passage.Content = value;
                            }
                        }
                    }
                }
                fs.Close();
            }
            catch (Exception e) {
                Console.WriteLine(e.Message);
            }

            // 读取第二个sheet
            try {
                List<Question> questionList = new List<Question>();

                FileStream fs = await ConvertIFormFileToFileStream(file);
                wk = new XSSFWorkbook(fs);
                // 读取第二个sheet
                ISheet sheet = wk.GetSheetAt(1);
                IRow row = sheet.GetRow(0);
                for (int i = 1; i <= sheet.LastRowNum; i++) {
                    Question question = new Question();
                    row = sheet.GetRow(i);
                    if (row != null) {
                        for (int j = 0; j < row.LastCellNum; j++) {
                            string value = row.GetCell(j).ToString();
                            if (value != null) {
                                if (j == 0) question.Title = value;
                                else if (j == 1) question.OptionsA = value;
                                else if (j == 2) question.OptionsB = value;
                                else if (j == 3) question.OptionsC = value;
                                else if (j == 4) question.OptionsD = value;
                                else if (j == 5) question.Answer = value;
                                else if (j == 6) question.Explanation = value;
                            }
                        }
                        if (question.Answer != null)
                            questionList.Add(question);
                    }
                    passage.QuestionList = questionList;
                }
                fs.Close();
            }
            catch (Exception e) {
                Console.WriteLine(e.Message);
            }
            passage.Title = title;
            dbContext.Passages.Add(passage);
            
           return ( dbContext.SaveChanges() != 0);
        }

        public int AddPassageList(List<Passage> passageList) {
            
            dbContext.AddRange(passageList);
            return dbContext.SaveChanges();
        }

        public bool DeletePassageById(int id) {
            var passage = dbContext.Passages
                .Where(item => item.PassageId == id && item.DeleteSign == 0)
                .SingleOrDefault();
            if (passage == null) return false;
            // 形式上删除passage
            passage.DeleteSign = 1;
            // 删除question
            var questionList = dbContext.Questions
                .Where(item => item.PassageId == id && item.DeleteSign == 0)
                .ToList();
            foreach(var item in questionList) {
                item.DeleteSign = 1;
            }
           int count = dbContext.SaveChanges();
            // 删除Redis中的内容
            try {
                redisHelper.deleteKey(MyConstant.Passage + "-" + id);
                redisHelper.deleteKey(MyConstant.QuestionList + "-" + id);
                redisHelper.deleteKey(MyConstant.AnswerList + "-" + id);
                // 修改文章总数
                if (count != 0) {
                    int sum = redisHelper.getStringObject<int>(MyConstant.PassageTotal);
                    redisHelper.setString(MyConstant.PassageTotal, sum + 1);
                }
                // 删除sortset中的元素
                redisHelper.Zremove(MyConstant.PassageSortSet, id.ToString());
            } catch(Exception ex) {
                Console.WriteLine(ex.Message);
            }
            return (count != 0);
        }

        public PassageDTO GetPassageById(int id) {
           PassageDTO passageDTO = new PassageDTO();
            // 先从Redis中查找数据
            if(redisHelper.exist(MyConstant.Passage + "-" + id)) {
                passageDTO = redisHelper.getStringObject<PassageDTO>(MyConstant.Passage + "-" + id);
                return passageDTO;
            }
            // Redis中不存在，从MySQL中查找数据
            var passageResult = dbContext.Passages
                .Where(item => item.PassageId == id && item.DeleteSign == 0)
                .SingleOrDefault();
            if(passageResult == null) {
                return passageDTO;
            }
            // 将查询到结果转换为passageDTO类型
            PassageDTO saveRedis = new PassageDTO();
            saveRedis.PassageId = id;
            saveRedis.QuestionList = null;
            saveRedis.Title = passageResult.Title;
            saveRedis.Content = passageResult.Content;
            // 将saveRedis存储到Redis中
            try {
                redisHelper.setString(MyConstant.Passage + "-" + id, saveRedis);
            } catch(Exception ex) {
                Console.WriteLine(ex.Message);
            }
           return saveRedis;
        }

        public PassagePage GetPassageByPageSize(int page = 1, int size = 10) {
            PassagePage passagePage = new PassagePage();
            // 获取文章总数
            int count = GetPassageTotal();
            passagePage.Total = count;
            List<int> idList = new List<int>();
            // 从sortset中获取数据
            idList = redisHelper.Zrange<int>(MyConstant.PassageSortSet, (page - 1) * size, page * size -1);
            if (idList.Count > 0) {
                // 根据id获取阅读理解文章
                foreach (int id in idList) {
                    // sortset是后来添加的，在做删除和添加的时候没有修改数据
                    // 使用try-catch防止程序运行到这里报错
                    try {
                        passagePage.PassageDTOList.Add(GetPassageById(id));
                    }
                    catch (Exception ex) {
                        Console.WriteLine(ex.Message);
                    }
                }
                return passagePage;
            }
            // 从MySQL中获取数据并将获取到的数据存储到sortset中
            var result = dbContext.Passages
                 .Where(item => item.DeleteSign == 0)
                 .Skip((page - 1) * size)
                 .Take(size)
                 .ToList()
                 .Select(item => new { item.PassageId });
            if (result == null) return passagePage;
            List<int> valueList = new List<int>();
            foreach (var item in result) {
                valueList.Add(item.PassageId);
            }
            // 将获取到的结果存储到sortset中
            redisHelper.Zadd(MyConstant.PassageSortSet, valueList, valueList);
            // 根据id获取值
            foreach (int id in valueList) {
                // sortset是后来添加的，在做删除和添加的时候没有修改数据
                // 使用try-catch防止程序运行到这里报错
                try {
                    passagePage.PassageDTOList.Add(GetPassageById(id));
                }
                catch (Exception ex) {
                    Console.WriteLine(ex.Message);
                }
            }
            return passagePage;
        }

        public PassagePage AdminGetPassageByPageSize(int page = 1, int size = 10, int delete = 0) {
            
            PassagePage passagePage = new PassagePage();
            // 先获取总数
            int total = dbContext.Passages
                .Where(item => item.DeleteSign == delete)
                .Count();
            passagePage.Total = total; 
            if(total == 0) {
                passagePage.Total = 0;
                return passagePage;
            }
            // 分页获取id数据
            var result = dbContext.Passages
                .Where(item => item.DeleteSign == delete)
                .Skip((page - 1) * size)
                .Take(size)
                .ToList();
           foreach(var item in result) {
                PassageDTO passageDTO = new PassageDTO();
                passageDTO.QuestionList = new List<QuestionDTO>();
                passageDTO.PassageId = item.PassageId;
                passageDTO.Content = item.Content;
                passageDTO.Title = item.Title;
                passagePage.PassageDTOList.Add(passageDTO);
            }
            return passagePage;
        }
        public int GetPassageTotal() {
            // 从MySQL中获取数据
            int count = dbContext.Passages
                .Where(item => item.DeleteSign == 0)
                .Count();
            // 将数据存储到Redis中
            redisHelper.setString(MyConstant.PassageTotal, count);
            return count;
        }

        public bool RecoverPassageById(int id) {
            var passage = dbContext.Passages
                .Where(item => item.PassageId == id && item.DeleteSign == 1)
                .SingleOrDefault();
            if (passage == null) return false;
            // 将删除标志位设置为0表示没有删除
            passage.DeleteSign = 0;
            var questionList = dbContext.Questions
                .Where(item => item.PassageId == id && item.DeleteSign == 1)
                .ToList();
            foreach(var item in  questionList) {
                item.DeleteSign = 0;
            }
            int count = dbContext.SaveChanges();
            // 修改文章总数
            if(count != 0) {
               int sum = redisHelper.getStringObject<int>(MyConstant.PassageTotal);
                redisHelper.setString(MyConstant.PassageTotal, sum + 1);
            }
            // 向sortset中添加元素
            try {
                redisHelper.Zadd(MyConstant.PassageSortSet, id, id);
            } catch(Exception ex) {
                Console.WriteLine(ex.Message);
                Console.WriteLine("添加到sortset中失败");
            }
         
            return count != 0;
        }

        public bool UpdatePassage(PassageDTO passageDTO) {
           
            var result = dbContext.Passages
                .Where(item => item.PassageId == passageDTO.PassageId)
                .SingleOrDefault();
            if(result == null) {
                return false;
            }
            result.Title = passageDTO.Title;
            result.Content = passageDTO.Content;
           int count =  dbContext.SaveChanges();
            // 删除Redis中的数据
            try {
                redisHelper.deleteKey(MyConstant.Passage + "-" + passageDTO.PassageId);
            } catch (Exception ex) {
                Console.WriteLine(ex.Message);
            }
            return (count == 1);
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
