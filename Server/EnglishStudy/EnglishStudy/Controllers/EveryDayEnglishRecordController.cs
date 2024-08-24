using EnglishStudy.DTO;
using EnglishStudy.Service;
using EnglishStudy.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EnglishStudy.Controllers {
    [ApiController]
    [Route("everydayRecord")]
    public class EveryDayEnglishRecordController {
        private IEveryDayEnglishRecord EveryDayEnglishRecord;
        private Result result = new Result();

        public EveryDayEnglishRecordController(IEveryDayEnglishRecord everyDayEnglishRecord) {
            this.EveryDayEnglishRecord = everyDayEnglishRecord;
        }

        /// <summary>
        /// 批量删除阅读记录
        /// </summary>
        /// <param name="UserId">用户id</param>
        /// <param name="idList">记录id</param>
        /// <returns></returns>
        [Authorize(MyConstant.UserOrAdmin)] // 添加管理员或者用户权限
        [HttpPost("delete/{UserId}")]
        public Result Delete(int UserId, [FromBody] IdList idList) {
            int count = EveryDayEnglishRecord.Delete(UserId, idList.list);
            return result.Ok(count);
        }

        /// <summary>
        /// 添加每日阅读记录
        /// </summary>
        /// <param name="UserId">用户id</param>
        /// <param name="EverydayId">每日英语id</param>
        /// <returns></returns>
        [Authorize(MyConstant.UserOrAdmin)] // 添加管理员或者用户权限
        [HttpPost("{UserId}/{EverydayId}")]
        public Result Add(int UserId, int EverydayId) {
            bool success = EveryDayEnglishRecord.Add(UserId, EverydayId);
            return result.Ok(success);
        }

        /// <summary>
        /// 分页获取每日英语阅读记录
        /// </summary>
        /// <param name="UserId">用户id</param>
        /// <param name="page">页码</param>
        /// <param name="size">页面大小</param>
        /// <returns></returns>
        [Authorize(MyConstant.UserOrAdmin)] // 添加管理员或者用户权限
        [HttpGet("{UserId}/{page}/{size}")]
        public Result GetByPageSize(int UserId, int page = 1, int size = 10) {
            var pageResult = EveryDayEnglishRecord.GetByPageSize(UserId, page, size);
            return result.Ok(pageResult);
        }

        /// <summary>
        /// 更新阅读时间
        /// </summary>
        /// <param name="RecordId">记录id</param>
        /// <returns></returns>
        [Authorize(MyConstant.UserOrAdmin)] // 添加管理员或者用户权限
        [HttpPost("update/{UserId}/{RecordId}")]
        public Result UpdateTime(int UserId,int RecordId) {
            var update = EveryDayEnglishRecord.UpdateTime(UserId,RecordId);
            return result.Ok(update);
        }

        /// <summary>
        /// 根据用户id和每日英语id获取记录id
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="EverydayId"></param>
        /// <returns></returns>
        [HttpGet("record/{UserId}/{EverydayId}")]
        [Authorize(MyConstant.UserOrAdmin)]
        public Result  getRecordIdByUserIdAndEverydayId(int UserId, int EverydayId) {
           return result.Ok( EveryDayEnglishRecord.getRecordIdByUserIdAndEverydayId(UserId, EverydayId));
        }

    }
}
