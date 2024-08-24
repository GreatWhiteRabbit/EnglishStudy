namespace EnglishStudy.DTO.Comment {

    // 被回复的内容
    public class ReplyCommentDetail {
        public int CommentId { get; set; } // 评论id

        public int ReplyCommentId { get; set; } // 被评论的一级评论id

        public int CommentReciveId { get; set; } // 接收id

        public int UserId { get; set; } // 评论的用户

        
        public string Image { get; set; } // 用户头像

     
        public string NickName { get; set; } // 用户昵称


        public string Content { get; set; } // 评论的内容

        public DateTime Time { get; set; } // 评论的时间

    }
}
