using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EnglishStudy.Entity {
    // 系统消息表

    [Table("message"),Comment("系统消息表")]
    public class Message {
        [Key,Column("message_id"),Comment("主键id")]
        public int MessageId { get; set; }

        [Column("title",TypeName ="varchar(100)"),Comment("消息标题"),Required]
        public string Title { get; set; }

        [Column("content",TypeName ="varchar(400)"),Comment("消息内容"),Required]
        public string content { get; set; }

        [Column("time",TypeName ="timestamp"),Comment("发布时间")]
        public DateTime Time { get; set; }

    }
}
