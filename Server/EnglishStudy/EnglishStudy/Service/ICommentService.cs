using EnglishStudy.DTO.Comment;
using EnglishStudy.Entity;

namespace EnglishStudy.Service {
    public interface ICommentService {

        /// <summary>
        /// 添加评论服务
        /// </summary>
        /// <param name="comment">评论类</param>
        /// <returns>非0表示添加成功</returns>
        public int AddComment(Comment comment);

        /// <summary>
        /// 删除评论服务
        /// </summary>
        /// <param name="userId">用户id</param>
        /// <param name="commentId">评论id</param>
        /// <returns>true表示删除成功</returns>
        public bool DeleteComment(int userId, int commentId);

        /// <summary>
        /// 分页获取评论包括一级评论和二级评论，
        /// 主要是一级评论,二级评论默认获取第一页的size个数据
        /// </summary>
        /// <param name="page">page</param>
        /// <param name="size">size</param>
        /// <returns></returns>
        public FirstCommentPage GetComment(int page, int size);

        /// <summary>
        /// 分页获取二级评论
        /// </summary>
        /// <param name="replyCommentId">一级评论的id</param>
        /// <param name="page">page</param>
        /// <param name="size">size</param>
        /// <returns></returns>
        public SecondCommentPage GetSecondCommentPage(int replyCommentId, int page, int size);

        /// <summary>
        /// 获取用户被回复的具体内容
        /// </summary>
        /// <param name="userId">用户id</param>
        /// <param name="commentReciveId">评论接收id</param>
        /// <param name="commentId">评论id</param>
        /// <returns></returns>
        public ReplyCommentDetail GetReplyCommentDetail(int userId, int commentReciveId, int commentId);

        /// <summary>
        /// 分页获取用户被回复的内容
        /// </summary>
        /// <param name="userId">用户</param>
        /// <param name="page">页</param>
        /// <param name="size">页大小</param>
        /// <returns></returns>
        public ReplyCommentDetailPage GetReplyCommentDetailByPage(int userId, int page = 1, int size = 10);

        /// <summary>
        /// 获取一级评论的条数
        /// </summary>
        /// <returns>total</returns>
        public int FirstCommentTotal();

        /// <summary>
        /// 获取一级评论下面二级评论的条数
        /// </summary>
        /// <param name="replyCommentId">一级评论的id</param>
        /// <returns>total</returns>
        public int SecondCommentTotal(int replyCommentId);

        /// <summary>
        /// 取消或者设置评论置顶
        /// </summary>
        /// <param name="commentId">评论id</param>
        /// <param name="top">top的值为true或者false</param>
        /// <returns>true表示设置成功</returns>
        public bool SetTopOrNot(int commentId, bool top);

        /// <summary>
        /// 将被回复的消息设置为已读
        /// </summary>
        /// <param name="commentReciveId">接收的消息</param>
        /// <param name="userId">接收的用户</param>
        /// <returns>true表示设置成功</returns>
        public bool SetRead(int commentReciveId, int userId);

        /// <summary>
        /// 将用户所有被回复的消息设置为已读
        /// </summary>
        /// <param name="userId">用户id</param>
        /// <returns>true表示设置成功</returns>
        public bool SetAllRead(int userId);

        /// <summary>
        /// 获取用户未读评论
        /// </summary>
        /// <param name="userId">用户id</param>
        /// <returns>未读评论条数</returns>
        public int GetUnReadCount(int userId);


        /// <summary>
        /// 根据commentId获取评论内容
        /// </summary>
        /// <param name="commentId">commentId</param>
        /// <returns>评论内容</returns>
        public FirstLeverCommentDTO GetFirstLeverComment(int commentId);

       
    }
}
