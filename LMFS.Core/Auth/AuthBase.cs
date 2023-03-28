using Newtonsoft.Json;
using System.Net;

namespace LMFS.Server.Core.Auth
{
    [Serializable]
    public class AuthBase
    {

        public Dictionary<string, UserCredential> UserCredentials = new Dictionary<string, UserCredential>();
        public DirectoryInfo AuthBaseFolder;
        public static AuthBase LoadFromFolder(DirectoryInfo directoryInfo)
        {
            AuthBase authBase = new AuthBase();
            authBase.AuthBaseFolder = directoryInfo;
            return authBase;
        }
        public void Init()
        {
            var auths = AuthBaseFolder.EnumerateFiles("*.auth");
            foreach (var item in auths)
            {
                var uc = JsonConvert.DeserializeObject<UserCredential>(File.ReadAllText(item.FullName));
                UserCredentials.Add(uc.Name, uc);
            }
        }


    }
}
