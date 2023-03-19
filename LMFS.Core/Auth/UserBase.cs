using LMFS.Exchange.Core.Auth;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace LMFS.Server.Core.Auth {
    [Serializable]
    public class AuthBase {
        public Dictionary<string, UserCredential> UserCertifications = new Dictionary<string, UserCredential>();
        public DirectoryInfo AuthBaseFolder;
        public static AuthBase LoadFromFolder(DirectoryInfo directoryInfo) {
            AuthBase authBase = new AuthBase();
            authBase.AuthBaseFolder = directoryInfo;
            return authBase;
        }
        public void Init() {
            var auths = AuthBaseFolder.EnumerateFiles("*.auth");
            foreach (var item in auths) {
                var uc = JsonConvert.DeserializeObject<UserCredential>(File.ReadAllText(item.FullName));
                UserCertifications.Add(uc.Name, uc);
            }
        }

    }
    [Serializable]
    public class UserCredential : IEqualityComparer<UserCredential> {
        public string Name;
        public string Hash;
        public static UserCredential FromPW(string name, string pw) {
            using (SHA256 _sha = SHA256.Create()) {
                var hash = _sha.ComputeHash(Encoding.UTF8.GetBytes($"{name},{pw}"));
                var hash_str = Convert.ToBase64String(hash);
                return new UserCredential {
                    Name = name,
                    Hash = hash_str
                };
            }
        }
        public bool Equals(UserCredential x, UserCredential y) {
            return x.Name == y.Name;
        }

        public int GetHashCode([DisallowNull] UserCredential obj) {
            return obj.Name.GetHashCode();
        }
    }
    [Serializable]
    public class UserBase {
        [NonSerialized]
        public List<UserGroup> InternalGroups;
        public List<UserGroup> Groups = null;
        public List<User> Users = null;
        public UserBase() {
            InternalGroups = new List<UserGroup> {
                new UserGroup { Name = "everyone", CanDelete = false, CanDump = false, CanRead = true, CanWrite = true },
                new UserGroup { Name = "root", CanDelete = true, CanDump = true, CanRead = true, CanWrite = true }
            };
        }
        public void FillInitData() {
            if (Groups == null)
                Groups = new List<UserGroup>();
            if (Users == null)
                Users = new List<User>();
            Groups.Add(new UserGroup { Name = "contri", CanDelete = false, CanWrite = true, CanRead = true, CanDump = true });
            Groups.Add(new UserGroup { Name = "view", CanDelete = false, CanWrite = false, CanRead = true, CanDump = true });
            Groups.Add(new UserGroup { Name = "reader", CanDelete = false, CanWrite = false, CanRead = true, CanDump = false });
            Users.Add(new User { Name = "root", Groups = new List<string> { "root" } });
            Users.Add(new User { Name = "admin", Groups = new List<string> { "root" } });
            Users.Add(new User { Name = "guest", Groups = new List<string> { "reader" } });
        }
    }
}
