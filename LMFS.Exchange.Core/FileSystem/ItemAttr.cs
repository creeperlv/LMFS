using LMFS.Data;
using LMFS.Exchange.Core.Auth;

namespace LMFS.Exchange.Core.FileSystem {
    /// <summary>
    /// Assume file: .lmfs-folder-attr || *.lmfs-attr
    /// </summary>
    [Serializable]
    public class ItemAttr {
        public string Owner;
        public List<UserGroup> Groups = new List<UserGroup>();
        public List<RTUser> User = new List<RTUser>();
    }
    public class LMFSFSFolder {
        public LMFSFolder folder;
        public ItemAttr Attribute;
        public LMFSFSFolder(string folder) {
            this.folder=LMFSFolder.FromDirectoryPath(folder);
        }
        public LMFSFSFolderGetCode GetFolder(string relative_path,out LMFSFolder folder) {

            folder=null;
            return LMFSFSFolderGetCode.not_found;
        }
    }
    public enum LMFSFSFolderGetCode {
        done,out_of_boundary,not_found
    }
    public class LMFSFSFile {
        public FileInfo fileInfo;
        public ItemAttr Attribute;
    }
}