namespace LMFS.Exchange.Core.FileSystem
{
    public class LMFSFSFile {
        public FileInfo file;
        public ItemAttr Attribute;
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
}