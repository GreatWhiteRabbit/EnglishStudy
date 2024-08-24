using EnglishStudy.Entity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace EnglishStudy.Utils {
    public class JWTHelper {
        private readonly IConfiguration _configuration;
        public JWTHelper(IConfiguration configuration) {
            _configuration = configuration;
        }

        private readonly int expireTime = 1;

        /// <summary>
        /// 生成token
        /// </summary>
        /// <param name="user">user类</param>
        /// <returns>生成的token</returns>
        public string CreatToken(User user) {
            List<Claim> claims = new List<Claim>();
            // 邮箱
            claims.Add(new Claim(ClaimTypes.Email, user.Email));
            // 角色（管理员还是一般用户）
           // claims.Add(new Claim(ClaimTypes.Role,user.Status.ToString()));
            claims.Add(new Claim("Status", user.Status.ToString()));
            // 用户id
            claims.Add(new Claim(ClaimTypes.NameIdentifier,user.UserId.ToString()));
            // 设置token过期时间
            DateTime express = DateTime.Now.AddDays(expireTime);

            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:SecretKey"]));
            var credetials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

            // 5. 根据以上，生成token
            var jwtSecurityToken = new JwtSecurityToken(
                 _configuration["JWT:Issuer"],     //Issuer
            _configuration["JWT:Audience"],   //Audience
            claims,                          //Claims,
            DateTime.Now,                    //notBefore
            express,    //expires
            credetials               //Credentials                  
            );
            var token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            return token.ToString();
        }


        /// <summary>
        /// 解析token，返回token原来的对象
        /// </summary>
        /// <param name="token">token</param>
        /// <returns>token解析后的user对象</returns>
        public User VerifyToken(string token) {
            User user = new User();
            var tokenHandler = new JwtSecurityTokenHandler();
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:SecretKey"]));
            ClaimsPrincipal claimsPrincipal = null;
            try {
               claimsPrincipal = tokenHandler.ValidateToken(token, new TokenValidationParameters {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = _configuration["JWT:Issuer"],
                    ValidAudience = _configuration["JWT:Audience"],
                    IssuerSigningKey = secretKey
                },out var validatedToken);
            } catch (Exception ex) {
                Console.WriteLine(ex.Message);
                return null;
            }

            Claim claim1 = claimsPrincipal.Claims.ToList()[0];
            Claim claim2 = claimsPrincipal.Claims.ToList()[1];
            Claim claim3 = claimsPrincipal.Claims.ToList()[2];
            user.Email = claim1.Value;
            user.Status = int.Parse(claim2.Value);
            user.UserId = int.Parse(claim3.Value);
            return user;
        }
        
    }
}
