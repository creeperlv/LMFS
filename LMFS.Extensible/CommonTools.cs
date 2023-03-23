//////////////////////////////////////
//This file contains Unix-Like tools//
//////////////////////////////////////

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace LMFS.Extensible
{

    [LMFSFunction("cd")]
    public class ChangeDirectory : FunctionBase
    {
        public override void Dispose()
        {
        }

        public override void Run(params string[] args)
        {
            if (args.Length == 0)
            {
                LMFSExtensibleEnv.CurrentDirectory = Environment.CurrentDirectory;
            }
            else
            {
                var DI = new DirectoryInfo(Path.Combine(LMFSExtensibleEnv.CurrentDirectory, args[0]));
                if (DI.Exists)
                {
                    LMFSExtensibleEnv.CurrentDirectory = DI.FullName;
                }
                else if (Directory.Exists(args[0]))
                {
                    DI = new DirectoryInfo(args[0]);
                    LMFSExtensibleEnv.CurrentDirectory = DI.FullName;
                }
            }

        }
    }
    [LMFSFunction("ls")]
    public class List : FunctionBase
    {
        public override void Dispose()
        {
        }

        public override void Run(params string[] args)
        {
            List<string> directories = new List<string>();
            bool IsList = false;
            {
                var DI = new DirectoryInfo(LMFSExtensibleEnv.CurrentDirectory);
                foreach (var item in args)
                {
                    if (item.StartsWith('-'))
                    {
                        foreach (var _switch in item)
                        {
                            switch (_switch)
                            {
                                case 'l':
                                    IsList = true;
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                    else
                    {
                        var _DI = new DirectoryInfo(Path.Combine(LMFSExtensibleEnv.CurrentDirectory, item));
                        if (_DI.Exists)
                        {
                            LMFSExtensibleEnv.CurrentDirectory = _DI.FullName;
                            directories.Add(_DI.FullName);
                        }
                        else if (Directory.Exists(args[0]))
                        {
                            _DI = new DirectoryInfo(args[0]);
                            LMFSExtensibleEnv.CurrentDirectory = _DI.FullName;
                            directories.Add(_DI.FullName);
                        }
                        else
                        {
                            LMFSConsole.STDOUT.WriteLine($"{item} is not a directory or not found.");
                        }
                    }
                }
                if (directories.Count == 0)
                {
                    directories.Add(DI.FullName);
                }
            }
            {
                foreach (var d in directories)
                {
                    DirectoryInfo DI = new DirectoryInfo(d);
                    LMFSConsole.STDOUT.WriteLine($"{DI.FullName}:");
                    foreach (var item in DI.EnumerateDirectories())
                    {
                        if (IsList)
                        {
                            LMFSConsole.STDOUT.WriteLine($"dwru\t{item.LastWriteTime}\t" + item.Name + "/");
                        }
                        else
                        {
                            LMFSConsole.STDOUT.Write(item.Name);
                            LMFSConsole.STDOUT.Write("\t");

                        }
                    }
                    foreach (var item in DI.EnumerateFiles())
                    {
                        if (IsList)
                        {
                            LMFSConsole.STDOUT.WriteLine($"dwru\t{item.LastWriteTime}\t" + item.Name);
                            //LMFSConsole.STDOUT.WriteLine(item.Name);
                        }
                        else
                        {
                            LMFSConsole.STDOUT.Write(item.Name);
                            LMFSConsole.STDOUT.Write("\t");

                        }
                    }
                }
            }
            LMFSConsole.STDOUT.WriteLine();
        }
    }
    [LMFSFunction("clear")]
    public class Clear : FunctionBase
    {
        public override void Dispose()
        {
        }

        public override void Run(params string[] args)
        {
            LMFSConsole.STDOUT.Write("\u001b[2J");
            LMFSConsole.STDOUT.Write("\u001b[100A");
        }
    }

    [LMFSFunction("cat")]
    public class ViewFile : FunctionBase
    {
        public override void Dispose()
        {
        }

        public override void Run(params string[] args)
        {
            foreach (var item in args)
            {
                LMFSConsole.STDOUT.WriteLine(File.ReadAllText(Path.Combine(LMFSExtensibleEnv.CurrentDirectory, item)));
            }
        }
    }
    [LMFSFunction("export")]
    public class Export : FunctionBase
    {
        public override void Dispose()
        {
        }

        public override void Run(params string[] args)
        {
            if (args.Length == 0)
            {
                foreach (DictionaryEntry item in LMFSExtensibleEnv.Environments)
                {

                    LMFSConsole.STDOUT.WriteLine($"{item.Key}={item.Value}");
                }
            }
            else
            {
                for (int i = 0; i < args.Length; i++)
                {
                    var item = args[i];
                    if (item.StartsWith('-'))
                    {

                    }
                    else
                    {
                    }
                }
            }
        }
    }
    [LMFSFunction("echo")]
    public class Echo : FunctionBase
    {
        public override void Dispose()
        {
        }

        public override void Run(params string[] args)
        {
            foreach (var item in args)
            {

                LMFSConsole.STDOUT.Write(String.Join(' ', item));
            }
            LMFSConsole.STDOUT.WriteLine();
        }
    }

    [LMFSFunction("rm")]
    public class Remove:FunctionBase
    {
        public override void Dispose()
        {
        }
        public override void Run(params string[] args)
        {
            bool Recursive=false;
            List<string> input = new List<string>();
            for (int i = 0; i < args.Length; i++)
            {
                var item = args[i];
                if (item.StartsWith("-"))
                {
                    foreach (var c in item)
                    {
                        switch (c)
                        {
                            case 'r':
                                Recursive = true; 
                                break;
                            default:
                                break;
                        }
                    }
                }
                else
                {
                    input.Add(item);
                }
            }
            foreach (var item in input)
            {
                var l=Path.Combine(LMFSExtensibleEnv.CurrentDirectory, item);
                if (Directory.Exists(l))
                {
                    Directory.Delete(l, Recursive);
                }else if (Directory.Exists(item))
                {
                    Directory.Delete(item, Recursive);
                }else if (File.Exists(l))
                {
                    File.Delete(l);
                }else if(File.Exists(item))
                {
                    File.Delete(item);
                }
            }
        }
    }
}
