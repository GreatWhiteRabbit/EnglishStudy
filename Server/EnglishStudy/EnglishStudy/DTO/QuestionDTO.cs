

namespace EnglishStudy.DTO {
    // 封装question的一个返回类
    public class QuestionDTO {
       
        public int QuestionId { get; set; }
        public string Title { get; set; }
   
        public string OptionsA { get; set; }

        public string OptionsB { get; set; }
        public string OptionsC { get; set; }

        public string OptionsD { get; set; }

    }
}
