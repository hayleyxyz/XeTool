using System;
using System.IO;
using XeLib.Utilities;

namespace XeLib.IO
{
    public class XeWriter
    {
        public Stream Stream { get; protected set; }

        protected byte[] internalBuffer;

        public XeWriter(Stream stream) {
            Stream = stream;
            internalBuffer = new byte[8];
        }

        public void WriteUInt32(uint value, Endian endian = Endian.Big) {
            BufferUtils.FromUInt32(value, internalBuffer, 0, endian);
            Stream.Write(internalBuffer, 0, 4);
        }

        public void WriteUInt16(ushort value, Endian endian = Endian.Big) {
            BufferUtils.FromUInt16(value, internalBuffer, 0, endian);
            Stream.Write(internalBuffer, 0, 2);
        }

        public void Write(byte[] buffer, int offset, int count) {
            Stream.Write(buffer, offset, count);
        }
    }
}
