using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EnglishStudy.Entity {
    /// <summary>
    /// 用于记录用户记忆单词的位置，如四级单词第一百个单词
    /// </summary>

    [Table("word_position"),Comment("单词记忆位置")]
    public class WordPosition {

        [Key,Column("position_id"),Comment("主键")]
        public int PositionId { get; set; }

        [Column("user_id"),Comment("用户id"),Required]
        public int UserId { get; set; }

        [Column("lastword_id"),Comment("上一次最后记忆的单词id"),Required]
        public int LastWordId { get; set; } // 上一次最后记忆的单词id

        [Column("type"),Comment("单词类型"),Required]
        public int Type { get; set; }
        [Column("delete_sign"),Comment("删除标记位"),DefaultValue(0)]
        public int DeleteSign { get; set; }
    }
}
