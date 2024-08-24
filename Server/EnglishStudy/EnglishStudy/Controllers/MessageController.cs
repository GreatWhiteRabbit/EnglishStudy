using EnglishStudy.DTO.MyMessage;
using EnglishStudy.Service;
using EnglishStudy.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EnglishStudy.Controllers {

    [ApiController]
    [Route("sys_msg")]
    public class MessageController {

        private Result result = new Result();

        private IMessageService messageService;
        public MessageController(IMessageService messageService) {
        this.messageService = messageService;
        }

        /// <summary>
        /// 发布系统消息接口
        /// </summary>
        /// <param name="title">标题</param>
        /// <param name="content">内容</param>
        /// <returns></returns>

        [HttpPost("publish")]
        [Authorize(MyConstant.Admin)]
        public Result PublishMessage([FromBody] MessageDTO messageDTO)  {
            var success = messageService.PublishMessage(messageDTO.Title, messageDTO.Content);
            return result.Ok(success);
        }

        /// <summary>
        /// 从消息表中拉取数据到消息接收表中
        /// </summary>
        /// <returns></returns>
        // TODO 管理员手动拉取或者系统定时拉取
        [HttpPost("pull")]
        [Authorize(MyConstant.Admin)]
        public async Task<Result> PullMessage() {
            var success = await messageService.PullMessage();
            return result.Ok();
        }

        /// <summary>
        /// 管理员获取系统消息
        /// </summary>
        /// <param name="page">page</param>
        /// <param name="size">size</param>
        /// <returns></returns>
        [HttpGet("list/{page}/{size}")]
        [Authorize(MyConstant.Admin)]
        public Result AdminGetMessageList(int page = 1, int size = 20) {
            var list = messageService.AdminGetMessageList(page, size);
            return result.Ok(list);
        }

        /// <summary>
        /// 用户获取系统消息
        /// </summary>
        /// <param name="userId">用户id</param>
        /// <param name="page">page</param>
        /// <param name="size">size</param>
        /// <returns></returns>
        // TODO 需要添加管理员或者普通用户权限
        [HttpGet("notify/{userId}/{page}/{size}")]
        [Authorize(MyConstant.UserOrAdmin)]
        public Result UserGetMessage(int userId, int page, int size) {
            var list = messageService.UserGetMessage(userId, page, size);
            return result.Ok(list);
        }

        /// <summary>
        /// 获取用户未读消息条数
        /// </summary>
        /// <param name="userId">用户id</param>
        /// <returns></returns>
        // TODO 这里只是测试，后续需要使用到signalR进行通信
        [HttpGet("count/{userId}")]
        [Authorize(MyConstant.UserOrAdmin)]
        public Result UserGetUnreadMessageCount(int userId) {
            int count = messageService.UserGetUnreadMessageCount(userId);
            return result.Ok(count);
        }

        /// <summary>
        /// 管理员删除系统消息
        /// </summary>
        /// <param name="messageId">消息id</param>
        /// <returns></returns>
        [HttpPost("delete/{messageId}")]
        [Authorize(MyConstant.Admin)]
        public async Task<Result> DeleteMessage(int messageId) {
            var success = await messageService.DeleteMessage(messageId);
            return result.Ok(success);  
        }

        /// <summary>
        /// 用户将系统消息设置为已读
        /// </summary>
        /// <param name="UserId">用户id</param>
        /// <param name="ReceiveId">消息接收id</param>
        /// <returns></returns>
        // TODO 需要添加管理员或者用户权限
        [HttpPost("read/{UserId}/{ReceiveId}")]
        [Authorize(MyConstant.UserOrAdmin)]
        public Result ReadMessage(int UserId, int ReceiveId) {
            var success = messageService.ReadMessage(UserId, ReceiveId);
            return result.Ok(success);
        }

        /// <summary>
        /// 设置定时任务的执行时间
        /// </summary>
        /// <param name="Hour">小时</param>
        /// <param name="Minute">分钟</param>
        /// <returns></returns>
        [HttpPost("time/{Hour}/{Minute}")]
        [Authorize(MyConstant.Admin)]
        public Result SetJobTime(int Hour, int Minute) {
            var success = messageService.SetJobTime(Hour, Minute);
            return result.Ok(success);
        }


        [HttpGet("time")]
        [Authorize(MyConstant.Admin)]
        public Result GetTime() {
            var time = messageService.GetTime();
            return result.Ok(time);
        }

    }
}
