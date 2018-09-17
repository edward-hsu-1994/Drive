using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Drive.Base.Jwt;
using Drive.Base.Mvc;
using Drive.FileSystem;
using Drive.Logic;
using Drive.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.IdentityModel.Tokens;
using XWidget.Extensions;
using XWidget.Linq;
using XWidget.Web.Exceptions;
using XWidget.Web.Jwt;

namespace Drive.Controllers {
    public class FileController : BaseController {
        public FileController(DriveLogicManager manager) : base(manager) {

        }

        /// <summary>
        /// 取得指令路徑下的檔案列表
        /// </summary>
        /// <param name="path">路徑</param>
        /// <param name="type">類型過濾</param>
        /// <param name="q">搜尋關鍵字，遞迴搜尋</param>
        /// <param name="skip">起始索引</param>
        /// <param name="take">取得筆數</param>
        /// <returns>檔案列表分頁結果</returns>
        [HttpGet]
        [HttpGet("{*path}")]
        public PagingWithUrl<IFileSystemItem> List(
            string path,
            FileSystemItemType? type,
            string q,
            int skip = 0,
            int take = 10) {
            if (string.IsNullOrWhiteSpace(path)) {
                path = "";
            }

            var rootDirectory = DirectoryEntity.FromPath(Startup.Configuration[Startup.RootDirectory]);

            string fullPath = System.IO.Path.Combine(Startup.Configuration[Startup.RootDirectory], path);

            IEnumerable<IFileSystemItem> fullResult;
            if (!string.IsNullOrWhiteSpace(q)) {
                fullResult = DirectoryEntity.FromPath(fullPath).Search(q);
            } else {
                fullResult = DirectoryEntity.FromPath(fullPath).GetChildren();
            }

            fullResult = fullResult.OrderBy(x => x.Type) // 目錄優先
                    .Where(x => !type.HasValue || x.Type == type.Value) // 類型過濾
                    .Select(x => {
                        x.RelativePath = x.Path.Substring(rootDirectory.Path.Length).Replace('\\', '/');
                        if (x is FileEntity file) {
                            file.DownloadUrl = $"{Request.Scheme}://{Request.Host}/api/File/download?path={Uri.EscapeDataString(x.RelativePath)}&token={Uri.EscapeDataString(BuildToken(file))}";
                        }
                        return x;
                    }); ;

            return new PagingWithUrl<IFileSystemItem>(fullResult, skip, take).Process(x => {
                var builder = new UriBuilder(Request.GetDisplayUrl());

                var queryBuilder = new QueryBuilder();
                if (type.HasValue) {
                    queryBuilder.Add("type", type.Value.ToString());
                }
                queryBuilder.Add("skip", (skip + take).ToString());
                queryBuilder.Add("take", take.ToString());

                builder.Query = queryBuilder.ToString();

                x.Next = builder.ToString();
            });
        }

        /// <summary>
        /// 下載指定路徑檔案
        /// </summary>
        /// <param name="path">路徑</param>
        /// <param name="token">存取權杖</param>
        /// <returns>檔案流</returns>
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

            var fileEntity = FileEntity.FromPath(fullPath);

            var file = fileEntity.FileInfo.Open(
                System.IO.FileMode.Open,
                System.IO.FileAccess.Read,
                System.IO.FileShare.Read);

            return File(file, fileEntity.ContentType, System.IO.Path.GetFileName(path), true);
        }

        /// <summary>
        /// 在指定路徑上傳檔案
        /// </summary>
        /// <param name="path">路徑</param>
        /// <param name="files">檔案集合</param>
        /// <returns>上傳結果</returns>
        [HttpPost]
        [HttpPost("{*path}")]
        [DisableRequestSizeLimit]
        public IEnumerable<IFileSystemItem> Upload(string path, [FromForm]IFormFileCollection files) {
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

        /// <summary>
        /// 在指定路徑建立目錄
        /// </summary>
        /// <param name="path">路徑</param>
        /// <returns>建立結果</returns>
        [HttpPost("createChild/{*path}")]
        public IFileSystemItem CreateDirectory(string path) {
            if (string.IsNullOrWhiteSpace(path)) {
                path = "";
            }

            var rootDirectory = DirectoryEntity.FromPath(Startup.Configuration[Startup.RootDirectory]);

            string fullPath = System.IO.Path.Combine(Startup.Configuration[Startup.RootDirectory], path);

            return DirectoryEntity.FromDirectoryInfo(Directory.CreateDirectory(fullPath));
        }

        /// <summary>
        /// 搬移檔案或目錄
        /// </summary>
        /// <param name="from">來源路徑</param>
        /// <param name="to">目標路徑</param>
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

            if (fromAttr == FileAttributes.Directory) {
                DirectoryEntity.FromPath(fromFullPath).Move(toFullPath);
            } else {
                FileEntity.FromPath(fromFullPath).Move(toFullPath);
            }
        }

        /// <summary>
        /// 刪除指定路徑的檔案或目錄
        /// </summary>
        /// <param name="paths">路徑集合</param>
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
