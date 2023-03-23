using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace LMFS.Extensible
{
    public class RoutedTWR : IDisposable
    {
        public MemoryStream Stream;
        public RoutedTWR()
        {
            Stream = new MemoryStream();
        }
        public TextWriter OpenWrite() { return (new StreamWriter(Stream)); }
        public TextReader OpenRead() { return (new StreamReader(Stream)); }

        public void Dispose()
        {
            Stream.Dispose();
        }
    }
    public class RoutedReader : TextReader
    {
        public TextReader UnderlyingReader;
        public override int Read()
        {
            return UnderlyingReader.Read();
        }
        public override string ReadLine()
        {
            return UnderlyingReader.ReadLine();
        }
        public override string ReadToEnd()
        {
            return UnderlyingReader.ReadToEnd();
        }
        public override int Read(char[] buffer, int index, int count)
        {
            return UnderlyingReader.Read(buffer, index, count);
        }
        public async override Task<string> ReadLineAsync()
        {
            return await UnderlyingReader.ReadLineAsync();
        }
        public async override Task<string> ReadToEndAsync()
        {
            return await UnderlyingReader.ReadToEndAsync();
        }
        public override int Peek()
        {
            return UnderlyingReader.Peek();
        }
        public override int ReadBlock(Span<char> buffer)
        {
            return UnderlyingReader.ReadBlock(buffer);
        }
    }
    public class RoutedWriter : TextWriter
    {
        public Encoding _Encoding;
        public override Encoding Encoding => _Encoding;
        public TextWriter? UnderlyingWriter;
        public override void Flush()
        {
            UnderlyingWriter?.Flush();
        }
        public override void Write(bool value)
        {
            UnderlyingWriter?.Write(value);
        }
        public override void Write(char value)
        {
            UnderlyingWriter?.Write(value);
        }
        public override void Write(char[] buffer)
        {
            UnderlyingWriter?.Write(buffer);
        }
        public override void Write(string value)
        {
            UnderlyingWriter?.Write(value);
        }
        public override void Write(int value)
        {
            UnderlyingWriter?.Write(value);
        }
        public override void Write(float value)
        {
            UnderlyingWriter?.Write(value);
        }
        public override void Write(double value)
        {
            UnderlyingWriter?.Write(value);
        }
        public override void Write(object value)
        {
            UnderlyingWriter?.Write(value);
        }
        public override void WriteLine(bool value)
        {
            UnderlyingWriter?.WriteLine(value);
        }
        public override void WriteLine(char value)
        {
            UnderlyingWriter?.WriteLine(value);
        }
        public override void WriteLine(char[] buffer)
        {
            UnderlyingWriter?.WriteLine(buffer);
        }
        public override void WriteLine(string value)
        {
            UnderlyingWriter?.WriteLine(value);
        }
        public override void WriteLine(int value)
        {
            UnderlyingWriter?.WriteLine(value);
        }
        public override void WriteLine(float value)
        {
            UnderlyingWriter?.WriteLine(value);
        }
        public override void WriteLine(double value)
        {
            UnderlyingWriter?.WriteLine(value);
        }
        public override void WriteLine(object value)
        {
            UnderlyingWriter?.WriteLine(value);
        }
        public override void WriteLine()
        {
            UnderlyingWriter?.WriteLine();
        }
    }
}
