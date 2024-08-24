namespace EnglishStudy.DTO.Comment {

    // 一级评论分页
    public class FirstCommentPage {
        public int Total { get;set; }

        public List<FirstLeverCommentDTO> FirstLeverCommentDTOList { get; set;} = new List<FirstLeverCommentDTO>();

       
    }
}
