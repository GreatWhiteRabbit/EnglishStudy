using EnglishStudy.Entity;

namespace EnglishStudy.DTO {
    /// <summary>
    /// 用于passgeList封装类
    /// </summary>
    public class MyPassageList {
        public List<Passage> passageList { get; set; } = new List<Passage>();
    }
}
