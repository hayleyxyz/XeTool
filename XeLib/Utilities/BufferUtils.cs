using System;
using System.Collections.Generic;
using System.Text;
using XeLib.IO;

namespace XeLib.Utilities
{
    public static class BufferUtils
    {
        private static byte[] internalBuffer = new byte[0x4];

        public static void Reverse(byte[] buffer) {
            Array.Reverse(buffer);
        }

        public static int ToInt32(byte[] buffer, int startIndex = 0, Endian endian = Endian.Big) {
            if(endian == Endian.Little) {
                return buffer[startIndex + 3] << 24 |
                    buffer[startIndex + 2] << 16 |
                    buffer[startIndex + 1] << 8 |
                    buffer[startIndex + 0];
            }
            else {
                return buffer[startIndex + 0] << 24 |
                    buffer[startIndex + 1] << 16 |
                    buffer[startIndex + 2] << 8 |
                    buffer[startIndex + 3];
            }
        }

        public static uint ToUInt32(byte[] buffer, int startIndex = 0, Endian endian = Endian.Big) {
            if (endian == Endian.Little) {
                return (uint)(buffer[startIndex + 3] << 24 |
                    buffer[startIndex + 2] << 16 |
                    buffer[startIndex + 1] << 8 |
                    buffer[startIndex + 0]);
            }
            else {
                return (uint)(buffer[startIndex + 0] << 24 |
                    buffer[startIndex + 1] << 16 |
                    buffer[startIndex + 2] << 8 |
                    buffer[startIndex + 3]);
            }
        }

        public static short ToInt16(byte[] buffer, int startIndex = 0, Endian endian = Endian.Big) {

            if(endian == Endian.Little) {
                return (short)(buffer[startIndex + 1] << 8 |
                    buffer[startIndex + 0]);
            }
            else {
                return (short)(buffer[startIndex + 0] << 8 |
                    buffer[startIndex + 1]);
            }
        }

        public static ushort ToUInt16(byte[] buffer, int startIndex = 0, Endian endian = Endian.Big) {

            if (endian == Endian.Little) {
                return (ushort)(buffer[startIndex + 1] << 8 |
                    buffer[startIndex + 0]);
            }
            else {
                return (ushort)(buffer[startIndex + 0] << 8 |
                    buffer[startIndex + 1]);
            }
        }

        public static sbyte ToInt8(byte[] buffer, int startIndex = 0) {
            return (sbyte)buffer[0];
        }

        public static byte ToUInt8(byte[] buffer, int startIndex = 0) {
            return buffer[0];
        }

        public static void FromUInt16(ushort value, byte[] buffer, int startIndex = 0, Endian endian = Endian.Big) {
            if(endian == Endian.Little) {

            }
            else {
                buffer[0] = (byte)((value >> 8) & 0xff);
                buffer[1] = (byte)(value & 0xff);
            }
        }
    }
}
