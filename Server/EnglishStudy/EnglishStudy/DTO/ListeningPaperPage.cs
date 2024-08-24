using EnglishStudy.Entity;

namespace EnglishStudy.DTO {
    /// <summary>
    /// 封装听力试题分页返回类
    /// </summary>
    public class ListeningPaperPage {
        public int Total { get; set; }

        public List<ListeningPaper> List { get; set; } = new List<ListeningPaper>();
    }
}
