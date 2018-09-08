using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Drive.FileSystem {
    public class FileEitity : IFileSystemItem {
        public FileInfo FileInfo { get; set; }

        private FileEitity() { }

        public string Path {
            get {
                return FileInfo.FullName;
            }
            set {
                FileInfo.MoveTo(value);
                FileInfo = new FileInfo(value);
            }
        }

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
                return DirectoryEitity.FromDirectoryInfo(this.FileInfo.Directory).GetParent(level - 1);
            }
        }

        public void Move(string targetPath) {
            File.Move(this.Path, targetPath);
            FileInfo = new FileInfo(targetPath);
        }

        public void MoveTo(IFileSystemItem target) {
            if (target is DirectoryEitity) {
                Move(System.IO.Path.Combine(target.Path, Name));
            } else if (target is FileEitity) {
                Move(target.Path);
            }
        }

        public static FileEitity FromPath(string path) {
            return FromFileInfo(new FileInfo(path));
        }

        public static FileEitity FromFileInfo(FileInfo file) {
            var result = new FileEitity();
            result.FileInfo = file;
            return result;
        }
    }
}
