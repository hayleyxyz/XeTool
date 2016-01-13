using System;
using System.Collections.Generic;
using System.Text;
using XeLib.IO;

namespace XeLib.Bootloaders
{
    public class SMC
    {
        public byte[] data;

        public void Read(XeReader reader, int length) {
            data = new byte[length];
            reader.Read(data, 0, length);
        }

        // TODO: SMC CRC calculation

        public static void Decrypt(ref byte[] data) {
            // TODO: Reverse engineer from scratch (currently taken from 360 Flash Tool src)
            var key = new byte[] { 0x42, 0x75, 0x4E, 0x79 };
            int i = 0, length = data.Length, index = 0;

            while(length-- > 0) {
                int mod = data[index] * 0xFB;
                data[index++] ^= key[i];
                i++; i &= 3;
                key[i] += (byte)mod;
                key[(i + 1) & 3] += (byte)(mod >> 8);
            }
        }
    }
}
