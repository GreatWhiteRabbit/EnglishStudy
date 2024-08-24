

namespace EnglishStudy.DTO {

    /// <summary>
    /// 封装答案返回类
    /// </summary>
    public class AnwserDTO {
        public int QuestionId { get; set; }

        public int PassageId { get; set; }

        public string Answer { get; set; }

        public string Explanation { get; set; }
    }
}
