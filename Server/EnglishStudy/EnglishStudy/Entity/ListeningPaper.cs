using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EnglishStudy.Entity {

    [Table("listening_paper"),Comment("听力试题表")]
    public class ListeningPaper {
        [Key,Column("listening_paper_id"),Comment("听力试题id")]
        public int ListeningPaperId { get; set; }
        [Column("papre_title",TypeName = "varchar(100)"),Comment("听力试题标题"),Required]
        public string PaperTitle { get; set; }
        [Column("audio",TypeName = "varchar(200)"),Comment("听力音频URL"),Required]
        public string Audio { get; set; }
        [Column("delete_sign"),Comment("删除标记位"),DefaultValue(0)]
        public int DeleteSign { get; set; }

        public List<Part> PartList { get; set; } = new List<Part>();
    }
}
