namespace EnglishStudy.DTO
{

    /// <summary>
    /// 封装wordDTO集合
    /// </summary>
    public class WordPage
    {
        public List<WordDTO> wordDTOs { get; set; } = new List<WordDTO>();
        public int total { get; set; } = 0;
    }
}
