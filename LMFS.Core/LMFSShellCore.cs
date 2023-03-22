using LibCLCC.NET.TextProcessing;
using LMFS.Extensible;
using System.Reflection;

namespace LMFS.Server.Core {
    public class LMFSShellCore {
        CommandLineParser CommandLineParser;
        Dictionary<string, Type> functions = new Dictionary<string, Type>();
        public IEnumerable<Type> GetTypesWithAttribute<TAttribute>() where TAttribute : Attribute {
            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => type.GetCustomAttributes(typeof(TAttribute), true).Length > 0);
        }
        LMFSConsole LMFSConsole;
        public void Execute(string command) {
            var s = CommandLineParser.Parse(command, false);
            List<List<string>> Piped = new List<List<string>> {
                new List<string>()
            };
            //List<string> strings = new List<string>();
            while (true) {
                if (s.content == "|")
                    Piped.Add(new List<string>());
                else
                    Piped.Last().Add(s.content);
                if (s.Next == null) break;
                s = s.Next;
            }
            foreach (var strings in Piped) {
                if (strings.Count > 0) {
                    var name = strings.First();
                    strings.RemoveAt(0);
                    if (functions.ContainsKey(name)) {
                        try {
                            var proc = (FunctionBase)Activator.CreateInstance(functions[name]);
                            proc.LMFSConsole = LMFSConsole;
                            try {
                                proc.Run(strings.ToArray());
                            }
                            catch (Exception e) {
                                LMFSConsole.STDOUT.WriteLine("Something went wrong: " + e);
                            }
                            finally {
                                try {
                                    proc.Dispose();
                                }
                                catch (Exception e) {
                                    LMFSConsole.STDOUT.WriteLine("Something went wrong at cleanup: " + e);
                                }
                            }
                        }
                        catch (Exception) {
                        }

                    }
                    else {
                        LMFSConsole.STDOUT.WriteLine("Command not found.");
                    }
                }
            }
        }
        public void InputListen() {
            LMFSConsole = new LMFSConsole();
            LMFSConsole.STDOUT = Console.Out;
            LMFSConsole.STDIN = Console.In;
            CommandLineParser = new CommandLineParser();
            CommandLineParser.PredefinedSegmentCharacters.Add('=');
            CommandLineParser.PredefinedSegmentCharacters.Add('|');
            {
                var ts = GetTypesWithAttribute<LMFSFunctionAttribute>();
                foreach (var item in ts) {
                    functions.Add(item.GetCustomAttribute<LMFSFunctionAttribute>().Name, item);
                }
            }
            while (true) {
                LMFSConsole.STDOUT.Write($"\x1b[0;92mLMFS:\x1b[0m{LMFSExtensibleEnv.CurrentDirectory}> # ");
                var line = Console.ReadLine();
                if (line == "exit") {
                    return;
                }
                else {
                    Execute(line);
                }
            }
        }
    }
}