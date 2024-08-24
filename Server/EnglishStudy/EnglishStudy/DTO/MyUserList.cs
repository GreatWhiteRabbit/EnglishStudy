using EnglishStudy.Entity;

namespace EnglishStudy.DTO {

    /// <summary>
    /// userList的集合，用于接收userList的json参数
    /// </summary>
    public class MyUserList {
        public List<User> userList { get; set; } = new List<User>();
    }
}
