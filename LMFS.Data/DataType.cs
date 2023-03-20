using LibCLCC.NET.Collections;
using System.Text;

namespace LMFS.Data {
    public enum DataType {
        Default, Zip, Hash
    }

    public enum AuthResult
    {
        Success, UserNameNotFound,HashMismatch,NotEnabled
    }
}