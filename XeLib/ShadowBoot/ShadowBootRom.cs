using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using XeLib.IO;
using XeLib.Bootloaders;
using XeLib.Security;

namespace XeLib.ShadowBoot
{
    public class ShadowBootRom
    {
        public Stream Stream { get; protected set; }

        public ushort magic; // 0x00
        public ushort version; // 0x02
        public uint bootloaderOffset; // 0x08
        public uint length; // 0x0c
        public string copyright; // 0x10-0x46
        public uint kvOffset; // 0x60;
        public uint smcSize; // 0x78
        public uint smcOffset; // 0x7d

        public SMC SMC;

        public CXBootloader SB;
        public CXBootloader SC;
        public CXBootloader SD;
        public CXBootloader SE;

        public ShadowBootRom(Stream stream) {
            Stream = stream;
            Read();
        }

        public void Read() {
            var reader = new XeReader(Stream);
            Read(reader);
        }

        public void Read(XeReader reader) {
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

            reader.Seek(smcOffset, SeekOrigin.Begin);
            SMC = new SMC();
            SMC.Read(reader, (int)smcSize);

            reader.Seek(bootloaderOffset, SeekOrigin.Begin);

            SB = new CXBootloader(Stream);

            SC = new CXBootloader(Stream);

            SD = new CXBootloader(Stream);

            SE = new CEBootloader(Stream);
        }

    }
}
