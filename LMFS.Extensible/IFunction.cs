using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMFS.Extensible {
    public interface IFunction : IDisposable {
        void Run(params string[] args);
    }
    [LMFSFunction("adduser")]
    public class AddUser : FunctionBase {
        public override void Dispose() {
            throw new NotImplementedException();
        }

        public override void Run(params string[] args) {
            throw new NotImplementedException();
        }
    }
    [LMFSFunction("addgroup")]
    public class AddGroup : FunctionBase {
        public override void Dispose() {
            throw new NotImplementedException();
        }

        public override void Run(params string[] args) {
            throw new NotImplementedException();
        }
    }
    [LMFSFunction("chmod")]
    public class Chmod : FunctionBase {
        public override void Dispose() {
            throw new NotImplementedException();
        }

        public override void Run(params string[] args) {
            throw new NotImplementedException();
        }
    }

}
