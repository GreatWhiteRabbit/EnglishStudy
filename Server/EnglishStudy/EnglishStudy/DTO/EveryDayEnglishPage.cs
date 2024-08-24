using EnglishStudy.Entity;

namespace EnglishStudy.DTO {

    public class EveryDayEnglishPage {
        public int Total { get; set; }

        public List<EveryDayEnglish> List { get; set; } = new List<EveryDayEnglish>();
    }
}
