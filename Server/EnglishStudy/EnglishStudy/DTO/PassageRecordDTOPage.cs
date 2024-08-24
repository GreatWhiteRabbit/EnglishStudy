namespace EnglishStudy.DTO {
    /// <summary>
    /// 用于返回阅读理解做题记录分页
    /// </summary>
    public class PassageRecordDTOPage {
        public int Total { get; set; }

        public List<PassageRecordDTO> List { get; set; } = new List<PassageRecordDTO>();
    }
}
