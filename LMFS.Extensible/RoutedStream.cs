using System;
using System.IO;
using System.Text;

namespace LMFS.Extensible {
    public class RoutedStream : Stream {
        public Stream? R_FROM;
        public Stream? W_TO;
        public override bool CanRead => R_FROM == null ? false : R_FROM.CanRead;

        public override bool CanSeek => R_FROM == null ? false : R_FROM.CanSeek;

        public override bool CanWrite => W_TO == null ? false : W_TO.CanWrite;

        public override long Length => R_FROM == null ? 0 : R_FROM.Length;

        public override long Position { get => R_FROM == null ? 0 : R_FROM.Position; set { if (R_FROM != null) R_FROM.Position = value; } }

        public override void Flush() {
            W_TO?.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count) {
            return R_FROM == null ? -1 : R_FROM.Read(buffer, offset, count);
        }

        public override long Seek(long offset, SeekOrigin origin) {
            return R_FROM == null ? -1 : R_FROM.Seek(offset, origin);
        }

        public override void SetLength(long value) {
            W_TO?.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count) {
            W_TO?.Write(buffer, offset, count);
        }
    }

}
