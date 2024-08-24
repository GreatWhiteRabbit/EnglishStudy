namespace EnglishStudy.DTO {

    /// <summary>
    /// 用户注册类和忘记密码封装类
    /// </summary>
    public class UserRegister {
        public string Email {  get; set; }
        public string Password { get; set; }
        public string Chaptcha { get; set; }
    }
}
