using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EnglishStudy.Entity {
    // 消息接收表，不添加和消息表的外键关系，不想弄得太复杂
    [Table("receive"),Comment("消息接收表")]
    public class Receive {

        [Key,Column("receive_id"),Comment("主键Id")]
        public int ReceiveId { get; set; }

        [Column("message_id"),Comment("消息id"),Required]
        public int MessageId { get; set; }

        [Column("user_id"),Comment("用户id"),Required]
        public int UserId { get; set; }

        [Column("read"),Comment("消息是否已读"),DefaultValue(0)]
        public int Read { get; set; }

        [Column("time",TypeName ="timestamp"),Comment("消息读取时间")]
        public DateTime Time { get; set; }
      
    }
}
