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
    [LMFSFunction("cd")]
    public class ChangeDirectory : FunctionBase {
        public override void Dispose() {
        }

        public override void Run(params string[] args) {
            if (args.Length == 0) {
                LMFSExtensibleEnv.CurrentDirectory = Environment.CurrentDirectory;
            }
            else {
                var DI = new DirectoryInfo(Path.Combine(LMFSExtensibleEnv.CurrentDirectory, args[0]));
                if (DI.Exists) {
                    LMFSExtensibleEnv.CurrentDirectory = DI.FullName;
                }
                else if (Directory.Exists(args[0])) {
                    DI = new DirectoryInfo(args[0]);
                    LMFSExtensibleEnv.CurrentDirectory = DI.FullName;
                }
            }

        }
    }
    [LMFSFunction("ls")]
    public class List : FunctionBase {
        public override void Dispose() {
        }

        public override void Run(params string[] args) {
            List<string> directories = new List<string>();
            bool IsList = false;
            {
                var DI = new DirectoryInfo(LMFSExtensibleEnv.CurrentDirectory);
                foreach (var item in args) {
                    if (item.StartsWith('-')) {
                        foreach (var _switch in item) {
                            switch (_switch) {
                                case 'l':
                                    IsList = true;
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                    else {
                        var _DI = new DirectoryInfo(Path.Combine(LMFSExtensibleEnv.CurrentDirectory, item));
                        if (_DI.Exists) {
                            LMFSExtensibleEnv.CurrentDirectory = _DI.FullName;
                            directories.Add(_DI.FullName);
                        }
                        else if (Directory.Exists(args[0])) {
                            _DI = new DirectoryInfo(args[0]);
                            LMFSExtensibleEnv.CurrentDirectory = _DI.FullName;
                            directories.Add(_DI.FullName);
                        }
                        else {
                            LMFSConsole.STDOUT.WriteLine($"{item} is not a directory or not found.");
                        }
                    }
                }
                if (directories.Count == 0) {
                    directories.Add(DI.FullName);
                }
            }
            {
                foreach (var d in directories) {
                    DirectoryInfo DI = new DirectoryInfo(d);
                    LMFSConsole.STDOUT.WriteLine($"{DI.FullName}:");
                    foreach (var item in DI.EnumerateDirectories()) {
                        if (IsList) {
                            LMFSConsole.STDOUT.WriteLine($"dwru\t{item.LastWriteTime}\t" + item.Name + "/");
                        }
                        else {
                            LMFSConsole.STDOUT.Write(item.Name);
                            LMFSConsole.STDOUT.Write("\t");

                        }
                    }
                    foreach (var item in DI.EnumerateFiles()) {
                        if (IsList) {
                            LMFSConsole.STDOUT.WriteLine($"dwru\t{item.LastWriteTime}\t" + item.Name);
                            //LMFSConsole.STDOUT.WriteLine(item.Name);
                        }
                        else {
                            LMFSConsole.STDOUT.Write(item.Name);
                            LMFSConsole.STDOUT.Write("\t");

                        }
                    }
                }
            }
            LMFSConsole.STDOUT.WriteLine();
        }
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
    [LMFSFunction("clear")]
    public class Clear : FunctionBase {
        public override void Dispose() {
        }

        public override void Run(params string[] args) {
            LMFSConsole.STDOUT.Write("\u001b[2J");
            LMFSConsole.STDOUT.Write("\u001b[100A");
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
