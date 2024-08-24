

using EnglishStudy.Entity;

namespace EnglishStudy.DTO
{
    /// <summary>
    /// 用于网络传输wordList集合
    /// </summary>
    public class MyWordList
    {
        public List<Word> wordList { get; set; } = new List<Word>();
    }
}
