using EnglishStudy.Entity;

namespace EnglishStudy.DTO.MyListening {
    // 添加part的封装类
    public class AddPartDTO {
        public int listeningPaperId {  get; set; }

        public string OriginalText { get; set; }

        public string PartTitle { get; set; }

        public List<ListeningQuestion> ListeningQuestionList { get; set; } = new List<ListeningQuestion>();
    }
}
