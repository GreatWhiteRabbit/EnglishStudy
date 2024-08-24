using EnglishStudy.DTO.MyMessage;
using EnglishStudy.Entity;

namespace EnglishStudy.Service {
    public interface IMessageService {

        /// <summary>
        /// 发布消息服务
        /// </summary>
        /// <param name="title">消息标题</param>
        /// <param name="content">消息内容</param>
        /// <returns>true表示发布成功</returns>
        public bool PublishMessage(string title,string content);

        /// <summary>
        /// 从消息表中拉取数据到消息接收表中
        /// </summary>
        public Task<bool> PullMessage();

        /// <summary>
        /// 管理员获取所有的消息
        /// </summary>
        /// <param name="page">页面</param>
        /// <param name="size">页面大小</param>
        /// <returns></returns>
        public List<Message> AdminGetMessageList(int page,int size);

        /// <summary>
        /// 用户获取系统消息
        /// </summary>
        /// <param name="userId"></param>
        public List<MessageDTO> UserGetMessage(int userId,int page,int size);

        /// <summary>
        /// 获取用户的所有未读消息
        /// </summary>
        /// <param name="userId">用户id</param>
        /// <returns>未读消息条数</returns>
        public int UserGetUnreadMessageCount(int userId);

        /// <summary>
        /// 管理员删除系统消息
        /// </summary>
        /// <param name="messageId">系统消息id</param>
        /// <returns>true表示删除成功</returns>
        public Task<bool> DeleteMessage(int messageId);

        /// <summary>
        /// 用户读取系统消息，即将消息设置为已读
        /// </summary>
        /// <param name="UserId">用户Id</param>
        /// <param name="ReceiveId">接收消息id</param>
        /// <returns>true表示已读成功</returns>
        public bool ReadMessage(int UserId, int ReceiveId);

        /// <summary>
        /// 设置定时任务的执行时间
        /// </summary>
        /// <param name="Hour">小时</param>
        /// <param name="Minute">分钟</param>
        /// <returns>true表示设置成功</returns>
        public bool SetJobTime(int Hour,int Minute);


        /// <summary>
        /// 获取定时任务执行的时间
        /// </summary>
        /// <returns>定时任务执行时间</returns>
        public string GetTime();
    }
}
