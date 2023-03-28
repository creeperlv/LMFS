namespace LMFS.Core
{
    public class ServerConfiguration
    {
        public List<string> ListeningUrl = new List<string>();
        public Dictionary<string, string> PathMap = new Dictionary<string, string>();
        public string Template = null;
        public MIMEMap MimeMap = null;
        public bool UserAuth = false;
        public bool server_shell = false;
        public string userbase = null;
        public string authbase = null;
        public bool EnableDump = false;
        public bool EnableBrowse = true;
        public bool EnableGet = true;
        public bool EnableSet = true;
        public bool EnablePush= true;
        public bool EnableRM= true;
        public bool EnableMkdir= true;
    }
}