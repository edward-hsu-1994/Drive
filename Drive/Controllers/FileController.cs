using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Drive.Base.Mvc;
using Drive.FileSystem;
using Drive.Logic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Drive.Controllers {
    public class FileController : ManageBaseController {
        public FileController(DriveLogicManager manager) : base(manager) {

        }

        [AllowAnonymous]
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
                return x;
            });
        }
    }
}
