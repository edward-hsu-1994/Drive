using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.AspNetCore.StaticFiles;
using Newtonsoft.Json;

namespace Drive.FileSystem {
    public class FileEntity : IFileSystemItem {
        [JsonIgnore]
        public FileInfo FileInfo { get; set; }

        public FileSystemItemType Type => FileSystemItemType.File;

        public string ContentType {
            get {
                var provider = new FileExtensionContentTypeProvider();
                if (provider.TryGetContentType(FileInfo.FullName, out string contentType)) {
                    return contentType;
                }
                return "application/octet-stream";
            }
        }

        private FileEntity() { }

        private string _RelativePath;
        public string RelativePath {
            get {
                return this._RelativePath ?? this.Name;
            }
            set {
                this._RelativePath = value;
            }
        }

        public string Path {
            get {
                return FileInfo.FullName;
            }
            set {
                FileInfo.MoveTo(value);
                FileInfo = new FileInfo(value);
            }
        }

        public string DownloadUrl { get; set; }

        public string Name {
            get {
                return FileInfo.Name;
            }
            set {
                var targetPath = System.IO.Path.Combine(this.FileInfo.Directory.FullName, value);
                FileInfo.MoveTo(targetPath);
                this.FileInfo = new FileInfo(targetPath);
            }
        }

        public long Size => FileInfo.Length;

        public DateTime CreationTime => FileInfo.CreationTime;

        public DateTime ModifyTime => FileInfo.LastWriteTime;

        public DateTime AccessTime => FileInfo.LastAccessTime;

        public void Delete() {
            File.Delete(this.Path);
            FileInfo = null;
        }

        public IFileSystemItem GetParent(int level = 1) {
            if (level == 0) {
                return this;
            } else {
                return DirectoryEntity.FromDirectoryInfo(this.FileInfo.Directory).GetParent(level - 1);
            }
        }

        public void Move(string targetPath) {
            File.Move(this.Path, targetPath);
            FileInfo = new FileInfo(targetPath);
        }

        public void MoveTo(IFileSystemItem target) {
            if (target is DirectoryEntity) {
                Move(System.IO.Path.Combine(target.Path, Name));
            } else if (target is FileEntity) {
                Move(target.Path);
            }
        }

        public static FileEntity FromPath(string path) {
            return FromFileInfo(new FileInfo(path));
        }

        public static FileEntity FromFileInfo(FileInfo file) {
            var result = new FileEntity();
            result.FileInfo = file;
            return result;
        }
    }
}
