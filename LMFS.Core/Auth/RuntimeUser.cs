using System.Diagnostics.CodeAnalysis;

namespace LMFS.Server.Core.Auth
{
    [Serializable]
    public class RuntimeUser : IEqualityComparer<RuntimeUser>, IEquatable<RuntimeUser>
    {
        public string Name;
        public List<Guid> IDs = new List<Guid>();

        public bool Equals(RuntimeUser x, RuntimeUser y)
        {
            return x.Name == y.Name;
        }

        public bool Equals(RuntimeUser other)
        {
            return Name.Equals(other.Name);
        }

        public int GetHashCode([DisallowNull] RuntimeUser obj)
        {
            return obj.Name.GetHashCode();
        }
    }
}
