using EnglishStudy.DTO;
using EnglishStudy.Entity;
using EnglishStudy.Utils;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace EnglishStudy.Service.ServiceImpl {
    public class EveryDayEnglishImpl : IEveryDayEnglish {

        private MyDbContext dbContext;

        private RedisHelper redisHelper = new RedisHelper();

        public EveryDayEnglishImpl(MyDbContext dbContext) {
            this.dbContext = dbContext;
        }

        // 存在的问题，传过来的集合中某个id可能已经被删除了，可能导致程序出错
        public int Delete(List<int> idList) {
            int count = 0;
            // 使用in查询获取数据
            var result = dbContext.EveryDayEnglishes
                .Where(item => idList.Contains(item.Everyday_id))
                .ToList();
           
            // 将删除标记位赋为1
            for (int i = 0; i < result.Count; i++)
                result[i].Delete_Sign = 1;
             count = dbContext.SaveChanges();
            if(count != 0) {
                try {
                    // 修改每日英语总数
                    int sum = redisHelper.getStringObject<int>(MyConstant.EveryDayEnglishTotal);
                    redisHelper.setString(MyConstant.EveryDayEnglishTotal, sum - count);
                   // 忘记写批量删除的Redis工具类
                    foreach (int item in idList) {
                        // 修改sortset
                        redisHelper.Zremove(MyConstant.EveryDayEnglishSortSet, item.ToString());
                        // 删除string的值
                        redisHelper.deleteKey(MyConstant.EveryDayEnglishString + "-" + item);
                    }
                }
                catch (Exception ex) {
                    Console.WriteLine(ex.Message);
                }
            }
            return count;
        }

        public EveryDayEnglish GetById(int id) {
            // 先从Redis中获取数据
            string key = MyConstant.EveryDayEnglishString + "-" + id;
            EveryDayEnglish everyDayEnglish = new EveryDayEnglish();
            // 判断Redis中是否存在数据
            bool exist = redisHelper.exist(key);
            if (exist) {
                everyDayEnglish = redisHelper.getStringObject<EveryDayEnglish>(key);
                return everyDayEnglish;
            }
            // 从MySQL中获取
            var result = dbContext.EveryDayEnglishes
                .Where(item => item.Everyday_id == id && item.Delete_Sign == 0)
                .SingleOrDefault();
            // 如果MySQL中不存在直接返回空值
            if (result == null) {
                return everyDayEnglish;
            }
            // 将获取到的值存储到Redis中
            everyDayEnglish.Everyday_id = id;
            everyDayEnglish.Title = result.Title;
            everyDayEnglish.Content = result.Content;
            everyDayEnglish.TitleTranslations = result.TitleTranslations;
            everyDayEnglish.Translations = result.Translations;
            everyDayEnglish.Delete_Sign = result.Delete_Sign;
            everyDayEnglish.Time = result.Time;

            redisHelper.setString(key, everyDayEnglish);
            return everyDayEnglish;

        }

        // 详见 https://learn.microsoft.com/zh-cn/ef/core/querying/pagination偏移分页
        // 如果数据库中存在1~11条数据，网络请求首先请求第一页数据即1-10条数据
        // 这个时候1-10条数据会被添加到Redis中的sortset中，这个时候如果1,2条数据
        // 删除了，那么sortset中的数据也会被删除，这个时候网络请求第一页数据，那么
        // 会命中sortset中的数据，此时应该第11条数据没有被加载到Redis中，导致11条
        // 数据没有被返回
        // 可以根据EveryDayEnglishRecordImpl中的分页数据获取方法修改，思路是一样的
        public EveryDayEnglishPage GetByPageSize(int page = 1, int size = 10) {
            EveryDayEnglishPage everyDayPage = new EveryDayEnglishPage();
            // 获取总数
            everyDayPage.Total = GetTotal();
            if(everyDayPage.Total == 0) return everyDayPage;
            // 从sortset中获取分页数据
            var result = redisHelper.Zrange<int>(MyConstant.EveryDayEnglishSortSet, (page - 1) * size, page * size - 1);
            if (result.Count > 0) {
                // 根据id获取每日英语
                foreach(int item in result) {
                    // 将结果添加到返回值中
                   everyDayPage.List.Add(GetById(item));
                }
                return everyDayPage;
            }
            // 从数据中获取id集合
            var Ids = dbContext.EveryDayEnglishes
                .Where(item => item.Delete_Sign == 0)
                .Skip((page - 1) * size)
                .Take(size)
                .ToList()
                .Select(item => new { item.Everyday_id });
            
            // 将ids存储到Redis中
            List<int> valueList = new List<int>();
            foreach(var item in Ids) {
                valueList.Add(item.Everyday_id);
            }
            redisHelper.Zadd(MyConstant.EveryDayEnglishSortSet, valueList, valueList);
            // 根据Ids中的值获取具体的每日英语
            foreach(var item in Ids) {
                everyDayPage.List.Add(GetById(item.Everyday_id));
            }
            return everyDayPage;
        }

        public EveryDayEnglishPage AdminGetByPageSize(int page = 1, int size = 10, int delete = 0) {
            EveryDayEnglishPage everyDayEnglishPage = new EveryDayEnglishPage();
            // 从MySQL中获取total
            int total = dbContext.EveryDayEnglishes
                .Where(item => item.Delete_Sign == delete)
                .Count();
            everyDayEnglishPage.Total = total;
            if (total == 0) return everyDayEnglishPage;
            // 从MySQL中获取size个数据
            var result = dbContext.EveryDayEnglishes
               .Where(item => item.Delete_Sign == delete)
               .Skip((page - 1) * size)
               .Take(size)
               .ToList();
            everyDayEnglishPage.List = result;
            return everyDayEnglishPage;
        }


        public int GetTotal() {
            // 先从redis中获取数据
            bool exist = redisHelper.exist(MyConstant.EveryDayEnglishTotal);
            int count = 0;
            if (exist) {
                count = redisHelper.getStringObject<int>(MyConstant.EveryDayEnglishTotal);
                return count;
            }
            // 从MySQL中获取数据
            count = dbContext.EveryDayEnglishes
                 .Where(item => item.Delete_Sign == 0)
                 .Count();
            // 将count存储到Redis中
            redisHelper.setString(MyConstant.EveryDayEnglishTotal, count);
            return count;
        }

        public bool Update(EveryDayEnglish everyDayEnglish) {
            // 先从MySQL中查找数据
            var result = dbContext.EveryDayEnglishes
                .Where(item => item.Everyday_id == everyDayEnglish.Everyday_id)
                .SingleOrDefault();
            if (result == null) return false;
            // 修改数据
            result.Title = everyDayEnglish.Title;
            result.TitleTranslations = everyDayEnglish.TitleTranslations;
            result.Content = everyDayEnglish.Content;
            result.Translations = everyDayEnglish.Translations;
            result.Time = DateTime.Now; 
            // 执行savechange
            int count = dbContext.SaveChanges();
            // 如果修改成功，修改Redis中的数据
            if(count == 1) {
                try {
                    if(redisHelper.exist(MyConstant.EveryDayEnglishString + "-" + everyDayEnglish.Everyday_id))
                    redisHelper.deleteKey(MyConstant.EveryDayEnglishString + "-" + everyDayEnglish.Everyday_id);
                }
                catch (Exception ex) {
                    Console.WriteLine(ex.ToString());
                }
                return true;
            }
            return false;
            
        }

        // 这里和delete进行相反的操作就行了
        public int Recover(List<int> idList) {
            int count = 0;
            // 使用in查询获取数据
            var result = dbContext.EveryDayEnglishes
                .Where(item => idList.Contains(item.Everyday_id))
                .ToList();

            // 将删除标记位赋为1
            for (int i = 0; i < result.Count; i++)
                result[i].Delete_Sign = 0;
            count = dbContext.SaveChanges();
            if (count != 0) {
                try {
                    // 修改每日英语总数
                    int sum = redisHelper.getStringObject<int>(MyConstant.EveryDayEnglishTotal);
                    redisHelper.setString(MyConstant.EveryDayEnglishTotal, sum + count);
                    // 忘记写批量删除的Redis工具类
                    redisHelper.Zadd(MyConstant.EveryDayEnglishSortSet, idList, idList);
                }
                catch (Exception ex) {
                    Console.WriteLine(ex.Message);
                }
            }
            return count;
        }

        public async Task<int> UploadFile(IFormFile file) {
            int count = 0;
            var filePath = file.FileName;
            // 通过文件扩展名判断文件是否为excel
            string extension = Path.GetExtension(filePath);
            // 不是excel文件直接返回
            if (!extension.Equals(".xls") && !extension.Equals(".xlsx")) {
                Console.WriteLine(filePath);
                Console.WriteLine("不是excel文件");
                return count;
            }
            List<EveryDayEnglish> list = new List<EveryDayEnglish>();
            IWorkbook wk = null;

            try {
                FileStream fs = await ConvertIFormFileToFileStream(file);
                wk = new XSSFWorkbook(fs);
                // 读取第一个sheet
                ISheet sheet = wk.GetSheetAt(0);
                IRow row = sheet.GetRow(0);
                for (int i = 1; i <= sheet.LastRowNum; i++) {
                    EveryDayEnglish everyDayEnglish = new EveryDayEnglish();
                    row = sheet.GetRow(i);
                    if (row != null) {
                        for (int j = 0; j < row.LastCellNum; j++) {
                            string value = row.GetCell(j).ToString();
                            if (value != null) {
                                if (j == 0) everyDayEnglish.Title = value;
                                else if (j == 1) everyDayEnglish.TitleTranslations = value;
                                else if (j == 2) everyDayEnglish.Content = value;
                                else if (j == 3) everyDayEnglish.Translations = value;
                            }
                        }
                    }
                    everyDayEnglish.Time = DateTime.Now;
                    list.Add(everyDayEnglish);
                }
                fs.Close();
            }
            catch (Exception e) {
                Console.WriteLine(e.Message);
            }

            // 将获取到的数据存储到MySQL中
            await dbContext.EveryDayEnglishes.AddRangeAsync(list);
            count = dbContext.SaveChanges();
            // 修改Redis中的每日英语数量
            try {
                // 如果Redis中存在总数的话，直接修改Redis中的值
                if(redisHelper.exist(MyConstant.EveryDayEnglishTotal)) {
                    int pre_count = redisHelper.getStringObject<int>(MyConstant.EveryDayEnglishTotal);
                    redisHelper.setString(MyConstant.EveryDayEnglishTotal, pre_count + count);
                }
                // 直接从数据库中获取总数，因为数据才才插入到数据库中，所以可以获取到最新值
                GetTotal();
            } catch(Exception e) { Console.WriteLine(e.Message); }
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
