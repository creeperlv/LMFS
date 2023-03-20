namespace LMFS
{
    public class Argument {
        public Operation Operation;
        public bool UseInput=false;
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
                    case "--accept-input":
                        argument.UseInput = true;
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
}