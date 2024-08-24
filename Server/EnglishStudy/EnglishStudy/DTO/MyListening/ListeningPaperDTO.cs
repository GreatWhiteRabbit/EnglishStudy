namespace EnglishStudy.DTO.MyListening {
    // 这是ListeningPaper的封装类，用于接收客户端数据或者发送数据到客户端
    public class ListeningPaperDTO {

        public int Id {  get; set; }

        public string PaperTitle { get; set; } = "null";
        public string Audio { get; set; } = "null";
    }
}
