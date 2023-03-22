using System;
using System.IO;

namespace LMFS.Extensible {
    public class RoutedTWR:IDisposable {
        public MemoryStream Stream;
        public RoutedTWR() {
            Stream = new MemoryStream();
        }
        public TextWriter OpenWrite() { return (new StreamWriter(Stream)); }
        public TextReader OpenRead() { return (new StreamReader(Stream)); }

        public void Dispose() {
            Stream.Dispose();
        }
    }

}
