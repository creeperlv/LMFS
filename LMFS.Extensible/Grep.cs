//////////////////////////////////////
//This file contains Unix-Like tools//
//////////////////////////////////////

using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace LMFS.Extensible
{
    [LMFSFunction("grep")]
    public class Grep : FunctionBase
    {
        public override void Dispose()
        {
        }
        public void Process(TextReader textReader, List<string> regex)
        {
            while (true)
            {
                var str=textReader.ReadLine();
                if (str == null) return;
                bool isMatch=true;
                foreach (var item in regex)
                {
                    isMatch&=Regex.IsMatch(str, item);
                }
                if (isMatch)
                {
                    //Console.WriteLine(str);
                    LMFSConsole.STDOUT.WriteLine(str);
                }
            }

        }
        public override void Run(params string[] args)
        {
            //Console.Write(LMFSConsole.STDIN.GetType().Name);    
            //Console.Write((LMFSConsole.STDIN as RoutedReader).UnderlyingReader.GetType().Name);    
            List<string> files = new List<string>();
            List<string> regex = new List<string>();
            foreach (var item in args)
            {
                if (item.StartsWith("-"))
                {

                }
                else
                {
                    var file = Path.Combine(LMFSExtensibleEnv.CurrentDirectory, item);
                    if (File.Exists(file))
                    {
                        files.Add(file);

                    }
                    else
                    {
                        regex.Add(item);
                    }
                }
            }
            if(files.Count > 0)
            {
                foreach (var f in files)
                {
                    using (var reader=File.OpenRead(f))
                    {
                        using (TextReader tr=new StreamReader(reader))
                        {
                            Process(tr, regex);
                        }
                    }
                }
            }
            else
            {
                Process(LMFSConsole.STDIN, regex);
            }

        }
    }

}
