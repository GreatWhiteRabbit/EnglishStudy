using EnglishStudy.DTO;
using EnglishStudy.Entity;
using EnglishStudy.Service;
using EnglishStudy.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EnglishStudy.Controllers {

    [Route("passage")]
    [ApiController]
    public class PassageController {

        private Result result = new Result();

        private IPassageService passageService;

        public PassageController(IPassageService passageService) {
            this.passageService = passageService;
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="file">题库file</param>
        /// <returns></returns>
        [Authorize(MyConstant.Admin)] // 添加管理员权限
        [HttpPost("upload/{title}")]
        public async Task<Result> AddFile(IFormFile file, string title) {
            bool success = await passageService.AddFile(file, title);
            if(success) {
                return result.Ok();
            }
            return result.failed(StatusCode.BadRequest, "上传失败");
        }

        /// <summary>
        /// 通过id获取阅读理解内容和题目
        /// </summary>
        /// <param name="id">文章的id</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public Result GetPassageById(int id) {
            var passage = passageService.GetPassageById(id);
            if(passage.Content == null) {
                return result.failed(StatusCode.NotFound, "文章丢失啦");
            }
            return result.Ok(passage);
        }

        /// <summary>
        /// 根据id形式上删除阅读理解id
        /// </summary>
        /// <param name="id">id</param>
        /// <returns></returns>
        [Authorize(MyConstant.Admin)] // 添加管理员权限
        [HttpPost("delete/{id}")]
        public Result DeletePassageById(int id) {
            var delete = passageService.DeletePassageById(id);
            if (delete) return result.Ok();
            return result.failed(StatusCode.BadRequest, "删除失败");
        }

        /// <summary>
        /// 修改阅读理解的内容
        /// </summary>
        /// <param name="passageDTO">passageDTO类</param>
        /// <returns></returns>
        [Authorize(MyConstant.Admin)] // 添加管理员权限

        [HttpPost("update")]
        public Result UpdatePassage([FromBody] PassageDTO passageDTO) {
            var update = passageService.UpdatePassage(passageDTO);
            if (update) return result.Ok();
            return result.failed(StatusCode.BadRequest, "修改失败");
        }

        /// <summary>
        /// 批量添加阅读理解
        /// </summary>
        /// <param name="passageList">阅读理解集合</param>
        /// <returns></returns>
        [Authorize(MyConstant.Admin)] // 添加管理员权限

        [HttpPost("addlist")]
        // 参数中的questionList设置为[]比较好
        public Result AddPassageList([FromBody] MyPassageList passageList) {
            int count = passageService.AddPassageList(passageList.passageList);
            return result.Ok(count);
        }

        /// <summary>
        /// 恢复阅读理解文章
        /// </summary>
        /// <param name="id">id</param>
        /// <returns></returns>
        [Authorize(MyConstant.Admin)] // 添加管理员权限
        [HttpPost("recover/{id}")]
        public Result RecoverPassageById(int id) {
            var recover = passageService.RecoverPassageById(id);
            if(recover) return result.Ok();
            return result.failed(StatusCode.ServerError, "恢复失败");
        }




        /// <summary>
        /// 根据页码和页面大小获取passage
        /// </summary>
        /// <param name="page">页码</param>
        /// <param name="size">页面大小</param>
        /// <returns></returns>
        [HttpGet("{page}/{size}")]
        public Result GetPassageByPageSize(int page = 1,int size = 10) {
            var passge = passageService.GetPassageByPageSize(page, size);
            return result.Ok(passge);
        }

        /// <summary>
        /// 管理员根据页码和页面大小获取passage
        /// </summary>
        /// <param name="page">页码</param>
        /// <param name="size">页面大小</param>
        /// <param name="delete">0表示未删除</param>
        /// <returns></returns>
        [HttpGet("admin/{page}/{size}/{delete}")]
        [Authorize(MyConstant.Admin)]
        public Result AdminGetPassageByPageSize(int page = 1, int size = 10,int delete = 0) {
            var passge = passageService.AdminGetPassageByPageSize(page, size,delete);
            return result.Ok(passge);
        }
    }
}
