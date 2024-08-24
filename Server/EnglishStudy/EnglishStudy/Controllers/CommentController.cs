using EnglishStudy.Entity;
using EnglishStudy.Service;
using EnglishStudy.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EnglishStudy.Controllers {
    [ApiController]
    [Route("comment")]
    public class CommentController {

        private ICommentService commentService;

        private Result result = new Result();

        public CommentController(ICommentService commentService) {
            this.commentService = commentService;
        }

        // 添加评论
        [HttpPost("add")]
        [Authorize(MyConstant.UserOrAdmin)]
        public Result AddComment([FromBody] Comment comment) {
            int id = commentService.AddComment(comment);
            return result.Ok(id);
        }

        // 删除评论
        [HttpPost("delete/{userId}/{commentId}")]
        [Authorize(MyConstant.UserOrAdmin)]
        public Result DeleteComment(int userId, int commentId) {
            bool success = commentService.DeleteComment(userId, commentId);
            return result.Ok(success);
        }

        // 分页获取一级评论
        [HttpGet("first/{page}/{size}")]
        public Result GetComment(int page = 1, int size = 10) {
            var commentPage = commentService.GetComment(page, size);
            return result.Ok(commentPage);
        }

        /// <summary>
        /// 根据一级评论分页获取二级评论
        /// </summary>
        /// <param name="replyCommentId">一级评论id</param>
        /// <param name="page">page</param>
        /// <param name="size">size</param>
        /// <returns></returns>
        [HttpGet("second/{replyCommentId}/{page}/{size}")]
        public Result GetSecondCommentPage(int replyCommentId, int page = 1, int size = 10) {
            var secondPage = commentService.GetSecondCommentPage(replyCommentId, page, size);
            return result.Ok(secondPage);
        }

        /// <summary>
        /// 获取被回复的内容
        /// </summary>
        /// <param name="userId">被回复的用户id</param>
        /// <param name="commentReciveId">评论接收id</param>
        /// <param name="commentId">评论id</param>
        /// <returns></returns>
        [HttpGet("detail/{userId}/{commentReciveId}/{commentId}")]
        [Authorize(MyConstant.UserOrAdmin)]
        public Result GetReplyCommentDetail(int userId, int commentReciveId, int commentId) {
            var detail = commentService.GetReplyCommentDetail(userId, commentReciveId, commentId);
            return result.Ok(detail);
        }

        /// <summary>
        /// 分页获取被回复的具体内容
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        [HttpGet("detail/page/{userId}/{page}/{size}")]
        [Authorize (MyConstant.UserOrAdmin)]
        public Result GetReplyCommentDetailByPage(int userId, int page = 1, int size = 10) {
           var pageResult = commentService.GetReplyCommentDetailByPage(userId, page, size);
            return result.Ok(pageResult);
        }


        /// <summary>
        /// 修改评论是否置顶
        /// </summary>
        /// <param name="commentId">评论id</param>
        /// <param name="top">true或者false</param>
        /// <returns>true表示修改成功</returns>
        [HttpPost("top/{commentId}/{top}")]
        [Authorize(MyConstant.Admin)]
        public Result SetTopOrNot(int commentId, bool top) {
            var success = commentService.SetTopOrNot(commentId, top);
            return result.Ok(success);
        }

        /// <summary>
        /// 将消息设置为已读
        /// </summary>
        /// <param name="commentReciveId">commentReciveId</param>
        /// <param name="userId">用户id</param>
        /// <returns>true表示修改成功</returns>
        [HttpPost("read/{commentReciveId}/{userId}")]
        [Authorize(MyConstant.UserOrAdmin)]
        public Result SetRead(int commentReciveId, int userId) {
            var success = commentService.SetRead(commentReciveId, userId);
            return result.Ok(success);
        }

        /// <summary>
        /// 将所有消息设置为已读
        /// </summary>
        /// <param name="userId">用户id</param>
        /// <returns>true表示设置成功</returns>
        [HttpPost("read/all/{userId}")]
        [Authorize(MyConstant.UserOrAdmin)]
        public Result SetAllRead(int userId) {
            var success = commentService.SetAllRead(userId);
            return result.Ok(success);
        }

        /// <summary>
        /// 获取用户未读消息条数
        /// </summary>
        /// <param name="userId">用户id</param>
        /// <returns>未读消息条数</returns>
        [HttpGet("count/{userId}")]
        [Authorize(MyConstant.UserOrAdmin)]
        public Result GetUnReadCount(int userId) {
            int count = commentService.GetUnReadCount(userId);
            return result.Ok(count);
        }

        /// <summary>
        /// 获取被回复评论的一级评论
        /// </summary>
        /// <param name="commentId">评论id</param>
        /// <returns></returns>
        [HttpGet("reply/comment/first/{commentId}")]
        [Authorize(MyConstant.UserOrAdmin)]
        public Result GetFirstLeverComment(int commentId) {
            return result.Ok(commentService.GetFirstLeverComment(commentId));
        }

        



    }
}
