using LMFS.Core;
using Newtonsoft.Json;
using System.Diagnostics;

namespace LMFS {
    public class Argument {
        public Operation Operation;
        public string main_argument = "config.json";
        public static Argument FromArgs(string[] args) {
            Argument argument = new Argument();
            for (int i = 0; i < args.Length; i++) {
                var item = args[i];
                switch (item) {
                    case "start":
                        argument.Operation = Operation.Start;
                        break;
                    case "init":
                        argument.Operation = Operation.Init;
                        break;
                    case "init-user-base":
                        argument.Operation = Operation.Init_User_Base;
                        break;
                    case "init-folder-attribute":
                        argument.Operation = Operation.Init_Folder_Attribute;
                        break;
                    case "export-template":
                        argument.Operation = Operation.ExportTemplate;
                        break;
                    default:
                        argument.main_argument = item;
                        break;
                }
            }
            return argument;
        }
    }
    public enum Operation {
        Help, Start, Init, ExportTemplate, Init_User_Base, Init_Folder_Attribute
    }
    internal class Program {
        static void Main(string[] args) {
            Argument argument = Argument.FromArgs(args);
            switch (argument.Operation) {
                case Operation.Start: {

                        Trace.Listeners.Add(new ConsoleLogger());
                        ServerConfiguration conf = JsonConvert.DeserializeObject<ServerConfiguration>(File.ReadAllText(argument.main_argument)) ?? CreateNewConfiguration();
                        ServerCore serverCore = new ServerCore(conf);
                        serverCore.Start();
                        serverCore.Listen();
                    }
                    break;
                case Operation.Init: {
                        ServerConfiguration serverConfiguration = CreateNewConfiguration();
                        File.WriteAllText(argument.main_argument, JsonConvert.SerializeObject(serverConfiguration, Formatting.Indented));
                        Directory.CreateDirectory("./template/");
                    }
                    break;
                case Operation.Init_User_Base: {
                        ServerConfiguration serverConfiguration = CreateNewConfiguration();
                        File.WriteAllText(argument.main_argument, JsonConvert.SerializeObject(serverConfiguration, Formatting.Indented));
                        Directory.CreateDirectory("./template/");
                    }
                    break;
                case Operation.ExportTemplate: {
                        ContentGenerator generator = new ContentGenerator();
                        generator.Export(argument.main_argument);
                    }
                    break;
                case Operation.Help: {
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

        private static ServerConfiguration CreateNewConfiguration() {
            ServerConfiguration serverConfiguration = new ServerConfiguration();
            serverConfiguration.ListeningUrl.Add("http://localhost:8080/");
            serverConfiguration.PathMap.Add("/", "./webroot/");
            serverConfiguration.MimeMap = MIMEMap.GenerateMIMEMap();
            serverConfiguration.Template = "./template/";
            return serverConfiguration;
        }
    }
}