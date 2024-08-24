namespace EnglishStudy.Entity.ChildEntity {
    /// <summary>
    /// 用于记录单选题作答情况
    /// </summary>
    public class AnswerDetail {

        // 题号
        public int QuestionId { get; set; }

        // 答案
        public string Answer { get; set; }
    }
}
