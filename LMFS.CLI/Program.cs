using Newtonsoft.Json;

namespace LMFS.CLI {

    internal class Program {
        static string FolderConfig = null;
        static bool FOLDER_MODE = false;
        static void PULL(LMFSHttpClient client, string remotepath, string localpath, bool preserve_folder, bool preserve_file) {
            var l = client.GetList(remotepath, out var success);
            if (success) {
                var di = new DirectoryInfo(localpath);
                if (!di.Exists) {
                    di.Create();
                }
                List<string> remote_d = new List<string>();
                List<string> remote_f = new List<string>();
                foreach (var item in l) {
                    if (item.StartsWith("DIR:")) {
                        remote_d.Add(item.Substring(4));
                    }
                    else if (item.StartsWith("FLE:")) {
                        remote_f.Add(item.Substring(4));
                    }
                }
                if (!preserve_file) {
                    var fs = di.EnumerateFiles();
                    foreach (var f in fs) {
                        bool hit = false;
                        if (FOLDER_MODE)
                            if (f.Name == FolderConfig) continue;
                        foreach (var item in remote_f) {
                            if (f.Name == item) {
                                hit = true;
                                break;
                            }
                        }
                        if (hit) {
                            f.Delete();
                        }
                    }
                }
                if (!preserve_folder) {
                    var fs = di.EnumerateDirectories();
                    foreach (var f in fs) {
                        bool hit = false;
                        foreach (var item in remote_d) {
                            if (f.Name == item) {
                                hit = true;
                                break;
                            }
                        }
                        if (hit) {
                            f.Delete();
                        }
                    }
                }
                foreach (var item in remote_f) {
                    if (FOLDER_MODE)
                        if (item == FolderConfig) continue;
                    client.Get(Path.Combine(remotepath, item).Replace('\\', '/'), Path.Combine(localpath, item));
                }
                foreach (var item in remote_d) {
                    PULL(client, Path.Combine(remotepath, item).Replace('\\', '/'), Path.Combine(localpath, item), preserve_folder, preserve_file);
                }
            }

        }
        static void PUSH(LMFSHttpClient client, string remotepath, string localpath, bool preserve_folder, bool preserve_file) {
            var l = client.GetList(remotepath, out var success);
            if (success) {
                var di = new DirectoryInfo(localpath);
                if (!di.Exists) {
                    di.Create();
                }
                List<string> remote_d = new List<string>();
                List<string> remote_f = new List<string>();
                foreach (var item in l) {
                    if (item.StartsWith("DIR:")) {
                        remote_d.Add(item.Substring(4));
                    }
                    else if (item.StartsWith("FLE:")) {
                        remote_f.Add(item.Substring(4));
                    }
                }
                var fs = di.EnumerateFiles();
                if (!preserve_file) {
                    foreach (var item in remote_f) {
                        bool hit = false;
                        if (FOLDER_MODE)
                            if (item == FolderConfig) continue;
                        foreach (var f in fs) {
                            if (f.Name == item) {
                                hit = true;
                                break;
                            }
                        }
                        if (hit) {
                            client.Remove(Path.Combine(remotepath, item).Replace("\\", "/"));
                        }
                    }
                }
                var ds = di.EnumerateDirectories();
                if (!preserve_folder) {
                    foreach (var item in remote_d) {
                        bool hit = false;
                        foreach (var f in ds) {
                            if (f.Name == item) {
                                hit = true;
                                break;
                            }
                        }
                        if (hit) {
                            client.Remove(Path.Combine(remotepath, item).Replace("\\", "/"));
                        }
                    }
                }
                foreach (var item in fs) {
                    if (FOLDER_MODE)
                        if (item.Name == FolderConfig) continue;
                    client.Push(remotepath, Path.Combine(localpath, item.Name));
                }
                foreach (var item in ds) {
                    PUSH(client, Path.Combine(remotepath, item.Name).Replace('\\', '/'), Path.Combine(localpath, item.Name), preserve_folder, preserve_file);
                }
            }
            else {
                client.Mkdir(remotepath);
                PUSH(client, remotepath, localpath, preserve_folder, preserve_file);
            }
        }
        static void Help() {
            Console.WriteLine("./LMFS.CLI <operation> [arguments...]");
            Console.WriteLine();
            Console.WriteLine("Operations:");
            Console.WriteLine();
            Console.WriteLine("init <path>");
            Console.WriteLine();
            Console.WriteLine("\tInitialize a folder configuration.");
            Console.WriteLine();
            Console.WriteLine("push <path> | push <url> -r <remote-path> -p <local-path>");
            Console.WriteLine();
            Console.WriteLine("\tPush a folder.");
            Console.WriteLine();
            Console.WriteLine("pull <path> | pull <url> -r <remote-path> -p <local-path>");
            Console.WriteLine();
            Console.WriteLine("\tPull a folder.");
            Console.WriteLine();
            Console.WriteLine("help");
            Console.WriteLine();
            Console.WriteLine("\tShow this help.");
            Console.WriteLine();
            Console.WriteLine("Options:");
            Console.WriteLine();
            Console.WriteLine("-PFOLDER --preserve-deleted-folders");
            Console.WriteLine("\tPreserve Deleted Folders (Both local and remote)");
            Console.WriteLine();
            Console.WriteLine("-PFILE --preserve-deleted-files");
            Console.WriteLine("\tPreserve Deleted Files (Both local and remote)");
            Console.WriteLine();
            Console.WriteLine("Examples:");
            Console.WriteLine();
            Console.WriteLine("./LMFS.CLI init ./folder.json");
            Console.WriteLine("\tInitialize a folder configuration.");
            Console.WriteLine();
            Console.WriteLine("./LMFS.CLI push ./folder.json");
            Console.WriteLine("\tPush a folder with given configuration.");
            Console.WriteLine();
            Console.WriteLine("./LMFS.CLI pull ./folder.json");
            Console.WriteLine("\tPull a folder with given configuration.");
            Console.WriteLine();
            Console.WriteLine("./LMFS.CLI push http://localhost/ -r /storage0/ -p ./synced-folder/");
            Console.WriteLine("\tPush a folder with given parameters.");
            Console.WriteLine();
            Console.WriteLine("./LMFS.CLI pull http://localhost/ -r /storage0/ -p ./synced-folder/");
            Console.WriteLine("\tPull a folder with given parameters.");
            Console.WriteLine();
        }
        static void Main(string[] args) {
            Arguments arguments = Arguments.FromArgs(args);
            if (arguments.Operation != Operation.help && arguments.Operation != Operation.init) {
                string url = arguments.url;
                string localpath = arguments.localpath;
                string remotepath = arguments.remotepath;
                if (File.Exists(url)) {
                    var fc = JsonConvert.DeserializeObject<FolderConfig>(File.ReadAllText(url));
                    url = fc.remote_server;
                    Console.WriteLine(url);
                    remotepath = fc.remote_path;
                    FileInfo fileInfo = new FileInfo(arguments.url);
                    localpath= fileInfo.Directory.FullName;
                    FOLDER_MODE = true;
                    FolderConfig = fileInfo.Name;
                }
                LMFSHttpClient client = new LMFSHttpClient(new Uri(url));

                switch (arguments.Operation) {
                    case Operation.pull:
                        PULL(client, remotepath, localpath, arguments.PreserveDeletedFolders, arguments.PreserveDeletedFiles);
                        break;
                    case Operation.push:
                        PUSH(client, remotepath, localpath, arguments.PreserveDeletedFolders, arguments.PreserveDeletedFiles);
                        break;
                    case Operation.get: {
                            var response = client.Get(arguments.remotepath, localpath);
                            Console.WriteLine(response.ToString());
                        }
                        break;
                    case Operation.GetList: {
                            var c = client.GetList(arguments.remotepath, out _);
                            foreach (var item in c) {
                                Console.WriteLine(item);
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
            else {
                switch (arguments.Operation) {
                    case Operation.help:
                        Help();
                        break;
                    case Operation.init: {
                            FolderConfig fc = new FolderConfig();
                            fc.remote_path = "/webroot/";
                            fc.remote_server = "http://localhost/";
                            File.WriteAllText(arguments.url, JsonConvert.SerializeObject(fc,
                                new JsonSerializerSettings {
                                    Formatting = Formatting.Indented
                                }));
                        }
                        break;
                    default:
                        break;
                }
            }
        }
    }
}