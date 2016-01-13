using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;
using XeLib.Utilities;

namespace XeLib.Compression
{
    public class LZX
    {
        [DllImport("LZX.dll", EntryPoint = "LZX_Create", CallingConvention = CallingConvention.Cdecl)]
        protected static extern uint LZX_Create(ref IntPtr context);
        
        [DllImport("LZX.dll", EntryPoint = "LZX_Decompress", CallingConvention = CallingConvention.Cdecl)]
        protected static extern uint LZX_Decompress(IntPtr context, byte[] src, ushort srcLength, byte[] dst, ref uint dstLength);

        [DllImport("LZX.dll", EntryPoint = "LZX_Destroy", CallingConvention = CallingConvention.Cdecl)]
        protected static extern uint LZX_Destroy(IntPtr context);

        protected IntPtr context;

        public LZX() {
            uint result = LZX_Create(ref context);

            if(result != 0) {
                throw new LZXException("LZX_Create() failed");
            }
        }

        ~LZX() {
            LZX_Destroy(context);
            context = IntPtr.Zero;
        }

        public int DecompressSingle(byte[] input, int inputLength, ref byte[] output, int outputLength) {
            uint outSize = (uint)outputLength;

            uint result = LZX.LZX_Decompress(context, input, (ushort)inputLength, output, ref outSize);

            if (result != 0) {
                throw new LZXException("LZX_Decompresss() failed");
            }

            return (int)outSize;
        }

        public void DecompressContinuous(Stream input, Stream output) {
            var sizes = new byte[4];
            byte[] compressedBuffer = null;
            byte[] uncompressedBuffer = null;

            while (input.Position < input.Length) {
                input.Read(sizes, 0, 4);

                var compressedLength = BufferUtils.ToUInt16(sizes, 0);
                var uncompressedLength = BufferUtils.ToUInt16(sizes, 2);

                if((input.Position + compressedLength) > input.Length) {
                    throw new LZXException("Data is corrupt.");
                }

                if(compressedBuffer == null) {
                    compressedBuffer = new byte[compressedLength];
                }

                if(uncompressedBuffer == null) {
                    uncompressedBuffer = new byte[uncompressedLength];
                }

                if(compressedBuffer.Length < compressedLength) {
                    Array.Resize(ref compressedBuffer, compressedLength);
                }

                if(uncompressedBuffer.Length < uncompressedLength) {
                    Array.Resize(ref uncompressedBuffer, uncompressedLength);
                }

                input.Read(compressedBuffer, 0, compressedLength);

                DecompressSingle(compressedBuffer, compressedLength, ref uncompressedBuffer, uncompressedLength);

                output.Write(uncompressedBuffer, 0, uncompressedLength);
            }
        }
    }
}
