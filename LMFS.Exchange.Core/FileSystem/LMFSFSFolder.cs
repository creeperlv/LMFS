using LMFS.Data;
using Newtonsoft.Json;

namespace LMFS.Exchange.Core.FileSystem
{
    public class LMFSFSFolder : LMFSFSItem
    {
        public LMFSFolder folder;
        public string Name => folder.Name;
        public bool IsDirExist(string path)
        {
            return folder.IsDirExist(path);
        }
        public bool IsFileExist(string path)
        {
            return folder.IsFileExist(path);
        }
        public LMFSFSFolder(string folder)
        {
            this.folder = LMFSFolder.FromDirectoryPath(folder);
            Attribute = new ItemAttr();
            var __attr = JsonConvert.DeserializeObject<ItemAttr>(File.ReadAllText(Path.Combine(this.folder.Folder.FullName, ".lmfs-folder-attr")));
            if (__attr != null) Attribute = __attr;
        }
        public LMFSFSFolder(LMFSFolder folder)
        {
            this.folder = folder;
            Attribute = new ItemAttr();
            var __attr = JsonConvert.DeserializeObject<ItemAttr>(File.ReadAllText(Path.Combine(this.folder.Folder.FullName, ".lmfs-folder-attr")));
            if (__attr != null) Attribute = __attr;
        }
        public LMFSFSFile SubGetFile(string path)
        {
            return folder.SubGetFile(path);
        }
        public LMFSFSFolderGetCode GetFolder(string relative_path, out LMFSFSFolder folder)
        {
            folder = this.folder.GetSubFolder(relative_path);
            return LMFSFSFolderGetCode.not_found;
        }

        public List<LMFSFSFolder> GetFolders()
        {
            return folder.GetFolders().Cast<LMFSFSFolder>().ToList();
        }
        public List<LMFSFSFile> GetFiles()
        {
            return folder.GetFiles().Cast<LMFSFSFile>().ToList();
        }
        public static implicit operator LMFSFSFolder(LMFSFolder folder)
        {
            return new LMFSFSFolder(folder);
        }
    }
}