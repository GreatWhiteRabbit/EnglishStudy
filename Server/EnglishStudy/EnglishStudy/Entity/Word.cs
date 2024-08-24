using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EnglishStudy.Entity {
    [Table("words")]
    public class Word {

        [Key,Column("word_id"),Comment("主键id")]
        public int WordId {  get; set; }

        [StringLength(50), Column("word"), Comment("英语单词"), Required]
        public string Words { get; set; } 

        [StringLength(70), Column("phonetic"), Comment("音标"), Required]
        public string Phonetic { get; set; } 

        [StringLength(300), Column("paraphrase"), Comment("中文释义"), Required]
        public string Paraphrase { get; set; } 

        [Column("type"), Comment("词汇类型(四六级)"), Required,DefaultValue(1)]
        public int Type { get; set; } = 1; //默认设置为1
            
    }
}
