//////////////////////////////////////
//This file contains Unix-Like tools//
//////////////////////////////////////

using System.Diagnostics;
using System.Threading.Tasks;

namespace LMFS.Extensible
{
    [LMFSFunction("exec")]
    public class Exec : FunctionBase
    {
        public override void Dispose()
        {
        }

        public override void Run(params string[] args)
        {
            ProcessStartInfo processStartInfo = new ProcessStartInfo(args[0]);
            string _args = "";
            for (int i = 1; i < args.Length; i++)
            {
                _args += args[i];
                _args += " ";
            }
            processStartInfo.Arguments = _args;
            processStartInfo.RedirectStandardInput = true;
            processStartInfo.RedirectStandardOutput = true;
            processStartInfo.WorkingDirectory = LMFSExtensibleEnv.CurrentDirectory;
            //processStartInfo.standard
            var p = Process.Start(processStartInfo);
            //p.OutputDataReceived += (sender, args) => { LMFSConsole.STDOUT.WriteLine(args.Data); };
            Task.Run(() =>
            {
                while (true)
                {
                    var l = p.StandardOutput.ReadLine();
                    if (l != null)
                    {
                        LMFSConsole.STDOUT.WriteLine(l);
                    }
                    else break;
                }
            });
            p.WaitForExit();
        }
    }

}
