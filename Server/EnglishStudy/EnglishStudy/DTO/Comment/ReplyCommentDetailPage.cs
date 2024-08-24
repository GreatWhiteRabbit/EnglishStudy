namespace EnglishStudy.DTO.Comment {
    // 分页获取用户被回复的消息
    public class ReplyCommentDetailPage {
        
        public int Total { get; set; }
        
        public List<ReplyCommentDetail> ReplyComments { get; set; } = new List<ReplyCommentDetail>();
    }
}
