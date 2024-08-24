using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EnglishStudy.Entity {

    [Table("user")]
    public class User {

        [Key,Column("user_id"),Comment("主键id")]
        
        public int UserId { get; set; }

        [Column("email"), Required, StringLength(40), Comment("邮箱")]
        public string Email { get; set; } 

        [Column("image"), Required, StringLength(200), Comment("头像"),DefaultValue("null")]
        
        public string Image { get; set; } = "null";

        [Column("nickname"), StringLength(20), Comment("昵称"),DefaultValue("太懒了还未起名")]
        public string NickName { get; set; } = "太懒了还未起名";


        [Column("password"), Required, StringLength(255), Comment("密码")]
        public string PassWord { get; set; } 

        [Column("status"), Required, Comment("级别(管理员或普通用户)"),DefaultValue(2)]

        public int Status { get; set; } = 2;
    }
}
