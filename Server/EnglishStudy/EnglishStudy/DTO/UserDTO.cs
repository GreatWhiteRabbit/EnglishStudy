namespace EnglishStudy.DTO {

    /// <summary>
    /// 封装user返回类
    /// </summary>
    public class UserDTO {
        public string Email {  get; set; }

        public string Image { get; set; } = "null"; // 默认为null字符串

        public string NickName { get; set; } = "太懒了还未起名";
    }
}
