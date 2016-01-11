using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using XeLib.IO;
using XeLib.Bootloader;

namespace XeLib.ShadowBoot
{
    public class ShadowBootRom
    {
        protected Stream stream;
        protected XeReader reader;

        public ushort magic; // 0x00
        public ushort version; // 0x02
        public uint bootloaderOffset; // 0x08
        public uint length; // 0x0c
        public string copyright; // 0x10-0x46
        public uint kvOffset; // 0x60;
        public uint smcSize; // 0x78
        public uint smcOffset; // 0x7d

        public List<SXBootloader> bootloaders;

        public ShadowBootRom(Stream stream) {
            this.stream = stream;
            reader = new XeReader(stream);
        }

        public void Read() {
            reader.Seek(0, SeekOrigin.Begin);

            magic = reader.ReadUInt16();
            version = reader.ReadUInt16();

            reader.Seek(0x08, SeekOrigin.Begin);
            bootloaderOffset = reader.ReadUInt32();
            length = reader.ReadUInt32();
            copyright = reader.ReadString(0x37);

            reader.Seek(0x60, SeekOrigin.Begin);
            kvOffset = reader.ReadUInt32();

            reader.Seek(0x78, SeekOrigin.Begin);
            smcSize = reader.ReadUInt32();
            smcOffset = reader.ReadUInt32();

            reader.Seek(bootloaderOffset, SeekOrigin.Begin);

            bootloaders = new List<SXBootloader>();

            var sb = new SXBootloader(stream);
            sb.Read();
            bootloaders.Add(sb);

            var sc = new SXBootloader(stream);
            sc.Read();
            bootloaders.Add(sc);

            var sd = new SXBootloader(stream);
            sd.Read();
            bootloaders.Add(sd);

            var se = new SXBootloader(stream);
            se.Read();
            bootloaders.Add(se);
        }

    }
}
