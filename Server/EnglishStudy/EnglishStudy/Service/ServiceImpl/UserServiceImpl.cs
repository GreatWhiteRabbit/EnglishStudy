using EnglishStudy.DTO;
using EnglishStudy.Entity;
using EnglishStudy.Utils;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

using EnglishStudy.DTO.MyImage;

namespace EnglishStudy.Service.ServiceImpl {
    public class UserServiceImpl : IUserService {
        private MyDbContext dbContext;
        private JWTHelper jWTHelper;

        private RedisHelper redisHelper = new RedisHelper();

        public UserServiceImpl(MyDbContext dbContext, JWTHelper jWTHelper) {
            this.dbContext = dbContext;
            this.jWTHelper = jWTHelper;
        }

        // 加密密钥
        private readonly string AES_Key = "yK84U9iNru1mvZwi";


        public TokenDTO Login(string email, string password) {
            TokenDTO tokenDTO = new TokenDTO();
            // 将密码加密
            string AES_Password = AES.EncodeAES(password, AES_Key);
            // 先从数据库中查找数据
            var user = dbContext.Users.Where(item => item.Email.Equals(email) && item.PassWord.Equals(AES_Password)).SingleOrDefault();
            // 账号或者密码错误
            if (user == null) {
                tokenDTO.Status = MyConstant.AccountOrPasswordError;
                return tokenDTO;
            }
            string token = jWTHelper.CreatToken(user);
            // 将token存放到Redis中
            int expireTime = 24 * 60 * 60;
            bool success = redisHelper.setString(user.Email + "-" + user.Status, token, expireTime);
            if (success) {
                tokenDTO.Token = token;
                tokenDTO.Status = MyConstant.LoginSuccess;
                tokenDTO.Id = user.UserId;
            }
            return tokenDTO;
        }

        public TokenDTO AdminLogin(string email, string password) {
            TokenDTO tokenDTO = new TokenDTO();

            // 将密码加密
            string AES_Password = AES.EncodeAES(password, AES_Key);
            // 先从数据库中查找数据
            var user = dbContext.Users
                .Where(item => item.Email.Equals(email) 
                && item.PassWord.Equals(AES_Password))
                .SingleOrDefault();
            // 账号或者密码错误
            if (user == null) {
                tokenDTO.Status = MyConstant.AccountOrPasswordError;
                return tokenDTO;
            }
            if(user.Status != 1) {
                Console.WriteLine("不是管理员");
                tokenDTO.Status = MyConstant.AccountOrPasswordError;
                return tokenDTO;
            }
            string token = jWTHelper.CreatToken(user);
            // 将token存放到Redis中
            int expireTime = 24 * 60 * 60;
            bool success = redisHelper.setString(user.Email + "-" + user.Status, token, expireTime);
            if (success) {
                tokenDTO.Token = token;
                tokenDTO.Status = MyConstant.LoginSuccess;
                tokenDTO.Id = user.UserId;
            }
            return tokenDTO;
        }

        public TokenDTO Registered(UserRegister userRegister) {
            TokenDTO tokenDTO = new TokenDTO();
            string password = userRegister.Password;
            string email = userRegister.Email;
            string chaptcha = userRegister.Chaptcha;
            // 查看验证码是否存在
            string code = redisHelper.getStringObject<string>(email);
            if (!code.Equals(chaptcha)) {
                tokenDTO.Status = MyConstant.ChaptcharError;
                return tokenDTO;
            }
            // 正则表达式
            var regex = new Regex(@"
    (?=.*[0-9])                     #必须包含数字
    (?=.*[a-zA-Z])                  #必须包含小写或大写字母
    .{8,16}                         #至少8个字符，最多30个字符
    ", RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace);
            // 判断密码是否符合规则
            if (!regex.IsMatch(password)) {
                tokenDTO.Status = MyConstant.PasswordRuleError;
                return tokenDTO;
            }
            User user = new User();
            user.Status = 2; // 1表示管理员,2表示一般用户
            user.Email = email;
            // 加密密码
            user.PassWord = AES.EncodeAES(password, AES_Key);
            // 将账号存储到数据库中
            dbContext.Users.Add(user);
            dbContext.SaveChanges();
            // 返回账号的token         
            try {
                tokenDTO.Token = jWTHelper.CreatToken(user);
                tokenDTO.Status = MyConstant.LoginSuccess;
                tokenDTO.Id = user.UserId;
                // 将token存储到Redis中
                int tokenExpire = 24 * 60 * 60; // 过期时间
                redisHelper.setString(user.Email + "-" + user.Status, tokenDTO.Token, tokenExpire);
            }
            catch (Exception ex) {
                Console.WriteLine(ex.Message);
                tokenDTO.Token = "出错了";
            }
            return tokenDTO;
        }

        // 用于注册时发送邮件
        public TokenDTO SendChaptchar(string email,string imageCode,string uuid) {

            TokenDTO tokenDTO = new TokenDTO();
            // 先检查图片验证码是否正确
            if (!redisHelper.exist("imageCode-" + uuid)) {
                // 验证码已经失效
                tokenDTO.Token = "图片验证码已经失效,请重试";
                return tokenDTO;
            }
            string code = redisHelper.getStringObject<string>("imageCode-" + uuid);
            if (!code.Equals(imageCode)) {
                // 验证码错误
                tokenDTO.Token = "图片验证码错误，请输入正确的验证码";
                return tokenDTO;
            }

            // 验证邮箱格式是否正确
            string pattern = @"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$";
            Console.WriteLine(Regex.IsMatch(email, pattern));
            if (!Regex.IsMatch(email, pattern)) {
                tokenDTO.Token = "邮箱格式错误";
                tokenDTO.Status = MyConstant.EmailRuleError;
                return tokenDTO;
            }
            // 查看邮箱是否已经存在
            var user = dbContext.Users.Where(item => item.Email.Equals(email)).SingleOrDefault();
            // 邮箱存在
            if (user != null) {
                tokenDTO.Token = "邮箱已存在";
                tokenDTO.Status = MyConstant.EmailAlreadyRegistered;
            }
            // 随机生成6位的验证码
            string chaptcha = "";
            Random random = new Random();
            for (int i = 0; i < 6; i++) {
                chaptcha = random.Next(0, 9) + chaptcha;
            }
            // 向邮箱发送验证码
            bool success = SendEmail(email, chaptcha);
            if (!success) {
                tokenDTO.Token = "邮件发送失败";
            }
            else {
                tokenDTO.Token = "邮件发送成功";
                redisHelper.setString(email, chaptcha, 5 * 60);
            }
            return tokenDTO;
        }

       
        private bool SendEmail(string emailTo, string content) {
            //设置发送方邮件信息
            string stmpServer = @"smtp.qq.com";//smtp服务器地址
            string mailAccount = @"1826311175@qq.com";//邮箱账号
            string pwd = redisHelper.getString("emailPassword");
            //string pwd = @"xcryxiqzzasaebbe";//邮箱密码（qq邮箱此处使用授权码，其他邮箱见邮箱规定使用的是邮箱密码还是授权码）
            Console.WriteLine(pwd);
            //邮件服务设置
            SmtpClient smtpClient = new SmtpClient();
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;//指定电子邮件发送方式
            smtpClient.Host = stmpServer;//指定发送方SMTP服务器
            smtpClient.EnableSsl = true;//使用安全加密连接
            smtpClient.UseDefaultCredentials = false;//不设置成false发送不出去，不和请求一起发送
            smtpClient.Credentials = new NetworkCredential(mailAccount, pwd);//设置发送账号密码

            MailMessage mailMessage = new MailMessage(mailAccount, emailTo);//实例化邮件信息实体并设置发送方和接收方
            mailMessage.Subject = "英语学习网站验证码";//设置发送邮件得标题
            mailMessage.Body = content;//设置发送邮件内容
            mailMessage.BodyEncoding = Encoding.UTF8;//设置发送邮件得编码
            mailMessage.IsBodyHtml = false;//设置标题是否为HTML格式
            mailMessage.Priority = MailPriority.Normal;//设置邮件发送优先级
            try {
                smtpClient.Send(mailMessage);//发送邮件
                return true;
            }
            catch (SmtpException ex) {
                Console.WriteLine(ex.ToString());

            }
            return false;
        }

        public bool Logout(string token) {
            // 先将token解析成User对象
            var user = jWTHelper.VerifyToken(token);
            if (user != null) {
                // 删除redis中的对应数据
                bool delete = redisHelper.deleteKey(user.Email + "-" + user.Status);
                if (delete) {
                    // 返回成功
                    return true;
                }
            }
            return false;
        }

        public int AddUsers(List<User> userList) {

            int count = 0;
            // 将密码加密
            for(int i = 0; i < userList.Count; i++) {
                userList[i].PassWord = AES.EncodeAES(userList[i].PassWord, AES_Key);
            }
            dbContext.Users.AddRange(userList);
            count = dbContext.SaveChanges();
            return count;
        }

        public bool UpdateUsers(int id, UserDTO userDTO) {
            // 根据id从数据库中查找用户
            var user = dbContext.Users.Where(item => item.UserId == id).Single();
            // 用户不存在，直接返回错误信息
            if(user == null) {
                return false;
            }
            // 修改user的内容
            if (userDTO.NickName != null) user.NickName = userDTO.NickName;
            if (userDTO.Image != null) user.Image = userDTO.Image;
            int count = dbContext.SaveChanges();
            // 如果修改成功，count=1
            return (count == 1);

        }

        public async Task<int> DeleteUsers(List<User> userList) {
            int count = 0;
            // 根据userList中的id删除用户
            foreach(var user in userList) {
                if (user != null) {
                    int id = user.UserId;
                    // 根据id获取到数据
                    var result = dbContext.Users.Where(item => item.UserId == id).Single();
                    // 异步删除数据
                    dbContext.Users.Remove(result);
                    count = count + await dbContext.SaveChangesAsync();
                }
            }
            // 返回成功删除的个数
            return count;
        }

        public User GetInfo(int id) {
            // 从数据库中获取数据
            var result = dbContext.Users.Where(item => item.UserId == id).Single();
            User user = new User();
            if (result != null) {
                user.Email = result.Email;
                user.NickName = result.NickName;
                user.Image = result.Image;
                user.UserId = result.UserId;
            }
            return user;
        }

        public bool UpdatePassword(int id, string password) {
           
            // 先判断password是否符合规则
            // 正则表达式
            var regex = new Regex(@"
    (?=.*[0-9])                     #必须包含数字
    (?=.*[a-zA-Z])                  #必须包含小写或大写字母
    .{8,16}                         #至少8个字符，最多30个字符
    ", RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace);
           // 密码不符合规则，返回false
            if(!regex.IsMatch(password)) {
                Console.WriteLine("密码不符合要求");
                return false;
            }
            // 从数据库中查找数据
            var result = dbContext.Users.Where(item => item.UserId == id).Single();
            if(result == null) {
                Console.WriteLine("数据库中没有此用户的记录");
                return false;
            }
            // 将密码加密
            string New_Password = AES.EncodeAES(password,AES_Key);
            // 修改密码
            result.PassWord = New_Password;
            // 如果前后密码一致，savechanges的值为0
            return (dbContext.SaveChanges() == 1);
            
        }

        public List<User> GetUsers(string email, string name, int page = 1, int size = 10) {
           return  dbContext.Users
                .Where(item => item.Email.Contains(email)
                && item.NickName.Contains(name))
                .Skip((page - 1) * size)
                .Take(size)
                .ToList();
           
        }

        public int ForgetPassword(UserRegister userRegister) {
            // 先从Redis中查找验证码是否存在
            string key = userRegister.Email;
            string value = redisHelper.getStringObject<string>(key);
            if(!userRegister.Chaptcha.Equals(value)) {
                // 验证码错误
                return 1;
            }
            // 正则表达式
            var regex = new Regex(@"
    (?=.*[0-9])                     #必须包含数字
    (?=.*[a-zA-Z])                  #必须包含小写或大写字母
    .{8,16}                         #至少8个字符，最多30个字符
    ", RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace);
            // 密码不符合规则，返回2
            if (!regex.IsMatch(userRegister.Password)) {
                return 2;
            }
            //修改密码
            var result = dbContext.Users
                .Where(item => item.Email == userRegister.Email)
                .SingleOrDefault();
            if(result == null) {
                // 用户不存在
                return 3;
            }
            result.PassWord = AES.EncodeAES(userRegister.Password,AES_Key);
            dbContext.SaveChanges();
            return 4;
        }

        public async Task<string> UploadImage(int UserId, IFormFile file) {
            string savePath = @"D:\nginx\image\";
            string accessURL = @"https://localhost:7031/resouce/image/";
            string url = "";
            // 获取文件后缀
            string suffix = Path.GetExtension(file.FileName);
            if(file.Length >= 5 * 1024 * 1024) {
                return "图片大小超出限制";
            }
            // 将文件保存
            try {
                string fileName = Guid.NewGuid().ToString();
                fileName = fileName + suffix;
                string saveName = savePath + fileName;
                using (var stream = new FileStream(saveName, FileMode.Create)) {
                    await file.CopyToAsync(stream);
                }
                url = accessURL + fileName;
                // 将生成的图片保存到MySQL中
               var result =  dbContext.Users
                    .Where(item => item.UserId == UserId)
                    .SingleOrDefault();
                if(result == null) {
                    return "";
                }
                result.Image = url;
                dbContext.SaveChanges();
                return url;
            }
            catch (Exception ex) {
                Console.WriteLine(ex.ToString());
                return "";
            }
        }

        public async Task<string> UploadCommentImage( IFormFile file) {
            string savePath = @"D:\nginx\image\";
            string accessURL = @"https://localhost:7031/resouce/image/";
            string url = "";
            // 获取文件后缀
            string suffix = Path.GetExtension(file.FileName);
            if (file.Length >= 5 * 1024 * 1024) {
                return "图片大小超出限制";
            }
            // 将文件保存
            try {
                string fileName = Guid.NewGuid().ToString();
                fileName = fileName + suffix;
                string saveName = savePath + fileName;
                using (var stream = new FileStream(saveName, FileMode.Create)) {
                    await file.CopyToAsync(stream);
                }
                url = accessURL + fileName;
                return url;
            }
            catch (Exception ex) {
                Console.WriteLine(ex.ToString());
                return "";
            }
        }

        public ImageCode DrawImage() {
            ImageCode imageCode = new ImageCode();
            string text = CreateImageCode(4);
            byte[] bytes = ImageHelper.GetVerifyCode(text);
            // 生成一个uuid
            string uuid = Guid.NewGuid().ToString();
            
            string dateTime =  DateTime.Now.Year.ToString() + '-' + DateTime.Now.Month.ToString() + '-' + DateTime.Now.Day.ToString();
            string savePath = @"D:\nginx\image\code\" + dateTime + @"\";
            string accessURL = @"https://localhost:7031/resouce/image/code/" + dateTime;
            string saveName = savePath + uuid + ".png";
            // 如果文件夹不存在，创建文件夹
            if(!Directory.Exists(savePath)) {
                Directory.CreateDirectory(savePath);
            }
            // 将数据保存到本地
            FileStream fs = new FileStream(saveName,FileMode.Create);
            fs.Write(bytes, 0, bytes.Length);
            fs.Close();

            // 将生成的uuid保存到Redis中
            redisHelper.setString("imageCode-" +  uuid, text,5 * 60);
            imageCode.uuid = uuid;
            imageCode.url = accessURL + "/" + uuid + ".png";
            return imageCode;
        }

        /// <summary>
        /// 随机生成count位图形验证码
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public string CreateImageCode(int count) {
            var characters = "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghjklmnopqrstuvwxyz23456789";
            string text = "";
            var random = new Random();
            for (int i = 0; i < count; i++) {
                text = text + characters[random.Next(characters.Length)];

            }
            return text;
        }

        // 用于忘记密码发送邮箱验证码
        public bool SendCode(string email, string imageCode, string uuid) {
            // 先检查图片验证码是否正确
            if (!redisHelper.exist("imageCode-" + uuid)) {
                // 验证码已经失效
                return false;
            }
            string code = redisHelper.getStringObject<string>("imageCode-" + uuid);
            if(!code.Equals(imageCode)) {
                // 验证码错误
                return false;
            }
            // 检查用户是否存在
            var result = dbContext.Users
                .Where(item => item.Email == email).SingleOrDefault();
            if(result == null) {
         
                // 用户不存在
                return false; 
            }
            // 随机生成6位的验证码
            string chaptcha = "";
            Random random = new Random();
            for (int i = 0; i < 6; i++) {
                chaptcha = random.Next(0, 9) + chaptcha;
            }
            // 发送验证码
            bool send = SendEmail(email, chaptcha);
            if(send == false) {
                return false;
            }
            else {
                redisHelper.setString(email, chaptcha, 5 * 60);
                return true;
            }
            
        }
    }
}
