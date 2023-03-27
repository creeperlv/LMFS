using LMFS.Exchange.Core.Auth;

namespace LMFS.Exchange.Core.FileSystem
{
    /// <summary>
    /// Assume file: .lmfs-folder-attr || *.lmfs-attr
    /// </summary>
    [Serializable]
    public class ItemAttr {
        public string Owner;
        public List<UserGroup> Groups = new List<UserGroup>();
        public List<RTUser> User = new List<RTUser>();
    }
    public enum LMFSFSFolderGetCode {
        done,out_of_boundary,not_found
    }
}