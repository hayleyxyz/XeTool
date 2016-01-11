using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using XeLib.IO;
using XeLib.Utilities;

namespace XeLib.Bootloader
{
    public class SXBootloader
    {
        protected Stream stream;
        protected XeReader reader;

        public ushort magic; // 0x00
        public ushort version; // 0x02
        public uint entryPoint; // 0x08
        public uint length; // 0x0c
        public byte[] data;

        public SXBootloader(Stream stream) {
            this.stream = stream;
            reader = new XeReader(stream);
        }

        public void Read() {
            magic = reader.ReadUInt16();
            version = reader.ReadUInt16();

            reader.Seek(4, SeekOrigin.Current);
            entryPoint = reader.ReadUInt32();
            length = reader.ReadUInt32();

            data = new byte[length - 0x10];
            reader.Read(data, 0, data.Length);
        }

        public string GetMagicAsString() {
            var buf = new byte[2];
            BufferUtils.FromUInt16(magic, buf);
            return Encoding.UTF8.GetString(buf);
        }
    }
}
