using EnglishStudy.DTO;
using EnglishStudy.Entity;
using EnglishStudy.Service;
using EnglishStudy.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Text;

namespace EnglishStudy.Controllers
{

    /// <summary>
    /// user控制器
    /// </summary>

    [Route("user")]
    [ApiController]
    public class UserController : ControllerBase{

        private Result result = new Result();

        private  IUserService userService;

        public UserController(IUserService userService) {
            this.userService = userService;
        }

        [HttpPost]
        [Route("login")]
        public Result Login([FromBody] User user) {
            var login = userService.Login(user.Email, user.PassWord);
            if(login.Status != MyConstant.LoginSuccess) {
                return result.failed(Utils.StatusCode.BadRequest, "账号或密码错误");
            }
            return result.Ok(login);
        }

        [HttpPost]
        [Route("adminLogin")]
        public Result AdminLogin([FromBody] User user) {
            var login = userService.AdminLogin(user.Email, user.PassWord);
            if (login.Status != MyConstant.LoginSuccess) {
                return result.failed(Utils.StatusCode.BadRequest, "账号或密码错误");
            }
            return result.Ok(login);
        }

        /// <summary>
        /// 注册控制器
        /// </summary>
        /// <param name="email">邮箱</param>
        /// <param name="password">密码</param>
        /// <param name="chaptcha">验证码</param>
        /// <returns></returns>
        [HttpPost]
        [Route("registered")]
        public Result Registered([FromBody] UserRegister userRegister) {
            var register = userService.Registered(userRegister);
            if(register.Status != MyConstant.LoginSuccess) {
                return result.failed(Utils.StatusCode.BadRequest, "注册失败");
            }
            return result.Ok(register);
        }

        /// <summary>
        /// 往邮箱发送验证码
        /// </summary>
        /// <param name="email">邮箱</param>
        /// <returns></returns>
        
        [HttpGet("chaptcha/{email}/{imageCode}/{uuid}")]
    
        public Result SendChaptchar(string email, string imageCode, string uuid) {
            var tokenDTO = userService.SendChaptchar(email,imageCode,uuid);
            return result.Ok(tokenDTO.Token);
        }

        /// <summary>
        /// 退出登录接口
        /// </summary>
        /// <param name="tokenDTO">token</param>
        /// <returns></returns>
        [HttpPost("logout")]
        public Result Logout([FromBody] TokenDTO tokenDTO) {
           bool success = userService.Logout(tokenDTO.Token);
            if(success) {
                return result.Ok();
            }
            return result.failed(Utils.StatusCode.ServerError, "服务器异常");
        }

        /// <summary>
        /// 添加用户接口
        /// </summary>
        /// <param name="userList"></param>
        /// <returns></returns>
        [Authorize(MyConstant.Admin)] // 添加管理员权限
        [HttpPost("add")]
        public Result AddUsers([FromBody] MyUserList users) {
            
            if (users.userList.Count == 0) return result.failed(Utils.StatusCode.BadRequest, "数据不能为空");
            int count = userService.AddUsers(users.userList);
            return result.Ok(count);
        }

        /// <summary>
        /// 根据id修改用户信息
        /// </summary>
        /// <param name="id">用户id</param>
        /// <param name="userDTO">用户信息</param>
        /// <returns></returns>
        [Authorize(MyConstant.UserOrAdmin)] // 添加管理员或者用户权限
        [HttpPost("update/{id}")]
        public Result UpdateUser(int id, [FromBody]UserDTO userDTO) {
           bool success = userService.UpdateUsers(id, userDTO);
            if(success) {
                return result.Ok();
            }
            return result.failed(Utils.StatusCode.ServerError, "出错啦");
        }


        /// <summary>
        /// 删除用户列表
        /// </summary>
        /// <param name="users">用户集合</param>
        /// <returns></returns>
        [Authorize(MyConstant.Admin)] // 添加管理员权限
        [HttpPost("delete")]
        public async Task<Result> DeleteUsers([FromBody] MyUserList users) {
            if (users.userList.Count == 0) return result.failed(Utils.StatusCode.BadRequest, "数据不能为空");

            int count = await userService.DeleteUsers(users.userList);
            return result.Ok(count);
        }


        /// <summary>
        /// 获取用户信息但是不包括敏感信息
        /// </summary>
        /// <param name="id">用户id</param>
        /// <returns></returns>
        [Authorize(MyConstant.UserOrAdmin)] // 添加管理员或者用户权限
        [HttpGet("{id}")]
        public Result GetInfo(int id) {
           var user = userService.GetInfo(id);
            return result.Ok(user);
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="user">user中只有密码和id</param>
        /// <returns></returns>
        [Authorize(MyConstant.UserOrAdmin)] // 添加管理员或者用户权限
        [HttpPost("password")]
        public Result UpdatePassword([FromBody] User user) {
            bool success = userService.UpdatePassword(user.UserId, user.PassWord);
            if(success) {
                return result.Ok();
            }
            return result.failed(Utils.StatusCode.ServerError, "出错啦");
        }


        /// <summary>
        /// 根据条件获取所有的用户
        /// </summary>
        /// <returns></returns>
        [Authorize(MyConstant.Admin)] // 添加管理员权限
        [HttpGet("alluser")]
        public Result GetUsers(string email = "",
            string name ="",
            int page = 1,
            int size = 10) {
            if (email.Equals("null")) email = "";
            if (name.Equals("null")) name = "";
            return result.Ok(userService.GetUsers(email,name,page,size));
        }

      

        /// <summary>
        /// 忘记密码接口
        /// </summary>
        /// <param name="userRegister"></param>
        /// <returns></returns>
        [HttpPost("forget")]
       
        public Result ForgetPassword([FromBody] UserRegister userRegister) {
           int count = userService.ForgetPassword(userRegister);
            if (count == 1) return result.failed(Utils.StatusCode.NotFound, "验证码错误");
            else if (count == 2) return result.failed(Utils.StatusCode.NotFound, "密码格式错误");
            else if (count == 3) return result.failed(Utils.StatusCode.NotFound, "用户不存在");
            return result.Ok();
        }


        /// <summary>
        /// 上传图片接口
        /// </summary>
        /// <param name="UserId">用户id</param>
        /// <param name="file">图片文件</param>
        /// <returns></returns>
      
        [Authorize(MyConstant.UserOrAdmin)] // 添加管理员或者用户权限
        [HttpPost("upload/{UserId}")]
        public async Task<Result> UploadImage(int UserId,IFormFile file) {
            string url = await userService.UploadImage(UserId, file);
            return result.Ok(url);
        }

        /// <summary>
        /// 上传图片
        /// </summary>
        /// <param name="file">图片文件</param>
        /// <returns></returns>

        [Authorize(MyConstant.UserOrAdmin)] // 添加管理员或者用户权限
        [HttpPost("upload/comment")]
        public async Task<Result> UploadCommentImage( IFormFile file) {
            string url = await userService.UploadCommentImage(file);
            return result.Ok(url);
        }

        /// <summary>
        /// 返回图片验证码
        /// </summary>
        /// <returns></returns>
        [HttpGet("imageCode")]
        public Result DrawImage() {
           return result.Ok(userService.DrawImage());
        }

        /// <summary>
        /// 忘记密码时发送验证码接口
        /// </summary>
        /// <param name="email">邮箱</param>
        /// <param name="imageCode">图片验证码</param>
        /// <param name="uuid">图片uuid</param>
        /// <returns></returns>
        [HttpPost("sendCode/{email}/{imageCode}/{uuid}")]
        public Result SendCode(string email, string imageCode, string uuid) {
            var success = userService.SendCode(email, imageCode, uuid);
            return result.Ok(success);
        }
    }
}
