namespace LMFS.Exchange.Core.FileSystem
{
    public class LMFSFSFile : LMFSFSItem
    {
        public FileInfo file;
        public LMFSFSFile(FileInfo fileInfo)
        {
            this.file = fileInfo;
        }

        public string Name => file.Name;

        public static implicit operator LMFSFSFile(FileInfo fi)
        {
            return new LMFSFSFile(fi);
        }
    }
    public class LMFSFSItem
    {
        public ItemAttr Attribute;
        public LMFSFSItem Parent;
    }
}