using Newtonsoft.Json.Serialization;
using System.Diagnostics;

namespace LMFS.Core {
    public class ConsoleLogger : TraceListener {
        public override void Write(string? message) {
            Console.Write(message);
        }

        public override void WriteLine(string? message) {
            Console.WriteLine(message);
        }
    }
}