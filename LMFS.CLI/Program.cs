namespace LMFS.CLI {

    internal class Program {
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
        static void Main(string[] args) {
            Arguments arguments = Arguments.FromArgs(args);
            if (arguments.Operation != Operation.help) {
                LMFSHttpClient client = new LMFSHttpClient(new Uri(arguments.url));

                switch (arguments.Operation) {
                    case Operation.pull:
                        PULL(client, arguments.remotepath, arguments.localpath[0], arguments.PreserveDeletedFolders, arguments.PreserveDeletedFiles);
                        break;
                    case Operation.push:
                        PUSH(client, arguments.remotepath, arguments.localpath[0], arguments.PreserveDeletedFolders, arguments.PreserveDeletedFiles);
                        break;
                    case Operation.get: {
                            var response = client.Get(arguments.remotepath, arguments.localpath[0]);
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
        }
    }
}