using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EnglishStudy.Entity {
    [Table("everyday_english")]
    public class EveryDayEnglish {

        [Key,Column("everyday_id"),Comment("每日英语主键")]
        public int Everyday_id {  get; set; }

        [Column("title",TypeName="varchar(200)"),Comment("标题"),Required]
        public string Title { get; set; }

        [Column("content",TypeName ="text"),Comment("内容"),Required]
        public string Content { get; set; }

        [Column("translations", TypeName = "text"), Comment("内容翻译"), Required]
        public string Translations { get; set; }

        [Column("delete_sign"), Comment("删除标志位"), DefaultValue(0)]
        public int Delete_Sign { get; set; }

        [Column("time",TypeName ="timestamp"),Comment("创建时间")]
        public DateTime Time { get; set; }

        [Column("title_translations", TypeName = "varchar(200)"), Comment("标题翻译"), Required]
        public string TitleTranslations { get; set; }
    }
}
