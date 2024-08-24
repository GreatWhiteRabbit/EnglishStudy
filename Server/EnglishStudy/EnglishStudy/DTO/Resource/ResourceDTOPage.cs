namespace EnglishStudy.DTO.Resource {
    // 对资源返回类进行分页封装
    public class ResourceDTOPage {
        public int Total { get; set; }
        public List<ResourceDTO> List { get; set; } = new List<ResourceDTO>();
    }
}
