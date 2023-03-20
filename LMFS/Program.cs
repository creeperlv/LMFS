using LMFS.Core;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Text;

namespace LMFS
{
    internal class Program
    {
        static void InputListen()
        {
            while (true)
            {
                var line = Console.ReadLine();
                if (line == "exit")
                {
                    Environment.Exit(0);
                }
                else if (line == "reload-templates")
                {
                    serverCore.LoadTemplate();
                    Console.WriteLine("Done.");
                }
                else if (line == "save-runtime-auth")
                {
                    serverCore.LoadTemplate();
                    Console.WriteLine("Done.");
                }
            }
        }
        static ConsoleCache Cache = new ConsoleCache();
        static void ConsoleService()
        {
            while (true)
            {
                var k = Console.ReadKey(true);
                (int x, int y) = Console.GetCursorPosition();
                if (k.Key == ConsoleKey.LeftArrow)
                {
                    x -= 1;
                    Cache.Index = Math.Max(Cache.Index - 1, 0);
                    Console.SetCursorPosition(Math.Max(0, Math.Min(Console.BufferWidth, x)), y);
                }
                else if (k.Key == ConsoleKey.RightArrow)
                {
                    x += 1;
                    Cache.Index = Math.Min(Cache.Index - 1, Cache.str.Length - 1);
                    Console.SetCursorPosition(Math.Max(0, Math.Min(Console.BufferWidth, x)), y);
                }
                else if (k.Key == ConsoleKey.Enter)
                {
                    string line = Cache.str;
                    Cache.str = "";
                    Cache.Index = 0;
                    Console.WriteLine();
                    Console.WriteLine(">" + line);
                }
                else if (k.Key == ConsoleKey.UpArrow)
                {

                }
                else if (k.Key == ConsoleKey.DownArrow)
                {

                }
                else if (k.Key == ConsoleKey.Backspace)
                {

                    Console.SetCursorPosition(0, y);
                    Cache.Remove();
                    for (int i = Cache.Index % Console.BufferWidth; i < Console.BufferWidth; i++)
                    {
                        Console.Write(' ');
                    }
                    Console.SetCursorPosition(0, y);
                    Console.Write(Cache.str);

                    Console.SetCursorPosition(Math.Max(0, Math.Min(Console.BufferWidth, Cache.Index % Console.BufferWidth)), y);
                }
                else
                {
                    Cache.AddChar(k.KeyChar);
                    Cache.Index++;
                    Console.Write(Cache.str.Substring(Cache.Index - 1));
                    Console.SetCursorPosition(Math.Max(0, Math.Min(Console.BufferWidth, Cache.Index % Console.BufferWidth)), y);
                }

            }
        }
        static ServerCore serverCore;
        static void Main(string[] args)
        {
            Argument argument = Argument.FromArgs(args);
            switch (argument.Operation)
            {
                case Operation.Start:
                    {

                        Trace.Listeners.Add(new ConsoleLogger());
                        ServerConfiguration conf = JsonConvert.DeserializeObject<ServerConfiguration>(File.ReadAllText(argument.main_argument)) ?? CreateNewConfiguration();
                        serverCore = new ServerCore(conf);
                        serverCore.Start();
                        if (argument.UseInput)
                        {
                            Task.Run(serverCore.Listen);
                            //Task.Run(ConsoleService);
                            //ConsoleService();
                            InputListen();
                        }
                        else
                        {

                            serverCore.Listen();
                        }
                    }
                    break;
                case Operation.Init:
                    {
                        ServerConfiguration serverConfiguration = CreateNewConfiguration();
                        File.WriteAllText(argument.main_argument, JsonConvert.SerializeObject(serverConfiguration, Formatting.Indented));
                        Directory.CreateDirectory("./template/");
                    }
                    break;
                case Operation.Init_User_Base:
                    {
                        GenUserBase();
                    }
                    break;
                case Operation.Init_Auth_Base:
                    {
                        GenAuthBase();
                    }
                    break;
                case Operation.ExportTemplate:
                    {
                        ContentGenerator generator = new ContentGenerator();
                        generator.Export(argument.main_argument);
                    }
                    break;
                case Operation.Help:
                    {
                        Console.WriteLine("Operations:");
                        Console.WriteLine();
                        Console.WriteLine("start <configuration-file>");
                        Console.WriteLine("\tStart a LMFS server.");
                        Console.WriteLine();
                        Console.WriteLine("init <configuration-file>");
                        Console.WriteLine("\tInit a LMFS server configuration.");
                        Console.WriteLine();
                        Console.WriteLine("export-template <dest>");
                        Console.WriteLine("\tExport a LMFS server template to target destination.");
                    }
                    break;
                default:
                    break;
            }
        }
        public static void GenAuthBase()
        {
            Directory.CreateDirectory("./authbase/");

        }
        public static void GenUserBase()
        {
            Directory.CreateDirectory("./userbase/");

        }
        private static ServerConfiguration CreateNewConfiguration()
        {
            ServerConfiguration serverConfiguration = new ServerConfiguration();
            serverConfiguration.ListeningUrl.Add("http://localhost:8080/");
            serverConfiguration.PathMap.Add("/", "./webroot/");
            serverConfiguration.MimeMap = MIMEMap.GenerateMIMEMap();
            serverConfiguration.Template = "./template/";
            return serverConfiguration;
        }
    }
}