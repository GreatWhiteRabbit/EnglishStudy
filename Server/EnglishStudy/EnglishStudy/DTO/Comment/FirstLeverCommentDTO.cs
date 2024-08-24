namespace EnglishStudy.DTO.Comment {

    // 一级评论封装类
    public class FirstLeverCommentDTO {
        public int CommentId { get; set; } // 评论id

        public int UserId { get; set; } // 评论的用户

        public string NickName { get; set; } // 用户昵称

        public string Image { get; set; } // 用户头像
            
        public int Status { get; set; } // 用户级别

        public string Content { get; set; } // 评论的内容

        public DateTime Time { get; set; } // 评论的时间

        public bool Top { get; set; } // 是否置顶

        public SecondCommentPage SecondCommentPage { get; set; } = new SecondCommentPage();

    }
}
