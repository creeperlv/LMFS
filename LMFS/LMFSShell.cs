using LibCLCC.NET.TextProcessing;
using LMFS.Extensible;
using LMFS.Server.Core;
using System.Reflection;

namespace LMFS {
    internal static class LMFSShell {
        static CommandLineParser CommandLineParser;
        static Dictionary<string, FunctionBase> functions = new Dictionary<string, FunctionBase>();
        internal static void InputListen() {
            LMFSShellCore lMFSShellCore = new LMFSShellCore();

            lMFSShellCore.InputListen();
        }
    }
}