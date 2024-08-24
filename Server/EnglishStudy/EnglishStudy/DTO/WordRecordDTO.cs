namespace EnglishStudy.DTO {
    /// <summary>
    /// 用于封装返回要记忆的单词
    /// </summary>
    public class WordRecordDTO {

        public int Id { get; set; }

        public string Words { get; set; }


        public string Phonetic { get; set; }


        public string Paraphrase { get; set; }
    }
}
