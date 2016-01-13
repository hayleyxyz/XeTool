using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using XeLib.IO;
using XeLib.Utilities;
using XeLib.Security;

namespace XeLib.Bootloaders
{
    public class Bootloader
    {
        public ushort magic; // 0x00
        public ushort version; // 0x02
        public uint entryPoint; // 0x08
        public uint length; // 0x0c
        public byte[] data;

        public void Read(XeReader reader) {
            // First read the header
            var header = new byte[0x10];
            reader.Read(header, 0, 0x10);

            // Parse info bits
            magic = BufferUtils.ToUInt16(header, 0x00);
            version = BufferUtils.ToUInt16(header, 0x02);
            entryPoint = BufferUtils.ToUInt32(header, 0x08);
            length = BufferUtils.ToUInt32(header, 0x0c);

            // Now that we have the length of the BL we can go back and read the entire thing
            data = new byte[length];
            Buffer.BlockCopy(header, 0, data, 0, header.Length);
            reader.Read(data, 0x10, (int)length - 0x10);
        }

        public string GetMagicAsString() {
            var buf = new byte[2];
            BufferUtils.FromUInt16(magic, buf);
            return Encoding.UTF8.GetString(buf);
        }

        public static void Decrypt(ref byte[] inOut, byte[] hmacKey) {
            var outDigest = new byte[0x10];
            Decrypt(ref inOut, hmacKey, ref outDigest);
        }

        public static void Decrypt(ref byte[] inOut, byte[] hmacKey, ref byte[] digest) {
            // Hash 0x10 bytes starting at 0x10
            var hash = XeCrypt.XeCryptHmacSha(hmacKey, inOut, 0x10, 0x10);

            // Copy resulting digest to parameter for use in decrpyting next BL 
            if (digest != null) {
                Buffer.BlockCopy(hash, 0, digest, 0, 0x10);
            }

            // We only need the first 0x10 bytes of the resulting hash
            var rc4Key = new byte[0x10];
            Buffer.BlockCopy(hash, 0, rc4Key, 0, 0x10);

            // Decrypt the data
            var rc4 = XeCrypt.XeCryptRc4Key(rc4Key);
            XeCrypt.XeCryptRc4Ecb(rc4, ref inOut, 0x20, inOut.Length - 0x20);
        }
    }
}
