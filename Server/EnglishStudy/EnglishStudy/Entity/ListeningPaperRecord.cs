using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace EnglishStudy.Entity {
    [Table("listeningpaper_record"),Comment("听力试题记录")]
    public class ListeningPaperRecord {
        [Key, Column("record_id"), Comment("记录id")]
        public int RecordId { get; set; }

        [Required, Column("user_id"), Comment("用户id")]
        public int UserId { get; set; }

        [Required, Column("listeningpaper_id"), Comment("听力试题id")]
        public int ListeningPaperId { get; set; }

        [Required, Column("detail", TypeName = "text"), Comment("做题详细记录")]
        public string Detail { get; set; }

        [Column("time", TypeName = "timestamp"), Comment("创建时间")]
        public DateTime Time { get; set; }

        [Column("delete_sign"), Comment("删除标记位"), DefaultValue(0)]
        public int DeleteSign { get; set; }

        [Column("accuracy"), Comment("准确度"), DefaultValue(0)]
        public Double Accuracy { get; set; }
    }
}
