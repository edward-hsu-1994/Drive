using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Drive.Base.Jwt;
using Drive.Base.Mvc;
using Drive.FileSystem;
using Drive.Logic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.IdentityModel.Tokens;
using XWidget.Web.Exceptions;
using XWidget.Web.Jwt;

namespace Drive.Controllers {
    public class FileController : BaseController {
        public FileController(DriveLogicManager manager) : base(manager) {

        }

        [HttpGet]
        [HttpGet("{*path}")]
        public IEnumerable<IFileSystemItem> List(string path) {
            if (string.IsNullOrWhiteSpace(path)) {
                path = "";
            }

            var rootDirectory = DirectoryEitity.FromPath(Startup.Configuration[Startup.RootDirectory]);

            string fullPath = System.IO.Path.Combine(Startup.Configuration[Startup.RootDirectory], path);

            return DirectoryEitity.FromPath(fullPath).GetChildren().Select(x => {
                x.RelativePath = x.Path.Substring(rootDirectory.Path.Length);
                if (x is FileEitity file) {
                    file.DownloadUrl = $"/api/File/download?path={Uri.EscapeDataString(x.RelativePath)}&token={BuildToken(file)}";
                }
                return x;
            });
        }

        [AllowAnonymous]
        [HttpGet("download")]
        public IActionResult Download(
            [FromQuery]string path,
            [FromQuery]string token) {
            DriveToken tokenInfo = VerifyToken(token);
            if (tokenInfo.Payload.Actor != tokenInfo.Payload.Name &&
               tokenInfo.Payload.Actor != path) {
                throw new PermissionsException();
            }

            string fullPath = System.IO.Path.Combine(Startup.Configuration[Startup.RootDirectory], path);

            var provider = new FileExtensionContentTypeProvider();

            if (!provider.TryGetContentType(path, out string contentType)) {
                contentType = "application/octet-stream";
            }

            var file = FileEitity.FromPath(fullPath).FileInfo.Open(
                System.IO.FileMode.Open,
                System.IO.FileAccess.Read,
                System.IO.FileShare.Read);

            return File(file, contentType, System.IO.Path.GetFileName(path), true);
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

        private string BuildToken(FileEitity file) {
            var tokenModel = new DriveToken() {
                Header = new DefaultJwtHeader() {
                    Algorithm = SecurityAlgorithms.HmacSha256
                },
                Payload = new MvcIdentityPayload() {
                    Actor = file.RelativePath,
                    Issuer = Startup.Configuration.GetSection("JWT:Issuer").Value,
                    Audience = Startup.Configuration.GetSection("JWT:Audience").Value,
                    Name = file.RelativePath,
                    Role = DriveToken.Roles.Download,
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
