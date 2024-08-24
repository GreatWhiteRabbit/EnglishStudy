using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EnglishStudy.Entity {
    /// <summary>
    /// 用户记忆单词的详细记录
    /// </summary>
    [Table("word_record"),Comment("记忆单词详细记录")]
    public class WordRecord {

        [Key,Column("record_id"),Comment("记录id")]
        public int RecordId { get; set; }

        [Column("user_id"),Comment("用户id"),Required]
        public int UserId { get; set; }

        [Column("word_type"),Comment("单词类型"),Required]
        public int WordType { get; set; }

        [Column("detail",TypeName ="text"),Comment("详细记录"),Required]
        public string Detail { get; set; }

        [Column("time",TypeName ="timestamp"),Comment("创建时间")]
        public DateTime Time { get; set; }

        [Column("total"),Comment("记忆单词总数")]
        public int Total { get; set; }

        [Column("delete_sign"),Comment("删除标记位"),DefaultValue(0)]
        public int Delete_sign { get; set; }
    }
}
