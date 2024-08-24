namespace EnglishStudy.DTO {
    /// <summary>
    /// 用于封装每日英语阅读记录返回类
    /// </summary>
    public class EveryDayEnglishRecordDTO {
      
        public int RecordId { get; set; }

        public int EverydayId { get; set; }

        public string Title { get; set; } = "The article is not found";

        public string TitleTranslations { get; set; } = "文章不见啦";

        public DateTime Time { get; set; }
    }
}
