using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EnglishStudy.Entity {

    [Table("en_record"),Comment("每日英语阅读记录")]
    public class EveryDayEnglishRecord {

        [Key,Comment("每日阅读记录id"),Column("record_id")]
        public int RecordId { get; set; }

        [Required,Comment("用户id"),Column("user_id")]
        public int UserId { get; set; }

        [Required,Comment("每日英语id"),Column("everyday_id")]
        public int EverydayId { get; set; }

        [Comment("阅读时间"),Column("time",TypeName = "timestamp")]
        public DateTime Time { get; set; }

        [Comment("删除标记位"),Column("delete_sign"),DefaultValue(0)]
        public int DeleteSign { get; set; }
    }
}
