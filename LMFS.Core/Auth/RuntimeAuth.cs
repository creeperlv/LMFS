using LMFS.Data;
using LMFS.Data.Utilities;
using Newtonsoft.Json;

namespace LMFS.Server.Core.Auth
{
    [Serializable]
    public class RuntimeAuth
    {
        [NonSerialized]
        public AuthBase AuthBase = null;
        public List<Guid> LoggedIns = new List<Guid>();
        public Dictionary<string, RuntimeUser> Connections = new Dictionary<string, RuntimeUser>();
        public bool IsPresent(Guid guid)
        {
            if (AuthBase == null) return true;
            return LoggedIns.Contains(guid);
        }
        public DirectoryInfo rtauth;
        public void Load()
        {
            var rts = rtauth.EnumerateFiles("*.rtauth");
            foreach (var item in rts)
            {
                var _ru = JsonConvert.DeserializeObject<RuntimeUser>(File.ReadAllText(item.FullName));
                Connections.Add(_ru.Name, _ru);
                foreach (var id in _ru.IDs)
                {
                    LoggedIns.Add(id);
                }
            }
        }
        public void Save()
        {
            var rts = rtauth.EnumerateFiles("*.rtauth").ToList();
            foreach (var item in Connections)
            {
                for (int i = 0; i < rts.Count; i++)
                {
                    var file = rts[i];

                    if (file.Name == item.Key + ".rtauth")
                    {
                        rts.RemoveAt(i);
                        break;
                    }
                }
                FileInfo fileInfo = new FileInfo(Path.Combine(rtauth.FullName, item.Key + ".rtauth"));
                if (fileInfo.Exists) fileInfo.Delete();
                File.WriteAllText(fileInfo.FullName, JsonConvert.SerializeObject(item, Formatting.Indented));
            }
            foreach (var item in rts)
            {
                item.Delete();
            }
        }
        public AuthResult NewLogin(string name, string hash, out Guid ID)
        {
            if (AuthBase == null)
            {
                ID = Guid.Empty;
                return AuthResult.NotEnabled;
            }
            if (AuthBase.UserCredentials.ContainsKey(name))
            {
                var uc = AuthBase.UserCredentials[name];
                if (uc.Hash.AddSalt().HashString() == hash)
                {
                    ID = Guid.NewGuid();
                    LoggedIns.Add(ID);


                }
                else
                {
                    ID = Guid.Empty;
                    if (!Connections.ContainsKey(uc.Name))
                    {
                        Connections.Add(uc.Name, new RuntimeUser { Name = uc.Name, IDs = new List<Guid>() });
                    }
                    Connections[uc.Name].IDs.Add(ID);
                    return AuthResult.HashMismatch;
                }
            }
            ID = Guid.Empty;
            return AuthResult.UserNameNotFound;
        }
    }
}
