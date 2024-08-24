
using EnglishStudy.Entity;
using EnglishStudy.Service;
using EnglishStudy.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace EnglishStudy.Controllers {

    [ApiController]
    [Route("resource")]
    public class ResourceController  {

       private IMyResourceService myResourceService;

        private Result result = new Result();

       public ResourceController(IMyResourceService myResourceService) {
        this.myResourceService = myResourceService;
        }

        /// <summary>
        /// 添加资源
        /// </summary>
        /// <param name="resource">资源对象</param>
        /// <returns></returns>
        [HttpPost("add")]
        [Authorize(MyConstant.Admin)]
        public Result AddResource([FromBody]Resource resource) {
           bool success =  myResourceService.AddResource(resource);
            return result.Ok(success);
        }

        /// <summary>
        /// 上传资源封面
        /// </summary>
        /// <param name="file">图片</param>
        /// <returns>url</returns>
        [HttpPost("upload/image")]
        [Authorize(MyConstant.Admin)]
        public async Task<Result> UploadImage(IFormFile file) {
            string url = await myResourceService.UploadImage(file);
            return result.Ok(url);
        }


        /// <summary>
        /// 上传资源接口
        /// </summary>
        /// <param name="file">资源文件</param>
        /// <returns>url</returns>
        [HttpPost("upload/resource")]
        [Authorize(MyConstant.Admin)]
        [RequestSizeLimit(1024*1024*1024)]
        public async Task<Result> UploadResource(IFormFile file) {
           
            string url = await myResourceService.UploadResource(file);
            return result.Ok(url);
        }

        /// <summary>
        /// 管理员分页获取资源
        /// </summary>
        /// <param name="page">page</param>
        /// <param name="size">size</param>
        /// <returns></returns>
        [HttpGet("admin/{page}/{size}")]
        [Authorize(MyConstant.Admin)]
        public Result AdminGetResource(int page = 1, int size = 10) {
            var pageDTO = myResourceService.AdminGetResource(page, size);
            return result.Ok(pageDTO);
        }

        /// <summary>
        /// 用户获取分页资源
        /// </summary>
        /// <param name="page">page</param>
        /// <param name="size">size</param>
        /// <returns></returns>
        [HttpGet("{page}/{size}")]
        public Result GetResource(int page = 1, int size = 10) {
            var pageDTO = myResourceService.GetResource(page, size);
            return result.Ok(pageDTO);
        }

        /// <summary>
        /// 修改提取码
        /// </summary>
        /// <param name="code">提取码</param>
        /// <returns></returns>
        [HttpPost("update/{code}")]
        [Authorize(MyConstant.Admin)]
        public Result UpdateCode(string code) {
            bool success = myResourceService.UpdateCode(code);
            return result.Ok(success);
        }

        /// <summary>
        /// 获取提取码
        /// </summary>
        /// <returns></returns>
        [HttpGet("code")]
        [Authorize(MyConstant.Admin)]
        public Result GetCode() {
            var code = myResourceService.GetCode();
            return result.Ok(code);
        }

        /// <summary>
        /// 修改资源是否显示
        /// </summary>
        /// <param name="resourceId">资源id</param>
        /// <param name="delete_sign">显示</param>
        /// <returns></returns>
        [HttpPost("show/{resourceId}/{delete_sign}")]
        [Authorize(MyConstant.Admin)]
        public Result ShowOrNot(int resourceId, bool delete_sign) {
            var success = myResourceService.ShowOrNot(resourceId, delete_sign);
            return result.Ok(success);
        }

        /// <summary>
        /// 删除资源
        /// </summary>
        /// <param name="resourceId">资源id</param>
        /// <returns></returns>
        [HttpPost("delete/{resourceId}")]
        [Authorize(MyConstant.Admin)]
        public Result DeleteResource(int resourceId) {
            var success = myResourceService.DeleteResource(resourceId);
            return result.Ok(success);
        }

        /// <summary>
        /// 获取资源URL
        /// </summary>
        /// <param name="resourceId">资源id</param>
        /// <param name="code">提取码</param>
        /// <returns></returns>
        [HttpGet("url/{resourceId}/{code}")]
        [Authorize(MyConstant.UserOrAdmin)]
        public Result GetResourceUrl(int resourceId, string code) {
            var url = myResourceService.GetResourceUrl(resourceId, code);
            return result.Ok(url);
        }

        /// <summary>
        /// 根据关键字获取资源
        /// </summary>
        /// <param name="keyword">关键字</param>
        /// <param name="page">page</param>
        /// <param name="size">size</param>
        /// <returns></returns>
        [HttpGet("search/{keyword}/{page}/{size}")]
        public Result SearchResource(string keyword, int page = 1, int size = 10) {
           var pageDTO =  myResourceService.SearchResource(keyword, page, size);
            return result.Ok(pageDTO);
        }

        /// <summary>
        /// 根据url下载文件
        /// </summary>
        /// <param name="url">url</param>
        /// <returns></returns>
        [HttpPost("download/{url}")]
        [Authorize(MyConstant.UserOrAdmin)]
        public FileContentResult DownloadFile(string url) {
              string savePath = @"D:\nginx\download\";
            /* string accessURL = @"https://localhost:7031/resouce/download/";
           // 获取文件名
           string fileName = url.Split(accessURL)[1];*/
            string fileName = url;
            // 获取文件路径
            string file = savePath + fileName;
            byte[] fileBytes = File.ReadAllBytes(file);
           
            return  new FileContentResult(fileBytes, "application/octet-stream")
            { 
                FileDownloadName = fileName,
            };
        }
}
}
