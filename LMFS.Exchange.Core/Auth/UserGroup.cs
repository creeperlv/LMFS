namespace LMFS.Exchange.Core.Auth {
    [Serializable]
    public class UserGroup {
        public string Name;
        public bool CanRead;
        public bool CanWrite;
        public bool CanDelete;
        public bool CanDump;
    }
    [Serializable]
    public class RTUser {
        public string Name;
        public bool CanRead;
        public bool CanWrite;
        public bool CanDelete;
        public bool CanDump;
    }
    [Serializable]

    public class User {
        public string Name;
        public List<string> Groups = new List<string>();
    }
}