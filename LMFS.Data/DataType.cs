using LibCLCC.NET.Collections;
using System.Text;

namespace LMFS.Data
{
    public enum DataType
    {
        Default, Zip, Hash, WriteTime
    }

    public enum AuthResult
    {
        Success, UserNameNotFound, HashMismatch, NotEnabled
    }
}