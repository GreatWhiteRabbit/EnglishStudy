using EnglishStudy.DTO.Resource;
using EnglishStudy.Entity;
using EnglishStudy.Utils;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EnglishStudy.Service.ServiceImpl {
    public class MyResourceServiceImpl : IMyResourceService {

        private MyDbContext dbContext;

        private RedisHelper redisHelper = new RedisHelper();

        private const string savePath = @"D:\nginx\download\";
        private const string accessURL = @"https://localhost:7031/resouce/download/";

        public MyResourceServiceImpl(MyDbContext dbContext) {
            this.dbContext = dbContext;
        }



        public bool AddResource(Resource resource) {
            resource.Time = DateTime.Now;
            resource.Delete_Sign = true;
            dbContext.Resources.Add(resource);
            int count = dbContext.SaveChanges();
            if(count != 0) {
                return true;
            }
            return false;
        }

        public ResourceDTOPage AdminGetResource(int page = 1, int size = 10) {
            ResourceDTOPage resourceDTOPage = new ResourceDTOPage();
            // 获取总数
            int total = dbContext.Resources.Count();
            if(total == 0) {
                return resourceDTOPage;
            }
            resourceDTOPage.Total = total;
            // 根据时间分页获取数据
            var sqlResult = dbContext.Resources
                .OrderByDescending(item => item.Time)
                .Skip((page - 1) * size)
                .Take(size)
                .ToList();
            List<ResourceDTO> list = new List<ResourceDTO>();
            for(int i = 0; i < sqlResult.Count; i++) {
                ResourceDTO resourceDTO = new ResourceDTO();
                resourceDTO.ResourceId = sqlResult[i].ResourceId;
                resourceDTO.Name = sqlResult[i].Name;
                resourceDTO.Url = sqlResult[i].Url;
                resourceDTO.Image = sqlResult[i].Image;
                resourceDTO.Sum = sqlResult[i].Sum;
                resourceDTO.Time = sqlResult[i].Time;
                resourceDTO.Delete_Sign = sqlResult[i].Delete_Sign;
                list.Add(resourceDTO);
            }
            resourceDTOPage.List = list;
            return resourceDTOPage;

        }

        public bool DeleteResource(int resourceId) {
            // 先获取文件路径
            var sqlResult = dbContext.Resources
                .Where(item => item.ResourceId == resourceId)
                .SingleOrDefault();
            if(sqlResult  == null) {
                return false;
            }
            // 获取文件名
            string fileName = sqlResult.Url.Split(accessURL)[1];

            // 获取文件路径
            string file = savePath + fileName;
  
            if(File.Exists(file)) {
                File.Delete(file);
            }
            dbContext.Resources.Remove(sqlResult);
            dbContext.SaveChanges();
            return true;
        }

        public string GetCode() {
            // 首先从Redis中获取code
            string code = "dbrabbit";
            try {
                bool exist = redisHelper.exist(MyConstant.ResourceCode);
                if (exist) {
                    code =  redisHelper.getStringObject<string>(MyConstant.ResourceCode);
                }
                else {
                    redisHelper.setString<string>(MyConstant.ResourceCode, code);
                }
                return code;
            } catch (Exception ex) {
                Console.WriteLine(ex.Message);
                return code;
            }
            
        }

        public bool UpdateCode(string code) {
            try {
                return redisHelper.setString<string>(MyConstant.ResourceCode, code);
            } catch(Exception ex) {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public ResourceDTOPage GetResource(int page = 1, int size = 10) {
            ResourceDTOPage resourceDTOPage = new ResourceDTOPage();
            int total = dbContext.Resources
                .Where(item => item.Delete_Sign == false)
                .Count();
            if(total == 0) {
                return resourceDTOPage;
            }
            resourceDTOPage.Total = total;
            // 按照下载量和时间分页获取资源
            var sqlResult = dbContext.Resources
                .Where(item => item.Delete_Sign == false)
                .OrderByDescending(item => item.Sum)
                .ThenByDescending(item => item.Time)
                .Skip((page - 1) * size)
                .Take(size)
                .ToList();
            List<ResourceDTO> list = new List<ResourceDTO>();
            for (int i = 0; i < sqlResult.Count; i++) {
                ResourceDTO resourceDTO = new ResourceDTO();
                resourceDTO.ResourceId = sqlResult[i].ResourceId;
                resourceDTO.Name = sqlResult[i].Name;
                resourceDTO.Image = sqlResult[i].Image;
                resourceDTO.Sum = sqlResult[i].Sum;
                resourceDTO.Time = sqlResult[i].Time;
                list.Add(resourceDTO);
            }
            resourceDTOPage.List = list;
            return resourceDTOPage;
        }

        public ResourceDTOPage SearchResource(string keyword,int page, int size) {
            
            ResourceDTOPage resourceDTOPage = new ResourceDTOPage();
            int total = dbContext.Resources
                .Where(item => item.Delete_Sign == false && 
                item.Name.Contains(keyword))
                .Count();
            if (total == 0) {
                return resourceDTOPage;
            }
            resourceDTOPage.Total = total;
            // 按照下载量和时间分页获取资源
            var sqlResult = dbContext.Resources
                .Where(item => item.Delete_Sign == false &&
                item.Name.Contains(keyword))
                .OrderBy(item => item.Sum)
                .OrderByDescending(item => item.Time)
                .Skip((page - 1) * size)
                .Take(size)
                .ToList();
            List<ResourceDTO> list = new List<ResourceDTO>();
            for (int i = 0; i < sqlResult.Count; i++) {
                ResourceDTO resourceDTO = new ResourceDTO();
                resourceDTO.ResourceId = sqlResult[i].ResourceId;
                resourceDTO.Name = sqlResult[i].Name;
                resourceDTO.Image = sqlResult[i].Image;
                resourceDTO.Sum = sqlResult[i].Sum;
                resourceDTO.Time = sqlResult[i].Time;
                list.Add(resourceDTO);
            }
            resourceDTOPage.List = list;
            return resourceDTOPage;
        }

        public string GetResourceUrl(int resourceId, string code) {
            // 首先判断提取码是否正确
            string systemCode = GetCode();
            // 提取码不正确
            if(!code.Equals(systemCode)) {
                return "error";
            }
            // 获取资源
            var sqlResult = dbContext.Resources
                .Where(item => item.ResourceId == resourceId && item.Delete_Sign == false)
                .SingleOrDefault();
            if (sqlResult == null) {
                // 资源不存在
                return "404";
            }
            string url = sqlResult.Url;
            sqlResult.Sum += 1;
            dbContext.SaveChanges();
            return url;
        }

        public bool ShowOrNot(int resourceId, bool delete_sign) {
           var sqlResult =  dbContext.Resources
                .Where(item => item.ResourceId == resourceId)
                .SingleOrDefault();
            if(sqlResult == null) {
                return false;
            }
            sqlResult.Delete_Sign = delete_sign;
            int count =  dbContext.SaveChanges();
            if(count != 0) {
                return true;
            }
            return false;
        }

       

        public async Task<string> UploadImage(IFormFile file) {
           
            string url = "";
            // 获取文件后缀
            string suffix = Path.GetExtension(file.FileName);
            if (file.Length >= 5 * 1024 * 1024) {
                return "图片大小超出限制";
            }
            // 将文件保存
            try {
                string fileName = Guid.NewGuid().ToString();
                fileName = fileName + suffix;
                string saveName = savePath + fileName;
                using (var stream = new FileStream(saveName, FileMode.Create)) {
                    await file.CopyToAsync(stream);
                }
                url = accessURL + fileName;
                return url;
            }
            catch (Exception ex) {
                Console.WriteLine(ex.ToString());
                return "";
            }
        }

        public async Task<string> UploadResource(IFormFile file) {
            // 获取文件后缀
            string suffix = Path.GetExtension(file.FileName);
            // 将文件保存
            try {
                string fileName = Guid.NewGuid().ToString();
                fileName = fileName + suffix;
                string saveName = savePath + fileName;
                using (var stream = new FileStream(saveName, FileMode.Create)) {
                    await file.CopyToAsync(stream);
                }
                return accessURL + fileName;
            }
            catch (Exception ex) {
                Console.WriteLine(ex.ToString());
                return "";
            }
        }


    }
}
