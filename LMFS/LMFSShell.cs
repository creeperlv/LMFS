using LibCLCC.NET.TextProcessing;
using LMFS.Extensible;
using System.Reflection;

namespace LMFS {
    internal static class LMFSShell {
        static CommandLineParser CommandLineParser;
        static Dictionary<string, FunctionBase> functions = new Dictionary<string, FunctionBase>();
        public static IEnumerable<Type> GetTypesWithAttribute<TAttribute>() where TAttribute : Attribute {
            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => type.GetCustomAttributes(typeof(TAttribute), true).Length > 0);
        }
        internal static void InputListen() {
            LMFSConsole.STDOUT = Console.Out;
            CommandLineParser = new CommandLineParser();
            {
                var ts = GetTypesWithAttribute<LMFSFunctionAttribute>();
                foreach (var item in ts) {
                    functions.Add(item.GetCustomAttribute<LMFSFunctionAttribute>().Name, (FunctionBase)Activator.CreateInstance(item));
                }
            }
            while (true) {
                LMFSConsole.STDOUT.Write($"\x1b[0;92mLMFS:\x1b[0m{LMFSExtensibleEnv.CurrentDirectory}> # ");
                var line = Console.ReadLine();
                if (line == "exit") {
                    Environment.Exit(0);
                }
                else if (line == "reload-templates") {
                    serverCore.LoadTemplate();
                    Console.WriteLine("Done.");
                }
                else if (line == "save-runtime-auth") {
                    serverCore.LoadTemplate();
                    Console.WriteLine("Done.");
                }
                else {
                    var s = CommandLineParser.Parse(line, false);
                    List<string> strings = new List<string>();
                    while (true) {
                        strings.Add(s.content);
                        if (s.Next == null) break;
                        s = s.Next;
                    }
                    if (strings.Count > 0) {
                        var name = strings.First();
                        strings.RemoveAt(0);
                        if (functions.ContainsKey(name)) {
                            try {
                                functions[name].Run(strings.ToArray());
                            }
                            catch (Exception e) {
                                LMFSConsole.STDOUT.WriteLine("Something went wrong: " + e);
                            }
                            finally {
                                try {
                                    functions[name].Dispose();
                                }
                                catch (Exception e) {
                                    LMFSConsole.STDOUT.WriteLine("Something went wrong at cleanup: " + e);
                                }
                            }
                        }
                        else {
                            LMFSConsole.STDOUT.WriteLine("Command not found.");
                        }
                    }
                }
            }
        }
    }
}