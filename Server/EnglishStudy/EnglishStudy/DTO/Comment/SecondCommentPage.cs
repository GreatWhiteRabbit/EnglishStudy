namespace EnglishStudy.DTO.Comment {

    // 二级评论分页
    public class SecondCommentPage {
        public int Total { get; set; }

        public List<SecondCommentDTO> SecondCommentList { get; set; } = new List<SecondCommentDTO>();
    }
}
