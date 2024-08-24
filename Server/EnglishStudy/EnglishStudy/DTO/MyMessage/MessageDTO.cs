namespace EnglishStudy.DTO.MyMessage {
    // 用于返回系统消息
    public class MessageDTO {
        public int MessageId { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }

        public DateTime Time { get; set; }

        public int UserId { get; set; }

        public int ReceiveId { get; set; }

        public int Read { get; set; }
    }
}
