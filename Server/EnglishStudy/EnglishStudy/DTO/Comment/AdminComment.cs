namespace EnglishStudy.DTO.Comment {
    // 管理员获取评论封装类
    public class AdminComment {

        public int CommentId { get; set; } // 评论id

        public int ReplyCommentId { get; set; } // 被评论的一级评论id
            
        public int UserId { get; set; } // 评论的用户

        public int ReplyUserId { get; set; }  // 被评论的用户的id

     
        public string NickName { get; set; } // 用户昵称

        public string ReplyNickName { get; set; } // 被评论的用户昵称


        public string Content { get; set; } // 评论的内容

        public DateTime Time { get; set; } // 评论的时间

        public bool Top { get; set; } // 是否置顶
    }
}
