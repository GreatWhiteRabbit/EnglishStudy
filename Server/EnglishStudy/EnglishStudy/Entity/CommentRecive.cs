using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace EnglishStudy.Entity {

    [Table("comment_recive"),Comment("评论接收表")]
    public class CommentRecive {

        [Key,Column("comment_recive_id"),Comment("主键，评论回复接收id")]
        public int CommentReciveId { get; set; } //评论回复接收id

        [Column("comment_id"),Comment("评论的id"),Required]
        public int CommentId { get; set; } // 评论id

        [Column("user_id"),Comment("用户id"),Required]
        public int UserId { get; set; } // 被回复的用户

        [Column("read"),Comment("是否已读"),DefaultValue(0)]
        public int Read { get; set; } // 是否已读

        [Column("time",TypeName ="timestamp"),Comment("读取时间")]
        public DateTime Time { get; set; } // 读取时间
    }
}
