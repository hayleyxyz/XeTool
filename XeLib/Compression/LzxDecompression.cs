using System;
using System.IO;
using XeLib.Utilities;

namespace XeLib.Compression
{
    public unsafe class LzxDecompression
    {
        protected IntPtr context;

        public LzxDecompression() {
            uint dataBlockMax = 0x8000;

            var config = new LzxNativeMethods.LZX_CONFIGURATION();
            config.CompressionWindowSize = 0x20000;
            config.SecondPartitionSize = 1;

            uint decompressBufferMin = 0;

            context = IntPtr.Zero;

            uint result = LzxNativeMethods.LDICreateDecompression(out dataBlockMax, &config, LzxNativeMethods.managed_mem_alloc, LzxNativeMethods.managed_mem_free, out decompressBufferMin, out context, IntPtr.Zero);
        }

        ~LzxDecompression() {
            if(context != null) {
                uint result = LzxNativeMethods.LDIDestroyDecompression(context);
            }
        }

        public int DecompressSingle(byte[] input, int inputLength, ref byte[] output, int outputLength) {
            uint outSize = (uint)outputLength;

            uint result = LzxNativeMethods.LDIDecompress(context, input, (uint)inputLength, output, out outSize);

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

                if ((input.Position + compressedLength) > input.Length) {
                    throw new Exception("Data is corrupt.");
                }

                if (compressedBuffer == null) {
                    compressedBuffer = new byte[compressedLength];
                }

                if (uncompressedBuffer == null) {
                    uncompressedBuffer = new byte[uncompressedLength];
                }

                if (compressedBuffer.Length < compressedLength) {
                    Array.Resize(ref compressedBuffer, compressedLength);
                }

                if (uncompressedBuffer.Length < uncompressedLength) {
                    Array.Resize(ref uncompressedBuffer, uncompressedLength);
                }

                input.Read(compressedBuffer, 0, compressedLength);

                DecompressSingle(compressedBuffer, compressedLength, ref uncompressedBuffer, uncompressedLength);

                output.Write(uncompressedBuffer, 0, uncompressedLength);
            }
        }
    }
}
