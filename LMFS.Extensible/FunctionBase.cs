using System.IO;

namespace LMFS.Extensible {
    public class FunctionBase : IFunction {
        public LMFSConsole LMFSConsole;
        public virtual void Dispose() {
        }

        public virtual void Run(params string[] args) {
        }
    }
    public class LMFSConsole {
        public TextWriter STDOUT;
        public TextReader STDIN;
    }
}
