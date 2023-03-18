namespace LMFS.CLI {
    public class Arguments {
        public string url;
        public string remotepath = null;
        public string localpath = null;
        public Operation Operation = Operation.help;
        public bool PreserveDeletedFolders;
        public bool PreserveDeletedFiles;
        public bool UseZip;
        public static Arguments FromArgs(string[] args) {
            Arguments arguments = new Arguments();
            int ParaMode = 0;
            for (int i = 0; i < args.Length; i++) {
                var item = args[i];
                switch (item) {
                    case "get-list":
                        arguments.Operation = Operation.GetList;
                        break;
                    case "pull":
                        arguments.Operation = Operation.pull;
                        break;
                    case "get":
                        arguments.Operation = Operation.get;
                        break;
                    case "push":
                        arguments.Operation = Operation.push;
                        break;
                    case "init":
                        arguments.Operation = Operation.init;
                        break;
                    case "-Z":
                    case "--use-zip":
                        arguments.UseZip = true;
                        break;
                    case "-PFOLDER":
                    case "--preserve-deleted-folders":
                        arguments.PreserveDeletedFolders = true;
                        break;
                    case "-PFILE":
                    case "--preserve-deleted-files":
                        arguments.PreserveDeletedFiles = true;
                        break;
                    case "-u":
                    case "--url":
                        ParaMode = 0;
                        break;
                    case "-r":
                    case "-remote-path": {
                            ParaMode = 2;
                        }
                        break;
                    case "-p":
                    case "-path":
                        ParaMode = 1;
                        break;
                    default:
                        switch (ParaMode) {
                            case 0:
                                arguments.url = item;
                                break;
                            case 2:
                                arguments.remotepath = item;
                                break;
                            case 1:
                                arguments.localpath = (item);
                                break;
                            default:
                                break;
                        }
                        break;
                }
            }
            return arguments;
        }
    }
}