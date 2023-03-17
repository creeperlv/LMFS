using System.Diagnostics;
using System.IO.Compression;

namespace LMFS.Exchange.Core {
    public static class CompressionManager {
        public static Dictionary<string, bool> Status = new Dictionary<string, bool>();
        public static string QueryComopress(string path) {
            var zip = Path.GetRandomFileName() + ".zip";
            Status.Add(zip, false);

            Task.Run(() => {
                var fullpath = Path.Combine(Path.GetTempPath(),zip);
                Trace.WriteLine("Compressing to:" + fullpath);
                ZipFile.CreateFromDirectory(path, fullpath);
                Trace.WriteLine("Compressed to:" + fullpath);
                Status[zip] = true;
            });
            return zip;
        }
        public static bool Query(string name, out bool Query) {
            if (Status.ContainsKey(name)) { Query = Status[name]; return true; }
            Query = false;
            return false;
        }
    }
}