using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EnglishStudy.Entity {

    // 评论表
    [Table("comment"), Comment("评论表")]
    public class Comment {

        [Key, Column("comment_id"), Comment("主键评论id")]
        public int CommentId { get; set; } // 评论id

        [Column("reply_comment_id"),Comment("被回复的一级评论id"),DefaultValue(0)]
        public int ReplyCommentId { get; set; } // 被回复的一级评论

        [Column("user_id"),Comment("用户id"),Required]
        public int UserId { get; set; } // 发表评论的用户

        [Column("reply_user_id"),Comment("被回复的用户id"),DefaultValue(0)]
        public int ReplyUserId { get; set; } // 被回复的用户

        [Column("content",TypeName ="varchar(500)"),Comment("评论内容"),DefaultValue("原内容已被删除")]
        public string Content { get; set; } = "原内容已被删除"; // 评论的内容

        [Column("time",TypeName ="timestamp"),Comment("评论时间"),Required]
        public DateTime Time { get; set; } // 评论的时间

        [Column("delete_sign"),Comment("删除标记位"),DefaultValue(0)]
        public int DeleteSign { get; set; } // 删除标记位

        [Column("top"),Comment("是否置顶"),DefaultValue(false)]
        public bool Top { get; set; } // 是否置顶


    }
}
