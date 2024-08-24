namespace EnglishStudy.DTO {

    /// <summary>
    /// 用于修改或者添加题目的封装类
    /// </summary>
    public class UpdateQuestionDTO {
        public int QuestionId { get; set; }

        public int PassageId { get; set; }
        public string Title { get; set; }

        public string OptionsA { get; set; }

        public string OptionsB { get; set; }
        public string OptionsC { get; set; }

        public string OptionsD { get; set; }

        public string Answer { get; set; }

        public string Explanation { get; set; }
    }
}
