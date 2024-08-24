using EnglishStudy.DTO;
using EnglishStudy.DTO.MyListening;
using EnglishStudy.Entity;
using EnglishStudy.Utils;
using Microsoft.EntityFrameworkCore;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace EnglishStudy.Service.ServiceImpl {
    public class ListeningPaperServiceImpl : IListeningPaperService {

        private MyDbContext dbContext;

        private RedisHelper redisHelper = new RedisHelper();

        public readonly static string savaPath = @"D:\nginx\audio\";

        public readonly static string accessPath = @"https://localhost:7031/resouce/audio/";
        public ListeningPaperServiceImpl(MyDbContext dbContext) {
            this.dbContext = dbContext;
        }

        public bool addQuestion(int listeningPaperId, ListeningQuestion listeningQuestion) {
            // 向数据库中添加数据
            dbContext.ListeningQuestions.Add(listeningQuestion);
            // 执行事务
            int count = dbContext.SaveChanges();
            // 事务执行失败，返回false
            if(count == 0) {
                return false;
            }
            // 执行成功，删除Redis中的paper数据
            int id = listeningPaperId;
            redisHelper.ZremoveByScore(MyConstant.ListeningPaperSortSet, id, id);
            return true;
        }

        public bool addPart(AddPartDTO addPartDTO) {
            // 将封装类的数据转换为数据库中的实体类
            Part part = new Part();
            part.PartTitle = addPartDTO.PartTitle;
            part.OriginalText = addPartDTO.OriginalText;
            part.ListeningQuestionList = addPartDTO.ListeningQuestionList;
            part.ListeningPaperId = addPartDTO.listeningPaperId;
            // 直接向数据库中添加数据
            dbContext.Parts.Add(part);
            // 执行事务
            int count = dbContext.SaveChanges();
            // 如果事务执行失败
            if(count == 0) {
                return false;
            }
            // 事务执行成功，从Redis中删除对应的数据
            int id = addPartDTO.listeningPaperId;
            redisHelper.ZremoveByScore(MyConstant.ListeningPaperSortSet, id, id);
            return true;
        }

        public bool updateTitleOrAudio(ListeningPaperDTO listeningPaper) {
            string title = listeningPaper.PaperTitle;
            string audio = listeningPaper.Audio;
            int id = listeningPaper.Id;
            // 根据id获取数据
            var result = dbContext.ListeningPapers
                  .Where(item => item.ListeningPaperId == id && item.DeleteSign == 0)
                  .SingleOrDefault();
            // 获取到空值，返回错误
            if (result == null) {
                return false;
            }
            // 修改title
            if (title.Equals("null") == false) {
                result.PaperTitle = title;
            }
            // 修改audio
            if (audio.Equals("null") == false) {
                result.Audio = audio;
            }
            int count = dbContext.SaveChanges();
            if (count == 0) {
                return false;
            }
            // 删除Redis中的数据
            redisHelper.ZremoveByScore(MyConstant.ListeningPaperSortSet, id, id);
            return true;
        }


        public bool deleteQuestionById(int listeningPaperId, int PartId, int questionId) {
            // 这里我不准备逻辑删除了，直接物理删除数据库中的数据
            // 逻辑删除的话要改很多的代码

            // 从数据库中查找数据
            var result = dbContext.ListeningQuestions
                .Where(item => item.DeleteSign == 0 
                && item.QuestionId == questionId
                && item.PartId == PartId)
                .SingleOrDefault();
            // 如果没有查找到数据，返回false
            if(result == null) {
                return false;
            }
            // 直接删除数据
            dbContext.Remove(result);
            int count = dbContext.SaveChanges();
            // 判断事务是否执行成功
            if(count == 0) {
                // 执行失败，返回false
                return false;
            }
            // 删除Redis中的数据
            redisHelper.ZremoveByScore(MyConstant.ListeningPaperSortSet, listeningPaperId, listeningPaperId);
            return true;
        }
      
        public bool deletePartById(int listeningPaperId, int partId) {
            // 从数据库中查找相对应的数据
            var result = dbContext.Parts
                .Where(item => item.DeleteSign == 0
                && item.PartId == partId && item.ListeningPaperId == listeningPaperId)
                .SingleOrDefault();
            // 如果没有查找到数据，返回false
            if(result == null) {
                return false;
            }
            // 将删除标记位设置为1
            result.DeleteSign = 1;
            // 执行事务
            int count = dbContext.SaveChanges();
            // 如果事务执行失败，返回false
            if(count == 0) {
                return false;
            }
            // 删除Redis中的数据
            redisHelper.ZremoveByScore(MyConstant.ListeningPaperSortSet, listeningPaperId, listeningPaperId);
            return true;
        }

        public bool updateQuestionById(int listeningPaperId,
            int partId, int questionId, ListeingQuestionDTO listeingQuestionDTO) {
            // 从数据库中查找数据
            var result = dbContext.ListeningQuestions
                .Where(item => item.DeleteSign == 0 
                && item.PartId == partId && item.QuestionId == questionId)
                .SingleOrDefault();
            // 如果没有查找到数据，返回false
            if(result == null) {
                return false;
            }
            // 修改数据
            result.OptionsA = listeingQuestionDTO.OptionsA;
            result.OptionsB = listeingQuestionDTO.OptionsB;
            result.OptionsC = listeingQuestionDTO.OptionsC;
            result.OptionsD = listeingQuestionDTO.OptionsD;
            result.Answer = listeingQuestionDTO.Answer;
            result.Title = listeingQuestionDTO.Title;
            // 执行事务
            int count = dbContext.SaveChanges();
            // 事务执行失败，返回false
            if(count == 0) {
                return false;
            }
            // 事务执行成功，删除Redis中的数据
            redisHelper.ZremoveByScore(MyConstant.ListeningPaperSortSet, listeningPaperId, listeningPaperId);
            return true;

        }

        public bool updatePartById(int listeningPaperId, int partId, UpdatePartDTO updatePartDTO) {
            // 从数据库中查找数据
            var result = dbContext.Parts
                .Where(item => item.DeleteSign == 0
                && item.ListeningPaperId == listeningPaperId
                && item.PartId == partId)
                .SingleOrDefault();
            // 如果没有查找到数据，返回false
            if(result == null) {
                return false;
            }
            // 修改数据
            result.PartTitle = updatePartDTO.PartTitle;
            result.OriginalText = updatePartDTO.OriginalText;
            // 执行事务
            int count = dbContext.SaveChanges();
            // 事务执行失败，返回false
            if(count == 0) {
                return false;
            }
            // 事务执行成功，删除Redis中的数据
            redisHelper.ZremoveByScore(MyConstant.ListeningPaperSortSet, listeningPaperId, listeningPaperId);
            return true;
        }



        // 答案啥的也返回吧，慢慢细分感觉太麻烦了，逻辑更加复杂
        public ListeningPaper GetByListeningPaperId(int listeningPaperId) {
            string key = MyConstant.ListeningPaperSortSet;
            // 从Redis中获取数据
            var result = redisHelper.ZrangeByScore<ListeningPaper>(key, listeningPaperId, listeningPaperId);
            // 获取到唯一的一条数据，返回结果
            if (result.Count != 0) {
                return result[0];
            }
            // 从MySQL中获取数据
            // 先从listeningPaper表中查询，再从part表中查询，将
            // part表中的内容添加到listeningPaper表中（其实可以使用group by进行
            // 查询的，即将listeningQuestion中的数据按照partid分组组成一张新表，最后
            // 三张表联查，但是SQL的话太复杂了，不想写，那么就分部获取吧，牺牲时间）
            var sqlResult = dbContext.ListeningPapers
                .Where(item => item.ListeningPaperId == listeningPaperId && item.DeleteSign == 0)
                .SingleOrDefault();

            if (sqlResult == null) {
                // MySQL中不存在数据
                return null;
            }
            // 从part表中查询
            var partReult = dbContext.Parts
                .Where(item => item.ListeningPaperId == listeningPaperId
                && item.DeleteSign == 0)
                .Include(item => item.ListeningQuestionList)
                .ToList();

            // 将partResult添加到sqlResult中
            // sqlResult.PartList.AddRange(partReult); 
            // 上面那一行代码会重复将partList添加到sqlresult中，我不清楚具体的原因，
            // 如果没有写查找partList的代码的话，sqlReuslt中的partList的值为空，但是写了查询之后
            // 我并没有把partList手动添加到sqlResult中，可能是EF Core自动帮我添加了

            // 将获取到的结果存放到Redis中
            redisHelper.Zadd(key, sqlResult, listeningPaperId);
            // 返回结果
            return sqlResult;

        }

        // 之删除主表中的数据就行了，从表中的数据只能通过主表id获得
        public bool DeleteByListeningPaperId(int listeningPaperId) {
            // 先获取总数
            int total = GetTotal();
            // 先从MySQL中获取要被删除的数据
            var result = dbContext.ListeningPapers
                .Where(item => item.ListeningPaperId == listeningPaperId && item.DeleteSign == 0)
                .SingleOrDefault();
            if (result == null) {
                // 数据不存在
                return false;
            }
            // 将获取到的数据删除标记位设置为1
            result.DeleteSign = 1;
            int count = dbContext.SaveChanges();
            // savechanges执行成功
            if (count == 1) {
                // 删除sortset中的数据，并且修改total
                redisHelper.ZremoveByScore(MyConstant.ListeningPaperSortSet, listeningPaperId, listeningPaperId);
                redisHelper.setString(MyConstant.ListeningPaperTotal, total - 1);
            }
            return (count == 1);
        }

        public ListeningPaperPage GetByPageSize(int page = 1, int size = 10) {
            ListeningPaperPage listeningPaperPage = new ListeningPaperPage();
            // 获取总条数
            int total = GetTotal();
            // 没有一条数据，直接返回结果
            if (total == 0) {
                return listeningPaperPage;
            }
            listeningPaperPage.Total = total;
            // 从Redis中查找分页的数据
            string key = MyConstant.ListeningPaperSortSet;
            var redisResult = redisHelper.Zrange<ListeningPaper>(key, (page - 1) * size, page * size - 1);

            // 从Redis中获取到了数据
            if (redisResult.Count > 0) {
                listeningPaperPage.List = redisResult;
                if (redisResult.Count == size) {
                    // 获取到了所有的数据，返回结果
                    return listeningPaperPage;
                }
                /*从运行结果来看，不加如下判断会执行无效的SQL代码
                从而导致程序效率下降*/
                if (total == (page - 1) * size + redisResult.Count) {
                    return listeningPaperPage;
                }
                // 还差size-redisResult.Count条数据从MySQL中获取
                int lastId = redisResult[redisResult.Count - 1].ListeningPaperId;
                // 从MySQL中获取剩余的数据
                // 先获取listeningPaperId
                var IdResult = dbContext.ListeningPapers
                    .Where(item => item.DeleteSign == 0 && item.ListeningPaperId > lastId)
                    .Take(size - redisResult.Count)
                    .Select(item => new { item.ListeningPaperId })
                    .ToList();
                // 根据id获取数据
                List<ListeningPaper> listeningPapers = new List<ListeningPaper>();
                List<int> intList = new List<int>();
                foreach (var item in IdResult) {
                    listeningPapers.Add(GetByListeningPaperId(item.ListeningPaperId));
                    intList.Add(item.ListeningPaperId);
                }
                // 将listeningPaper中的数据存储到Redis中
                redisHelper.Zadd(key, listeningPapers, intList);
                // 将listeningPaper中的数据添加到listeningPaperPage中
                listeningPaperPage.List.AddRange(listeningPapers);
                // 返回数据
                return listeningPaperPage;

            }
            // 从MySQL中查找分页数据
            // 先查找listeningPaperId
            var myResult = dbContext.ListeningPapers
                .Where(item => item.DeleteSign == 0)
                .Skip((page - 1) * size)
                .Take(size)
                .ToList()
                .Select(item => new { item.ListeningPaperId });
            // 根据id获取数据
            List<ListeningPaper> MyList = new List<ListeningPaper>();
            List<int> scoreList = new List<int>();
            foreach (var item in myResult) {
                MyList.Add(GetByListeningPaperId(item.ListeningPaperId));
                scoreList.Add(item.ListeningPaperId);
            }
            // 将获取到的数据存储到Redis中
            redisHelper.Zadd(key, MyList, scoreList);
            // 奖获取到的数据添加到返回类中
            listeningPaperPage.List = MyList;
            return listeningPaperPage;

        }


        public async Task<string> uploadAudio(IFormFile file) {
            string url = await SaveAudio(file);
            return url;
        }

        // EF core真的好用，居然可以同时级联插入三张表
        // 能运行成功，但是有时候存在bug，程序会卡着不动
        public async Task<int> AddListeningPaper(List<IFormFile> files, string PaperTitle) {
            // 获取听力试题总数
            int total = GetTotal();
            ListeningPaper listeningPaper = new ListeningPaper();
            string path = "";
            foreach (IFormFile file in files) {
                string extension = Path.GetExtension(file.FileName);

                if (extension.Equals(".wma")
                    || extension.Equals(".wav")
                    || extension.Equals(".flac")
                    || extension.Equals(".alac")
                    || extension.Equals(".mp3")) {
                    // 将音频文件保存到路径中
                    path = await SaveAudio(file);

                    if (path.Equals("")) return -1;
                }
                else if (extension.Equals(".xls") || extension.Equals(".xlsx"))
                    // 将excel文件解析
                    listeningPaper.PartList = await ParseFile(file);
            }

            if (listeningPaper.PartList.Count == 0) return -1;
            listeningPaper.PaperTitle = PaperTitle;
            listeningPaper.Audio = accessPath + path;
            dbContext.ListeningPapers.Add(listeningPaper);

            int count = dbContext.SaveChanges();
            if (count == 0) return -1;
            // 修改听力试题总数
            redisHelper.setString(MyConstant.ListeningPaperTotal, total + 1);
            return listeningPaper.ListeningPaperId;
        }

        public int addSingle(ListeningPaper listening) {
            // 获取听力试题总数
            int total = GetTotal();
            dbContext.ListeningPapers.Add(listening);
            int count = dbContext.SaveChanges();
            if (count == 0) return -1;
            // 修改听力试题总数
            redisHelper.setString(MyConstant.ListeningPaperTotal, total + 1);
            return listening.ListeningPaperId;
        }

        public int GetTotal() {
            int total = 0;
            // 从Redis中查找
            string key = MyConstant.ListeningPaperTotal;
            if (redisHelper.exist(key)) {
                total = redisHelper.getStringObject<int>(key);
                return total;
            }
            // 从MySQL中查找
            total = dbContext.ListeningPapers
                .Where(item => item.DeleteSign == 0)
                .Count();
            // 将获取到的数据存储到Redis中
            redisHelper.setString(key, total);
            // 返回数据
            return total;
        }

        //保存音频文件
        public async Task<string> SaveAudio(IFormFile file) {

            // 获取文件后缀
            string suffix = Path.GetExtension(file.FileName);
            // 将文件保存
            try {
                string fileName = Guid.NewGuid().ToString();
                fileName = fileName + suffix;
                string saveName = savaPath + fileName;
                using (var stream = new FileStream(saveName, FileMode.Create)) {
                    await file.CopyToAsync(stream);
                }
                return accessPath +  fileName;
            }
            catch (Exception ex) {
                Console.WriteLine(ex.ToString());
                return "";
            }
        }

        // 解析excel
        public async Task<List<Part>> ParseFile(IFormFile file) {
            List<Part> PartList = new List<Part>();
            var filePath = file.FileName;
            // 通过文件扩展名判断文件是否为excel
            string extension = Path.GetExtension(filePath);
            // 不是excel文件直接返回
            if (!extension.Equals(".xls") && !extension.Equals(".xlsx")) {
                Console.WriteLine(filePath);
                Console.WriteLine("不是excel文件");
                return PartList;
            }
            List<int> sumList = new List<int>();
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
                        Part part = new Part();
                        for (int j = 0; j < row.LastCellNum; j++) {
                            string value = row.GetCell(j).ToString();
                            if (value != null) {
                                if (j == 0) {
                                    part.PartTitle = value;
                                }
                                else if (j == 1) {
                                    part.OriginalText = value;
                                }
                                else if (j == 2) {
                                    sumList.Add(int.Parse(value));
                                }
                            }

                        }
                        PartList.Add(part);
                    }
                }
                fs.Close();
            }
            catch (Exception e) {
                Console.WriteLine(e.Message);
                return PartList;
            }
            // 读取第二个sheet
            List<ListeningQuestion> list = new List<ListeningQuestion>();
            try {

                FileStream fs = await ConvertIFormFileToFileStream(file);
                wk = new XSSFWorkbook(fs);
                // 读取第二个sheet
                ISheet sheet = wk.GetSheetAt(1);
                IRow row = sheet.GetRow(0);
                int sum = 0;
                for (int i = 1; i <= sheet.LastRowNum; i++) {
                    ListeningQuestion listeningQuestion = new ListeningQuestion();
                    row = sheet.GetRow(i);
                    if (row != null) {
                        for (int j = 0; j < row.LastCellNum; j++) {
                            string value = row.GetCell(j).ToString();
                            if (value != null) {
                                if (j == 0) listeningQuestion.Title = value;
                                else if (j == 1) listeningQuestion.OptionsA = value;
                                else if (j == 2) listeningQuestion.OptionsB = value;
                                else if (j == 3) listeningQuestion.OptionsC = value;
                                else if (j == 4) listeningQuestion.OptionsD = value;
                                else if (j == 5) listeningQuestion.Answer = value;
                            }
                        }

                    }
                    list.Add(listeningQuestion);
                }
                Console.WriteLine("308-" + list.Count);
                int count = 0;
                for (int i = 0; i < PartList.Count; i++) {
                    for (int j = 0; j < sumList[i]; j++) {
                        PartList[i].ListeningQuestionList.Add(list[count]);
                        count++;
                    }
                }
                fs.Close();
            }
            catch (Exception e) {
                Console.WriteLine(e.Message);
            }
            return PartList;
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
