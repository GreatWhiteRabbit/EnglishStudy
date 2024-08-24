namespace EnglishStudy.DTO.MyImage {
    /// <summary>
    /// 用于返回生成的图片验证码
    /// </summary>
    public class ImageCode {
       public string url { get; set; } // 图片地址

        public string uuid { set; get; } // 图片的uuid，方便到时候验证图片的验证码
    }
}
