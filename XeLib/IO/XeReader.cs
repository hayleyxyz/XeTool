using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using XeLib.Utilities;

namespace XeLib.IO
{
    public class XeReader
    {
        public Stream Stream { get; protected set; }

        public long Length { get { return Stream.Length; } }

        protected byte[] internalBuffer;

        public XeReader(Stream stream) {
            Stream = stream;
            internalBuffer = new byte[8];
        }

        public long Seek(long offset, SeekOrigin origin) {
            return Stream.Seek(offset, origin);
        }

        public int Read(byte[] buffer, int offset, int count) {
            return Stream.Read(buffer, offset, count);
        }

        public int ReadInt32(Endian endian = Endian.Big) {
            Read(internalBuffer, 0, 4);
            return BufferUtils.ToInt32(internalBuffer, 0, endian);
        }

        public uint ReadUInt32(Endian endian = Endian.Big) {
            Read(internalBuffer, 0, 4);
            return BufferUtils.ToUInt32(internalBuffer, 0, endian);
        }

        public short ReadInt16(Endian endian = Endian.Big) {
            Read(internalBuffer, 0, 2);
            return BufferUtils.ToInt16(internalBuffer, 0, endian);
        }

        public ushort ReadUInt16(Endian endian = Endian.Big) {
            Read(internalBuffer, 0, 2);
            return BufferUtils.ToUInt16(internalBuffer, 0, endian);
        }

        public sbyte ReadInt8() {
            return (sbyte)Stream.ReadByte();
        }

        public byte ReadUInt8() {
            return (byte)Stream.ReadByte();
        }

        public string ReadString(int length) {
            var buffer = new byte[length];
            Read(buffer, 0, length);
            return Encoding.UTF8.GetString(buffer);
        }
    }
}
