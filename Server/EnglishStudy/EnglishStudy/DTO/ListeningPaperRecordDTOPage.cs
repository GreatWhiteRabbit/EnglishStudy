namespace EnglishStudy.DTO {
    /// <summary>
    /// 用于返回听力做题记录分页数据
    /// </summary>
    public class ListeningPaperRecordDTOPage {
        public int Total { get; set; }

        public List<ListeningPaperRecordDTO> List { get; set;} = new List<ListeningPaperRecordDTO>();
    }
}
