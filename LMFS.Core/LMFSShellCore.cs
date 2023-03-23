using LibCLCC.NET.TextProcessing;
using LMFS.Extensible;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace LMFS.Server.Core
{
    public class LMFSShellCore
    {
        CommandLineParser CommandLineParser;
        Dictionary<string, Type> functions = new Dictionary<string, Type>();
        public IEnumerable<Type> GetTypesWithAttribute<TAttribute>() where TAttribute : Attribute
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => type.GetCustomAttributes(typeof(TAttribute), true).Length > 0);
        }
        LMFSConsole LMFSConsole;
        public void Execute(string command)
        {
            var s = CommandLineParser.Parse(command, false);
            List<List<string>> Piped = new List<List<string>> {
                new List<string>()
            };
            //List<string> strings = new List<string>();
            while (true)
            {
                if (s.content == "|")
                    Piped.Add(new List<string>());
                else
                    Piped.Last().Add(s.content);
                if (s.Next == null) break;
                s = s.Next;
            }
            routedReader.UnderlyingReader = Console.In;
            routedWriter.UnderlyingWriter = Console.Out;
            if (Piped.Count > 1)
            {
                routedWriter.UnderlyingWriter = BufferWriter;
            }
            else
            {
            }

            MemoryStream memoryStream_old = null;
            for (int i = 0; i < Piped.Count; i++)
            {
                var strings = Piped[i];
                MemoryStream memoryStream = new MemoryStream();
                BufferWriter = new StreamWriter(memoryStream);
                if (i == Piped.Count - 1)
                {
                    routedWriter.UnderlyingWriter = Console.Out;
                }
                else if (Piped.Count > 1)
                {
                    routedWriter.UnderlyingWriter = BufferWriter;
                }
                if (strings.Count > 0)
                {
                    var name = strings.First();
                    strings.RemoveAt(0);
                    if (functions.ContainsKey(name))
                    {
                        try
                        {
                            var proc = (FunctionBase)Activator.CreateInstance(functions[name]);
                            proc.LMFSConsole = LMFSConsole;
                            try
                            {
                                proc.Run(strings.ToArray());
                            }
                            catch (Exception e)
                            {
                                LMFSConsole.STDOUT.WriteLine("Something went wrong: " + e);
                            }
                            finally
                            {
                                try
                                {
                                    proc.Dispose();
                                }
                                catch (Exception e)
                                {
                                    LMFSConsole.STDOUT.WriteLine("Something went wrong at cleanup: " + e);
                                }
                            }
                        }
                        catch (Exception)
                        {
                        }
                    }
                    else
                    {
                        LMFSConsole.STDOUT.WriteLine("Command not found.");
                    }
                    //Console.WriteLine("Reader Swapped to BufferReader.");
                    if (i != Piped.Count - 1)
                    {
                        if (memoryStream_old != null)
                        {
                            memoryStream_old.Dispose();
                        }
                        routedWriter.Flush();
                        if (BufferReader != null)
                            BufferReader.Close();
                        memoryStream.Flush();
                        memoryStream_old = memoryStream;
                        memoryStream_old.Position = 0;
                        memoryStream.Seek(0, SeekOrigin.Begin);
                        //Console.WriteLine($"BUFFER:{memoryStream_old.Length}");
                        BufferReader = new StreamReader(memoryStream_old);

                        routedReader.UnderlyingReader = BufferReader;
                    }
                }
            }
            //Console.WriteLine(BufferReader.ReadToEnd());
            if (memoryStream_old != null)
            {
                BufferReader.Dispose();
                BufferWriter.Dispose();
                memoryStream_old.Dispose();
            }


        }
        TextWriter BufferWriter;
        TextReader BufferReader;
        RoutedWriter routedWriter;
        RoutedReader routedReader;
        public void Setup()
        {
            routedWriter = new RoutedWriter();
            routedWriter.UnderlyingWriter = Console.Out;
            routedReader = new RoutedReader();

            LMFSConsole.STDIN = routedReader;
            LMFSConsole.STDOUT = routedWriter;

        }
        public void InputListen()
        {
            LMFSConsole = new LMFSConsole();
            Setup();

            CommandLineParser = new CommandLineParser();
            CommandLineParser.PredefinedSegmentCharacters.Add('=');
            CommandLineParser.PredefinedSegmentCharacters.Add('|');
            {
                var ts = GetTypesWithAttribute<LMFSFunctionAttribute>();
                foreach (var item in ts)
                {
                    functions.Add(item.GetCustomAttribute<LMFSFunctionAttribute>().Name, item);
                }
            }
            while (true)
            {
                LMFSConsole.STDOUT.Write($"\x1b[0;92mLMFS:\x1b[0m{LMFSExtensibleEnv.CurrentDirectory}> # ");
                var line = Console.ReadLine();
                if (line == "exit")
                {
                    return;
                }
                else
                {
                    Execute(line);
                }
            }
        }
    }
}