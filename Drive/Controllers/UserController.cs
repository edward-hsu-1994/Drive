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
    public class UserController : ManageBaseController {
        public UserController(DriveLogicManager manager) : base(manager) {
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
                    Role = DriveToken.Roles.Administrator,
                    Subject = DriveToken.Subjects.ConsoleLogin,
                    IssuedAt = DateTime.Now,
                    Expires = DateTime.Now.AddHours(12)
                }
            };

            var key = new SymmetricSecurityKey(Startup.Configuration.GetSection("JWT:SecureKey")
                .Value.ToHash<MD5>()
            );

            return tokenModel.Sign(key);
        }
    }
}
