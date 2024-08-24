
namespace EnglishStudy.DTO.Resource {
    // 返回资源封装类
    public class ResourceDTO {
      
        public int ResourceId { get; set; } // 资源id

        public string Name { get; set; } // 资源名称


        public string Url { get; set; } = "null"; // 资源url


        public string Image { get; set; } = "null"; // 封面url

 
        public int Sum { get; set; } // 下载总数

    
        public DateTime Time { get; set; } // 添加时间


        public bool Delete_Sign { get; set; } = false; // 删除标记位
    }
}
