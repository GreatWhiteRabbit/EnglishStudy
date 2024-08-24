namespace EnglishStudy.DTO.Comment {

    // 二级评论封装类
    public class SecondCommentDTO {
        public int CommentId { get; set; } // 评论id

        public int ReplyCommentId { get; set; } // 被评论的一级评论id


        public int UserId { get; set; } // 评论的用户

        public int ReplyUserId { get; set; }  // 被评论的用户的id

        public string Image { get; set; } // 用户头像

        public string ReplyImage { get; set; } // 被回复用户的头像
        public string NickName { get; set; } // 用户昵称

        public string ReplyNickName { get; set; } // 被评论的用户昵称

        public int Status { get; set; } // 用户级别

        public string Content { get; set; } // 评论的内容

        public DateTime Time { get; set; } // 评论的时间

       
    }
}
