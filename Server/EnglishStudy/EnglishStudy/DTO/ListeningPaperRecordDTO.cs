using EnglishStudy.Entity.ChildEntity;

namespace EnglishStudy.DTO {
    /// <summary>
    /// 用户封装听力试题做题记录返回类
    /// </summary>
    public class ListeningPaperRecordDTO {
        public int ListeningPaperId { get; set; }

        public string PaperTitle { get; set; }

        public int ListeningPaperRecordId { get; set; }

        public DateTime Time { get; set; }

        public Double Accuracy { get; set; }

        public AnswerDetailList List { get; set; } = new AnswerDetailList();
    }
}
