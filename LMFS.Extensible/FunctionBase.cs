using System.IO;

namespace LMFS.Extensible {
    public class FunctionBase : IFunction {
        public virtual void Dispose() {
        }

        public virtual void Run(params string[] args) {
        }
    }
    public static class LMFSConsole {
        public static TextWriter STDOUT;
        public static TextReader STDIN;
    }
}
