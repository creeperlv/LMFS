using LMFS.Data.Utilities;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
using System.Text;

namespace LMFS.Server.Core.Auth
{
    [Serializable]
    public class UserCredential : IEqualityComparer<UserCredential>,IEquatable<UserCredential>
    {
        public string Name;
        public string Hash;
        public static UserCredential FromPW(string name, string pw)
        {
            return new UserCredential
            {
                Name = name,
                Hash = $"{name},{pw}".HashString()
            };
        }
        public bool Equals(UserCredential x, UserCredential y)
        {
            return x.Name == y.Name;
        }

        public bool Equals(UserCredential other)
        {
            return Name == other.Name;
        }

        public int GetHashCode([DisallowNull] UserCredential obj)
        {
            return obj.Name.GetHashCode();
        }
    }
}
