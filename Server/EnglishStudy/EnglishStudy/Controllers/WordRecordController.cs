using EnglishStudy.Entity.ChildEntity;
using EnglishStudy.Service;
using EnglishStudy.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EnglishStudy.Controllers {

    [ApiController]
    [Route("wordRecord")]
    public class WordRecordController {
        private IWordRecordService wordRecordService;

        private Result result = new Result();

        public WordRecordController(IWordRecordService wordRecordService) {
            this.wordRecordService = wordRecordService;
        }

        /// <summary>
        /// 根据用户id和单词类型获取要记忆的单词
        /// </summary>
        /// <param name="UserId">用户id</param>
        /// <param name="type">单词类型</param>
        /// <param name="size">单词数量</param>
        /// <returns></returns>
        [Authorize(MyConstant.UserOrAdmin)] // 添加管理员或者用户权限
        [HttpGet("{UserId}/{type}/{size}")]
        public Result GetWordList(int UserId, int type, int size = 20) {
            var List = wordRecordService.GetWordList(UserId, type, size);
            return result.Ok(List);
        }

        /// <summary>
        /// 添加记忆单词记录
        /// </summary>
        /// <param name="UserId">用户id</param>
        /// <param name="Type">单词类型</param>
        /// <param name="LastId">最后一个单词的id</param>
        /// <param name="wordRecordDetail">单词集合</param>
        /// <returns></returns>
        [Authorize(MyConstant.UserOrAdmin)] // 添加管理员或者用户权限
        [HttpPost("add/{UserId}/{Type}/{LastId}")]
        public Result AddRecord(int UserId, int Type, int LastId, [FromBody] WordRecordDetail wordRecordDetail) {
            int id = wordRecordService.AddRecord(UserId, Type, LastId, wordRecordDetail);
            return result.Ok(id);
        }


        /// <summary>
        /// 获取近size次total
        /// </summary>
        /// <param name="UserId">用户id</param>
        /// <param name="Type">单词类型</param>
        /// <param name="size">size</param>
        /// <returns></returns>
        [Authorize(MyConstant.UserOrAdmin)] // 添加管理员或者用户权限
        [HttpGet("total/{UserId}/{Type}/{size}")]
        public Result GetTotalList(int UserId, int Type, int size = 10) {
            return result.Ok( wordRecordService.GetTotalList(UserId, Type, size));
        }

        /// <summary>
        /// 获取最近一次单词记忆记录
        /// </summary>
        /// <param name="UserId">用户id</param>
        /// <param name="type">单词类型</param>
        /// <returns></returns>
        [Authorize(MyConstant.UserOrAdmin)] // 添加管理员或者用户权限
        [HttpGet("last/{UserId}/{type}")]
        public Result GetLastRecord(int UserId, int type) {
            return result.Ok(wordRecordService.GetLastRecord(UserId, type));
        }

        /// <summary>
        /// 重置单词记忆情况
        /// </summary>
        /// <param name="UserId">用户id</param>
        /// <param name="type">单词类型</param>
        /// <returns></returns>
        [Authorize(MyConstant.UserOrAdmin)] // 添加管理员或者用户权限
        [HttpPost("update/{UserId}/{type}")]
        public Result UpdateLastId(int UserId, int type) {
            var success = wordRecordService.UpdateLastId(UserId, type);
            return result.Ok(success);
        }

        /// <summary>
        /// 获取所有记忆的单词
        /// </summary>
        /// <param name="UserId">用户id</param>
        /// <param name="type">单词类型</param>
        /// <param name="page">当前页</param>
        /// <param name="size">页面大小</param>
        /// <returns></returns>
        [HttpGet("history/{UserId}/{type}/{page}/{size}")]
        [Authorize(MyConstant.UserOrAdmin)]
        public Result getAllWord(int UserId, int type, int page, int size) {
           var wordPage =  wordRecordService.getAllWord(UserId, type, page, size);
            return result.Ok(wordPage);
        }
    }
}
