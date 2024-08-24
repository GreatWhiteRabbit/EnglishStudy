using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EnglishStudy.Entity {

    [Table("part"),Comment("听力段部分")]
    public class Part {

        [Key,Column("part_id"),Comment("听力小题部分")]
        public int PartId { get; set; }
        [Column("listening_paper_id")]
        public int ListeningPaperId { get; set; }
        [Column("original_text",TypeName = "text"),Comment("听力原文"),Required]
        public string OriginalText { get; set; }
        [Column("part_title",TypeName = "varchar(100)"),Comment("题目"),Required]
        public string PartTitle { get; set; } // 例如 “请听下面对话，回答1-2小题”
        [Column("delete_sign"),Comment("删除标记位"),DefaultValue(0)]
        public int DeleteSign { get; set; } 

        public List<ListeningQuestion> ListeningQuestionList { get; set; } = new List<ListeningQuestion>();
    }
}
