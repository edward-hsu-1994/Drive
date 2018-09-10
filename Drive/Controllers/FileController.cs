using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Drive.Base.Jwt;
using Drive.Base.Mvc;
using Drive.FileSystem;
using Drive.Logic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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

            var rootDirectory = DirectoryEntity.FromPath(Startup.Configuration[Startup.RootDirectory]);

            string fullPath = System.IO.Path.Combine(Startup.Configuration[Startup.RootDirectory], path);

            return DirectoryEntity.FromPath(fullPath).GetChildren().Select(x => {
                x.RelativePath = x.Path.Substring(rootDirectory.Path.Length);
                if (x is FileEntity file) {
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

            var file = FileEntity.FromPath(fullPath).FileInfo.Open(
                System.IO.FileMode.Open,
                System.IO.FileAccess.Read,
                System.IO.FileShare.Read);

            return File(file, contentType, System.IO.Path.GetFileName(path), true);
        }

        [HttpPost]
        [HttpPost("{*path}")]
        [DisableRequestSizeLimit]
        public IEnumerable<IFileSystemItem> Upload(string path) {
            if (string.IsNullOrWhiteSpace(path)) {
                path = "";
            }

            var rootDirectory = DirectoryEntity.FromPath(Startup.Configuration[Startup.RootDirectory]);

            string fullPath = System.IO.Path.Combine(Startup.Configuration[Startup.RootDirectory], path);

            DirectoryEntity targetDirectory = DirectoryEntity.FromPath(fullPath);

            return Request.Form.Files.Select(x => {
                return targetDirectory.CreateFile(x.FileName, x.OpenReadStream());
            });
        }

        [HttpPost("createChild/{*path}")]
        public IFileSystemItem CreateDirectory(string path) {
            if (string.IsNullOrWhiteSpace(path)) {
                path = "";
            }

            var rootDirectory = DirectoryEntity.FromPath(Startup.Configuration[Startup.RootDirectory]);

            string fullPath = System.IO.Path.Combine(Startup.Configuration[Startup.RootDirectory], path);

            return DirectoryEntity.FromDirectoryInfo(Directory.CreateDirectory(fullPath));
        }

        [HttpPut]
        public void Move(
            [FromQuery]string from,
            [FromQuery]string to) {
            if (string.IsNullOrWhiteSpace(from)) {
                from = "";
            }
            if (string.IsNullOrWhiteSpace(to)) {
                to = "";
            }

            var rootDirectory = DirectoryEntity.FromPath(Startup.Configuration[Startup.RootDirectory]);

            string fromFullPath = System.IO.Path.Combine(Startup.Configuration[Startup.RootDirectory], from);
            string toFullPath = System.IO.Path.Combine(Startup.Configuration[Startup.RootDirectory], to);

            // get the file attributes for file or directory
            FileAttributes fromAttr = System.IO.File.GetAttributes(fromFullPath);
            FileAttributes toAttr = System.IO.File.GetAttributes(toFullPath);

            if (toAttr != FileAttributes.Directory) {
                throw new ParameterException("目標路徑必須為目錄");
            }

            if (fromAttr == FileAttributes.Directory) {
                DirectoryEntity.FromPath(fromFullPath).Move(toFullPath);
            } else {
                FileEntity.FromPath(fromFullPath).Move(toFullPath);
            }
        }

        [HttpPut("delete")]
        public void Delete([FromBody]IEnumerable<string> paths) {
            foreach (var path in paths) {
                if (string.IsNullOrWhiteSpace(path)) {
                    continue; // 根目錄禁止刪除
                }

                string fullPath = System.IO.Path.Combine(Startup.Configuration[Startup.RootDirectory], path);

                FileAttributes attr = System.IO.File.GetAttributes(fullPath);

                if (attr == FileAttributes.Directory) {
                    DirectoryEntity.FromPath(fullPath).Delete();
                } else {
                    FileEntity.FromPath(fullPath).Delete();
                }
            }
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

        private string BuildToken(FileEntity file) {
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
