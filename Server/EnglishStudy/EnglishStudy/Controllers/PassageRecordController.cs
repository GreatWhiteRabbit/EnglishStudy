using EnglishStudy.DTO;
using EnglishStudy.Entity.ChildEntity;
using EnglishStudy.Service;
using EnglishStudy.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EnglishStudy.Controllers {

    [ApiController]
    [Route("passageRecord")]
    public class PassageRecordController {
        private IPassageRecordService passageRecordService;

        private Result result = new Result();

        public PassageRecordController(IPassageRecordService passageRecordService) {
            this.passageRecordService = passageRecordService;
        }
        /// <summary>
        /// 增加记录
        /// </summary>
        /// <param name="UserId">用户id</param>
        /// <param name="PassageId">阅读理解id</param>
        /// <param name="list">答题集合</param>
        /// <returns></returns>
        // 这里的答题集合应该要答完所有的题目，否则前端不允许提交
        // 或者前端设置一个默认的答案
        [Authorize(MyConstant.UserOrAdmin)] // 添加管理员或者用户权限
        [HttpPost("add/{UserId}/{PassageId}")]
        public Result AddRecord(int UserId, int PassageId, [FromBody] AnswerDetailList list) {
            bool success = passageRecordService.AddRecord(UserId, PassageId, list.List);
            if (success) return result.Ok();
            return result.failed(StatusCode.BadRequest, "添加失败");
        }

        /// <summary>
        /// 分页获取数据
        /// </summary>
        /// <param name="UserId">用户id</param>
        /// <param name="page">页码</param>
        /// <param name="size">页面大小</param>
        /// <returns></returns>
        [Authorize(MyConstant.UserOrAdmin)] // 添加管理员或者用户权限
        [HttpGet("{UserId}/{page}/{size}")]
        public Result GetRecordByPageSize(int UserId, int page, int size) {
            var pageResult = passageRecordService.GetRecordByPageSize(UserId, page, size);
            return result.Ok(pageResult);
        }

        /// <summary>
        /// 根据UserId和passageid获取记录
        /// </summary>
        /// <param name="UserId">用户id</param>
        /// <param name="PassageId">阅读理解id</param>
        /// <returns></returns>
        [Authorize(MyConstant.UserOrAdmin)] // 添加管理员或者用户权限
        [HttpGet("record/{UserId}/{PassageId}")]
        public Result GetRecordByPassageId(int UserId, int PassageId) {
            var DTO = passageRecordService.GetRecordByPassageId(UserId, PassageId);
            if (DTO.RecordId == 0) {
                return result.failed(StatusCode.NotFound, "没有记录");
            }
            return result.Ok(DTO);
        }

        /// <summary>
        /// 批量删除数据
        /// </summary>
        /// <param name="UserId">用户id</param>
        /// <param name="idList">id集合</param>
        /// <returns></returns>
        [Authorize(MyConstant.UserOrAdmin)] // 添加管理员或者用户权限
        [HttpPost("delete/{UserId}")]
        public Result DeleteBatch(int UserId, [FromBody] IdList idList) {
           
            int count = passageRecordService.DeleteBatch(UserId, idList.list);
            return result.Ok(count);
        }

        /// <summary>
        /// 更新准确率
        /// </summary>
        /// <param name="UserId">用户id</param>
        /// <param name="PassageId">文章id</param>
        /// <param name="Accuracy">准确率</param>
        /// <returns></returns>
        [Authorize(MyConstant.UserOrAdmin)] // 添加管理员或者用户权限
        [HttpPost("accracy/{UserId}/{PassageId}/{Accuracy}")]
        public Result UpdateAccuracy(int UserId, int PassageId, Double Accuracy) {
            return result.Ok(passageRecordService.UpdateAccuracy(UserId, PassageId, Accuracy));
       
        }

        /// <summary>
        /// 获取近size的准确率
        /// </summary>
        /// <param name="UserId">用户id</param>
        /// <param name="size">size</param>
        /// <returns></returns>
        [Authorize(MyConstant.UserOrAdmin)] // 添加管理员或者用户权限
        [HttpGet("accuracylist/{UserId}/{size}")]
        public Result GetAccuracyList(int UserId, int size = 10) {
            return result.Ok(passageRecordService.GetAccuracyList(UserId, size));
        }
    }
}
