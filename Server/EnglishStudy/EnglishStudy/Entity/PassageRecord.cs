using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EnglishStudy.Entity {

    [Table("passage_record"),Comment("阅读理解做题记录")]
    public class PassageRecord {

        [Key,Column("record_id"),Comment("记录id")]
        public int RecordId { get; set; }

        [Required,Column("user_id"),Comment("用户id")]
        public int UserId { get; set; }

        [Required,Column("passage_id"),Comment("阅读理解id")]
        public int PassageId { get; set; }

        [Required,Column("detail", TypeName = "text"),Comment("做题详细记录")]
        public string Detail { get; set; }

        [Column("time",TypeName ="timestamp"),Comment("创建时间")]
        public DateTime Time { get; set; }

        [Column("delete_sign"),Comment("删除标记位"),DefaultValue(0)]
        public int DeleteSign { get; set; }

        [Column("accuracy"),Comment("准确度"),DefaultValue(0)]
        public Double Accuracy { get; set; }
    }
}
