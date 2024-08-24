using EnglishStudy.Entity.ChildEntity;

namespace EnglishStudy.DTO {
    /// <summary>
    /// 用于返回阅读理解做题情况分页集合
    /// </summary>
    public class PassageRecordDTO {

        /// <summary>
        /// 阅读理解标题
        /// </summary>
        public string PassageTitle { get; set; }

        /// <summary>
        /// 阅读理解id
        /// </summary>
        public int PassageId { get; set; }

        /// <summary>
        /// 答题记录id
        /// </summary>
        public int RecordId { get; set; }

        public DateTime Time { get; set; }

        public Double Accuracy { get; set; }

        public AnswerDetailList List { get; set; } = new AnswerDetailList();
    }
}
