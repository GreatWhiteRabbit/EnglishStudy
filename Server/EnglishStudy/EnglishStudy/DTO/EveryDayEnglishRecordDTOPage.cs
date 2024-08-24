namespace EnglishStudy.DTO {
    /// <summary>
    /// 用于封装返回分页的每日英语阅读记录
    /// </summary>
    public class EveryDayEnglishRecordDTOPage {

        public int Total { get; set; }

        public List<EveryDayEnglishRecordDTO> RecordList { get; set; } = new List<EveryDayEnglishRecordDTO>();
    }
}
