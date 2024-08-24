using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EnglishStudy.Entity {
    [Table("question")]
    public class Question {
        // 问题ID
        [Key,Comment("问题ID"),Column("question_id")]
        public int QuestionId { get; set; }
        // 文章ID
        
        public int PassageId { get; set; }

        // 具体问题
        [Comment("具体问题"),Column("title",TypeName = "varchar(255)"),Required]
        public string Title { get; set; }

        // 选项A
        [Comment("A选项"),Column("options_a", TypeName = "varchar(255)"),Required]
        public string OptionsA { get; set; }

        // 选项B
        [Comment("B选项"), Column("options_b", TypeName = "varchar(255)"), Required]
        public string OptionsB { get; set; }

        // 选项C
        [Comment("C选项"), Column("options_c", TypeName = "varchar(255)"), Required]
        public string OptionsC { get; set; }

        // 选项D
        [Comment("D选项"), Column("options_d", TypeName = "varchar(255)"), Required]
        public string OptionsD { get; set; }

        // 答案
        [Comment("正确答案"), Column("answer", TypeName = "varchar(10)"), Required]
        public string Answer { get; set; }

        // 答案解释
        [Comment("答案解释"),Column("explanation", TypeName = "text"),DefaultValue("略")]
        public string Explanation { get; set; }

        // 删除标记
        [Comment("删除标记位"),Column("delete_sign"),DefaultValue(0)]
        public int DeleteSign { get; set; }

       // public Passage Passage { get; set; } = null;
    }
}
