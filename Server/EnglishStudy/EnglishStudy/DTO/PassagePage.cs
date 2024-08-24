namespace EnglishStudy.DTO {

    /// <summary>
    /// 用于返回对阅读理解的分页查询
    /// </summary>
    public class PassagePage {
        
        /// <summary>
        /// 总条数
        /// </summary>
        public int Total { get; set; }

        /// <summary>
        /// 查询到的阅读理解文章集合
        /// </summary>
        public List<PassageDTO> PassageDTOList { get; set; } = new List<PassageDTO>();
    }
}
