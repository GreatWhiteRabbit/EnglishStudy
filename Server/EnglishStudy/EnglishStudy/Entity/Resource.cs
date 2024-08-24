using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EnglishStudy.Entity {
    [Table("resource"),Comment("资源")]
    public class Resource {

        [Key,Comment("资源Id")]
        public int ResourceId { get; set; } // 资源id

        [Column("name",TypeName ="varchar(200)"),Comment("资源名称"),Required]

        public string Name { get; set; } // 资源名称

        [Column("url",TypeName ="varchar(200)"),Comment("资源URL"),Required]
        public string Url { get; set; } // 资源url

        [Column("image",TypeName ="varchar(200)"),Comment("封面")]
        public string Image { get; set; } = "null"; // 封面url

        [Column("sum"),Comment("下载次数")]
        public int Sum { get; set; } // 下载总数

        [Column("time", TypeName = "timestamp"), Comment("添加时间"), Required]
        public DateTime Time { get; set; } // 添加时间

        [Column("delete_sign"), Comment("是否显示"), DefaultValue(false)]
        public bool Delete_Sign { get; set; } // 删除标记位

    }
}
