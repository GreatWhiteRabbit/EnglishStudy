namespace EnglishStudy.DTO {
    /// <summary>
    /// 用于返回token
    /// </summary>
    public class TokenDTO {
        /// <summary>
        /// token
        /// </summary>
        public string Token { get; set; } = "null";

        /// <summary>
        /// 1表示账号不存在
        /// 2不表示账号密码错误
        /// 3表示邮箱已注册
        /// 4表示登录成功
        /// 5表示验证码错误
        /// 6表示密码不符合规则
        /// </summary>
        public int Status { get; set; }

        public int Id { get; set; }
    }
}
