using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EnglishStudy.Entity {

    [Table("passage")]
    public class Passage {
        // 文章id
        [Comment("文章ID"),Key,Column("passage_id")]
        public int PassageId { get; set; }

        // 文章标题
        [Comment("文章标题"),Column("title", TypeName = "varchar(100)")]
        public string Title { get; set; }

        [Comment("文章内容"),Column("content", TypeName = "text"),Required]
        // 文章内容
        public string Content { get; set; }
        [Comment("删除标记位"),Column("delete_sign"),DefaultValue(0)]
        // 是否删除
        public int DeleteSign { get; set; }

        
        public List<Question> QuestionList { get; set; } = new List<Question>();
    }
}
