using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace Drive.FileSystem {
    public class DirectoryEitity : IFileSystemItem {
        [JsonIgnore]
        public DirectoryInfo DirectoryInfo { get; set; }

        public string Type => "Directory";

        private DirectoryEitity() { }

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
                return DirectoryInfo.FullName;
            }
            set {
                DirectoryInfo.MoveTo(value);
                DirectoryInfo = new DirectoryInfo(value);
            }
        }

        public string Name {
            get {
                return DirectoryInfo.Name;
            }
            set {
                var targetPath = System.IO.Path.Combine(this.DirectoryInfo.Parent.FullName, value);
                DirectoryInfo.MoveTo(targetPath);
                this.DirectoryInfo = new DirectoryInfo(targetPath);
            }
        }

        public long Size {
            get {
                long DirSize(DirectoryInfo d) {
                    long size = 0;
                    // Add file sizes.
                    FileInfo[] fis = d.GetFiles();
                    foreach (FileInfo fi in fis) {
                        size += fi.Length;
                    }
                    // Add subdirectory sizes.
                    DirectoryInfo[] dis = d.GetDirectories();
                    foreach (DirectoryInfo di in dis) {
                        size += DirSize(di);
                    }
                    return size;
                }
                return DirSize(this.DirectoryInfo);
            }
        }

        public DateTime CreationTime => DirectoryInfo.CreationTime;

        public DateTime ModifyTime => DirectoryInfo.LastWriteTime;

        public DateTime AccessTime => DirectoryInfo.LastAccessTime;

        public void Delete() {
            Directory.Delete(this.Path, true);
            DirectoryInfo = null;
        }

        public IFileSystemItem GetParent(int level = 1) {
            if (level == 0) {
                return this;
            } else {
                return FromDirectoryInfo(this.DirectoryInfo.Parent).GetParent(level - 1);
            }
        }

        public IFileSystemItem[] GetChildren() {
            var files = DirectoryInfo.GetFiles().Select(x => (IFileSystemItem)FileEitity.FromFileInfo(x));
            var directories = DirectoryInfo.GetDirectories().Select(x => (IFileSystemItem)DirectoryEitity.FromDirectoryInfo(x));

            return directories.Concat(files).ToArray();
        }

        public IFileSystemItem[] Search(string patten) {
            var files = DirectoryInfo.GetFiles(patten, SearchOption.AllDirectories).Select(x => (IFileSystemItem)FileEitity.FromFileInfo(x));
            var directories = DirectoryInfo.GetDirectories(patten, SearchOption.AllDirectories).Select(x => (IFileSystemItem)DirectoryEitity.FromDirectoryInfo(x));

            return directories.Concat(directories).ToArray();
        }

        public void Move(string targetPath) {
            Directory.Move(this.Path, targetPath);
            DirectoryInfo = new DirectoryInfo(targetPath);
        }

        public void MoveTo(IFileSystemItem target) {
            Move(target.Path);
        }

        public DirectoryEitity CreateSubdirectory(string name) {
            return FromDirectoryInfo(DirectoryInfo.CreateSubdirectory(System.IO.Path.Combine(Path, name)));
        }

        public void CreateFile(string filename, Stream stream) {
            using (var newFile = File.Create(System.IO.Path.Combine(Path, filename))) {
                stream.CopyTo(newFile);
            }
        }

        public static DirectoryEitity FromPath(string path) {
            return FromDirectoryInfo(new DirectoryInfo(path));
        }

        public static DirectoryEitity FromDirectoryInfo(DirectoryInfo directory) {
            var result = new DirectoryEitity();
            result.DirectoryInfo = directory;
            return result;
        }
    }
}
