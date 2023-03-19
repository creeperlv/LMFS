using Ignore;
using Newtonsoft.Json;

namespace LMFS.Data {
    /// <summary>
    /// .lmfs.folder
    /// </summary>
    [Serializable]
    public class LMFSFolder {
        public bool UseGitIgnore;
        [NonSerialized]
        public DirectoryInfo Folder;
        RuntimeIgnore? GitIgnore = null;
        public static LMFSFolder? FromDirectoryPath(string path) {
            var lmfsFolder = new LMFSFolder { Folder = new DirectoryInfo(path) };
            if (Directory.Exists(path)) {
                return lmfsFolder;
            }
            else {
                var p = Path.Combine(path, ".lmfs.folder");
                if (File.Exists(p)) {
                    return FromConfig(new FileInfo(p)) ?? lmfsFolder;
                }
            }
            return null;
        }
        public List<LMFSFolder> GetFolders() {
            var f = Folder.GetDirectories();
            List<LMFSFolder> lMFSFolders = new List<LMFSFolder>();
            foreach (var item in f) {
                if (UseGitIgnore) {
                    if (GitIgnore != null) {
                        var rp = Path.GetRelativePath(GitIgnore.BasePath, item.FullName);
                        if (GitIgnore.Ignore.IsIgnored(rp)) {
                            continue;
                        }
                    }
                }
                lMFSFolders.Add(new LMFSFolder { Folder = item, GitIgnore = GitIgnore, UseGitIgnore = UseGitIgnore });
            }
            return lMFSFolders;
        }
        public List<FileInfo> GetFiles() {
            var f = Folder.GetFiles();
            List<FileInfo> lMFSFolders = new List<FileInfo>();
            foreach (var item in f) {
                if (UseGitIgnore) {
                    if (GitIgnore != null) {
                        var rp = Path.GetRelativePath(GitIgnore.BasePath, item.FullName);
                        if (GitIgnore.Ignore.IsIgnored(rp)) {
                            continue;
                        }
                    }
                }
                lMFSFolders.Add(item);
            }
            return lMFSFolders;
        }
        public static LMFSFolder? FromConfig(FileInfo config) {
            LMFSFolder? result = JsonConvert.DeserializeObject<LMFSFolder>(File.ReadAllText(config.FullName));
            if (result == null) { return result; }
            var d = config.Directory;
            if (d != null) {
                if (result.UseGitIgnore) {
                    var ignore_path = Path.Combine(d.FullName, ".gitignore");
                    if (File.Exists(ignore_path))
                        result.GitIgnore = RuntimeIgnore.FromGitIgnore(ignore_path, d);
                }
                result.Folder = d;
                return result;
            }
            return null;
        }
    }
    public class RuntimeIgnore {
        public Ignore.Ignore Ignore;
        public string BasePath;
        public static RuntimeIgnore FromGitIgnore(string gitignore, DirectoryInfo Base) {
            var l = File.ReadAllLines(gitignore).ToList();
            l.RemoveAll((x) => {
                var a = x.Trim();
                if (a == "") return true;
                if (a.StartsWith("#")) return true;
                return false;
            });
            RuntimeIgnore runtimeIgnore = new RuntimeIgnore();
            runtimeIgnore.BasePath = Base.FullName;
            runtimeIgnore.Ignore = new Ignore.Ignore(); ;
            runtimeIgnore.Ignore.Add(l);
            return runtimeIgnore;
        }
    }
}