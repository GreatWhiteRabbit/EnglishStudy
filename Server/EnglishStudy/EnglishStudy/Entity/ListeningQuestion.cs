using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EnglishStudy.Entity {
    [Table("listening_question")]
    public class ListeningQuestion {

        [Key,Column("question_id"),Comment("听力小题题号")]
        public int QuestionId { get; set; }

        [Column("part_id"),Comment("听力段号")]
        public int PartId { get; set; }

        [Column("options_a",TypeName ="varchar(255)"),Comment("A选项"),Required]
        public string OptionsA { get; set; }
        [Column("options_b",TypeName ="varchar(255)"),Comment("B选项"),Required]

        public string OptionsB { get; set; }
        [Column("options_c", TypeName = "varchar(255)"), Comment("C选项"), Required]

        public string OptionsC { get; set; }
        [Column("options_d", TypeName = "varchar(255)"), Comment("D选项"), Required]

        public string OptionsD { get; set; }
        [Column("title",TypeName ="varchar(255)"),Comment("题目内容"),Required]
        public string Title { get; set; }
        [Column("answer",TypeName ="varchar(10)"),Comment("答案"),Required]
        public string Answer { get; set; }
        [Column("delete_sign"),Comment("删除标记位"),DefaultValue(0)]
        public int DeleteSign { get; set; }
    }
}
