using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Drive.Base.Jwt;
using Drive.Base.Mvc;
using Drive.FileSystem;
using Drive.Logic;
using Drive.Models.EF;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using XWidget.Web.Exceptions;
using XWidget.Web.Jwt;

namespace Drive.Controllers {
    /// <summary>
    /// 使用者控制器
    /// </summary>
    [Authorize(Roles = DriveToken.Roles.Administrator)]
    public class UserController : BaseController {
        public UserController(DriveLogicManager manager) : base(manager) {
        }

        /// <summary>
        /// 取得使用者列表
        /// </summary>
        /// <returns>使用者列表</returns>
        [HttpGet]
        public async Task<IEnumerable<User>> List() {
            return this.Mask(await Manager.UserLogic.ListAsync());
        }

        /// <summary>
        /// 取得指定使用者資訊
        /// </summary>
        /// <param name="userId">使用者唯一識別號</param>
        /// <returns>使用者資訊</returns>
        [HttpGet("{userId}")]
        public async Task<User> Get([FromRoute]string userId) {
            return this.Mask(await Manager.GetAsync<User>(userId));
        }

        /// <summary>
        /// 建立使用者
        /// </summary>
        /// <param name="user">使用者資訊</param>
        /// <returns>建立的使用者資訊</returns>
        [HttpPost]
        public async Task<User> Create([FromBody]User user) {
            return await Manager.CreateAsync(user);
        }

        /// <summary>
        /// 更新使用者
        /// </summary>
        /// <param name="user">使用者資訊</param>
        /// <returns>更新後使用者資訊</returns>
        [HttpPut]
        public async Task<User> Update([FromBody]User user) {
            return await Manager.UpdateAsync(user);
        }

        /// <summary>
        /// 刪除指定使用者
        /// </summary>
        /// <param name="userId">使用者唯一識別號</param>
        [HttpDelete("{userId}")]
        public async Task Delete([FromRoute]string userId) {
            if (Manager.List<User>(x => x.IsAdmin).Count() == 1) throw new OperatorException("至少要有一個管理者");
            await Manager.DeleteAsync<User>(userId);
        }

        /// <summary>
        /// 使用者登入
        /// </summary>
        /// <returns>存取權杖</returns>
        [AllowAnonymous]
        [HttpPost("token")]
        public async Task<string> Authorize(
            [FromBody]User user) {
            var targetUser = await Manager.UserLogic.GetAsync(user.Id);

            if (!targetUser.MatchPassword(user.Password)) {
                throw new AuthorizationException();
            }

            return BuildToken(user);
        }

        /// <summary>
        /// 驗證存取權杖並取得角色名稱
        /// </summary>
        /// <param name="token">存取權杖</param>
        /// <returns>驗證結果</returns>
        [AllowAnonymous]
        [HttpPost("verify")]
        public async Task<string> Verify([FromBody]string token) {
            var tokenInfo = VerifyToken(token);

            if (tokenInfo == null) return null;

            return tokenInfo.Payload.Role;
        }

        private DriveToken VerifyToken(string token) {
            if (JwtTokenConvert.Verify<DriveToken, DefaultJwtHeader, MvcIdentityPayload>(token, new TokenValidationParameters() {
                IssuerSigningKey = new SymmetricSecurityKey(Startup.Configuration.GetSection("JWT:SecureKey").Value.ToHash<MD5>()),
                ValidIssuer = Startup.Configuration.GetSection("JWT:Issuer").Value, // 驗證的發行者
                ValidAudience = Startup.Configuration.GetSection("JWT:Audience").Value, // 驗證的TOKEN接受者

                ValidateIssuerSigningKey = true,
                ValidateIssuer = true, // 檢查TOKEN發行者
                ValidateAudience = true, // 檢查該TOKEN是否發給本服務
                ValidateLifetime = true // 檢查TOKEN是否有效
            }, out DriveToken tokenInfo)) {
                return tokenInfo;
            };
            return null;
        }

        private string BuildToken(User user) {
            var tokenModel = new DriveToken() {
                Header = new DefaultJwtHeader() {
                    Algorithm = SecurityAlgorithms.HmacSha256
                },
                Payload = new MvcIdentityPayload() {
                    Actor = user.Id,
                    Issuer = Startup.Configuration.GetSection("JWT:Issuer").Value,
                    Audience = Startup.Configuration.GetSection("JWT:Audience").Value,
                    Name = user.Id,
                    Role = user.IsAdmin ? DriveToken.Roles.Administrator : DriveToken.Roles.Default,
                    Subject = DriveToken.Subjects.ConsoleLogin,
                    IssuedAt = DateTime.Now,
                    Expires = DateTime.Now.AddDays(7)
                }
            };

            var key = new SymmetricSecurityKey(Startup.Configuration.GetSection("JWT:SecureKey")
                .Value.ToHash<MD5>()
            );

            return tokenModel.Sign(key);
        }
    }
}
