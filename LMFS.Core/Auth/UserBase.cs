using LMFS.Exchange.Core.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LMFS.Server.Core.Auth
{
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
