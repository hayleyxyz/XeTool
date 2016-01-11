using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using XeLib.IO;
using XeLib.Bootloader;
using XeLib.Cryptography;

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

            var hmacKey = new byte[] { 0xDD, 0x88, 0xAD, 0x0C, 0x9E, 0xD6, 0x69, 0xE7, 0xB5, 0x67, 0x94, 0xFB, 0x68, 0x56, 0x3E, 0xFA };

            var hash = XeCrypt.XeCryptHmacSha(hmacKey, sb.data, 0, 0x10);

            var rc4Key = new byte[0x10];
            Buffer.BlockCopy(hash, 0, rc4Key, 0, 0x10);

            var rc4 = XeCrypt.XeCryptRc4Key(rc4Key);
            XeCrypt.XeCryptRc4Ecb(rc4, ref sb.data, 0x10, sb.data.Length - 0x10);

            File.WriteAllBytes("C:\\SB.bin", sb.data);

            var sc = new SXBootloader(stream);
            sc.Read();
            bootloaders.Add(sc);

            hmacKey = new byte[] { 0xF1, 0x98, 0x9B, 0xD8, 0x00, 0x95, 0x4A, 0x2A, 0xEA, 0x45, 0x5B, 0xB9, 0x89, 0x94, 0x9E, 0x07 };

            hash = XeCrypt.XeCryptHmacSha(hmacKey, sc.data, 0, 0x10);

            rc4Key = new byte[0x10];
            Buffer.BlockCopy(hash, 0, rc4Key, 0, 0x10);

            rc4 = XeCrypt.XeCryptRc4Key(rc4Key);
            XeCrypt.XeCryptRc4Ecb(rc4, ref sc.data, 0x10, sc.data.Length - 0x10);

            File.WriteAllBytes("C:\\SC.bin", sc.data);

            var sd = new SXBootloader(stream);
            sd.Read();
            bootloaders.Add(sd);

            var se = new SXBootloader(stream);
            se.Read();
            bootloaders.Add(se);
        }

    }
}
