using EnglishStudy.DTO;
using EnglishStudy.Entity;
using EnglishStudy.Service;
using EnglishStudy.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EnglishStudy.Controllers {
    [ApiController]
    [Route("everyday")]
    public class EveryDayEnglishController {
        private IEveryDayEnglish everyDayEnglish;

        private Result result = new Result();

        public EveryDayEnglishController(IEveryDayEnglish everyDayEnglish) {
            this.everyDayEnglish = everyDayEnglish;
        }

        /// <summary>
        /// 上传excel文件
        /// </summary>
        /// <param name="file">文件</param>
        /// <returns></returns>
        [Authorize(MyConstant.Admin)] // 只有管理员才有权限
        [HttpPost("upload")]
        public async Task<Result> UploadFile(IFormFile file) {
            var content = await everyDayEnglish.UploadFile(file);
            return result.Ok(content);
        }

        /// <summary>
        /// 通过id获取每日英语
        /// </summary>
        /// <param name="id">每日英语id</param>
        /// <returns></returns>
        // 不需要添加权限
        [HttpGet("{id}")]
        public Result GetById(int id) {
            EveryDayEnglish everyDay = everyDayEnglish.GetById(id);
            if(everyDay == null) {
                return result.failed(StatusCode.NotFound, "找不到啦");
            }
            return result.Ok(everyDay);
        }

        /// <summary>
        /// 修改每日英语
        /// </summary>
        /// <param name="everyDay">修改后的每日英语内容</param>
        /// <returns></returns>
        [Authorize(MyConstant.Admin)] // 添加管理员权限
        [HttpPost("update")]
        public Result Update([FromBody] EveryDayEnglish everyDay) {
            bool success = everyDayEnglish.Update(everyDay);
            if (success) return result.Ok();
            return result.failed(StatusCode.ServerError, "服务器出错啦");
        }

        /// <summary>
        /// 根据页码和页面大小获取数据
        /// </summary>
        /// <param name="page">页码</param>
        /// <param name="size">页码大小</param>
        /// <returns></returns>
        [HttpGet("{page}/{size}")]
        public Result GetByPageSize(int page, int size) {
            var resultPage = everyDayEnglish.GetByPageSize(page, size);
            return result.Ok(resultPage);
        }

        /// <summary>
        /// 管理员根据页码和页面大小获取数据
        /// </summary>
        /// <param name="page">页码</param>
        /// <param name="size">页码大小</param>
        /// <param name="delete">0表示未删除</param>
        /// <returns></returns>
        [Authorize(MyConstant.Admin)]
        [HttpGet("admin/{page}/{size}/{delete}")]
        public Result AdminGetByPageSize(int page, int size,int delete) {
            var resultPage = everyDayEnglish.AdminGetByPageSize(page, size, delete);
            return result.Ok(resultPage);
        }

        /// <summary>
        /// 批量删除每日英语
        /// </summary>
        /// <param name="list">id的集合</param>
        /// <returns></returns>
        [Authorize(MyConstant.Admin)] // 添加管理员权限
        [HttpPost("delete")]
        public Result Delete([FromBody] IdList list) {
            int count = everyDayEnglish.Delete(list.list);
            return result.Ok(count);
        }

        /// <summary>
        /// 批量恢复数据
        /// </summary>
        /// <param name="list">id集合</param>
        /// <returns></returns>
        [Authorize(MyConstant.Admin)] // 添加管理员权限
        [HttpPost("recover")]
        public Result Recover([FromBody] IdList list) {
            int count = everyDayEnglish.Recover(list.list);
            return result.Ok(count);
        }

    }
}
