using LMFS.Core;
using LMFS.Extensible;

namespace LMFS.Server.Core.Management
{
    [LMFSFunction("serverconfig")]
    public class ServerConfig : FunctionBase
    {
        public override void Run(params string[] args)
        {
            foreach (var item in args)
            {
                if (item.StartsWith("-"))
                {

                }
                else
                {
                    switch (item)
                    {
                        case "reload-template":
                            ServerCore.CurrentCore?.LoadTemplate();
                            break;
                        default:
                            break;
                    }
                }
            }
        }
    }

}
