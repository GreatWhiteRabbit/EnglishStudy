using EnglishStudy.DTO;
using EnglishStudy.DTO.MyImage;
using EnglishStudy.Entity;

namespace EnglishStudy.Service {

    /// <summary>
    /// user接口
    /// </summary>
    public interface IUserService {

        /// <summary>
        /// 登录服务
        /// </summary>
        /// <param name="email">邮箱</param>
        /// <param name="password">密码</param>
        /// <returns>TokenDTO</returns>
        public TokenDTO Login(string email, string password);

        public TokenDTO AdminLogin(string email, string password);

        /// <summary>
        /// 注册服务
        /// </summary>
        /// <param name="email">邮箱</param>
        /// <param name="password">密码</param>
        /// <param name="chaptcha">验证码</param>
        /// <returns>TokenDTO</returns>
        public TokenDTO Registered(UserRegister userRegister);

        /// <summary>
        /// 往邮箱发送验证码服务
        /// </summary>
        /// <param name="email">邮箱</param>
        /// <returns>TokenDTO</returns>
        public TokenDTO SendChaptchar(string email, string imageCode, string uuid);

        /// <summary>
        /// 退出登录服务
        /// </summary>
        /// <param name="token">用户的token</param>
        /// <returns>false表示操作失败，true表示操作成功</returns>
        public bool Logout(string token);

        /// <summary>
        /// 添加用户服务
        /// </summary>
        /// <param name="userList">用户列表</param>
        /// <returns>添加成功的个数</returns>
        public int AddUsers(List<User> userList);

        /// <summary>
        /// 修改用户的信息
        /// </summary>
        /// <param name="userDTO">用户信息</param>
        /// <param name="id">用户id</param>
        /// <returns>true表示修改成功</returns>
        public bool UpdateUsers(int id ,UserDTO userDTO);

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="userList">用户列表</param>
        /// <returns>成功删除的个数</returns>
        public Task<int> DeleteUsers(List<User> userList);

        /// <summary>
        /// 根据用户id获取用户信息
        /// </summary>
        /// <param name="id">id</param>
        /// <returns>用户信息</returns>
        public User GetInfo(int id);

        /// <summary>
        /// 根据id修改用户密码
        /// </summary>
        /// <param name="id">用户id</param>
        /// <param name="password">密码</param>
        /// <returns>true表示修改成功</returns>
        public bool UpdatePassword(int id, string password);

        /// <summary>
        /// 模糊匹配获取所有的user
        /// </summary>
        /// <param name="prefix">模糊匹配</param>
        /// <returns>所有的user</returns>
        public List<User> GetUsers(string email, string name, int page = 1, int size = 10);

        /// <summary>
        /// 忘记密码
        /// </summary>
        /// <param name="userRegister"></param>
        /// <returns></returns>
        public int ForgetPassword(UserRegister userRegister);

        /// <summary>
        /// 上传图片服务
        /// </summary>
        /// <param name="UserId">用户id</param>
        /// <param name="file">图片文件</param>
        /// <returns>图片的地址</returns>
        public Task<string> UploadImage(int UserId,IFormFile file);


        /// <summary>
        /// 上传图片接口
        /// </summary>
        /// <param name="file">图片</param>
        /// <returns></returns>
        public Task<string> UploadCommentImage(IFormFile file);

        /// <summary>
        /// 生成图片验证码
        /// </summary>
        /// <returns></returns>
        public ImageCode DrawImage();

        /// <summary>
        /// 发送验证码接口，用户需要输入图形验证码才能发送，主要用于找回密码
        /// </summary>
        /// <param name="email">邮箱</param>
        /// <param name="imageCode">图片验证码内容</param>
        /// <param name="uuid">图片的uuid</param>
        /// <returns>true表示发送成功</returns>
        public bool SendCode(string email, string imageCode,string uuid);
    }
}
