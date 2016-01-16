using System;
using System.IO;

namespace XeLib.Compression
{
    public unsafe class LzxDecompression
    {
        public LzxDecompression() {

        }

        public uint Decompress(byte[] input, ref byte[] output) {
            uint dataBlockMax = 0x8000;

            var config = new LzxNativeMethods.LZX_CONFIGURATION();
            config.CompressionWindowSize = 0x20000;
            config.SecondPartitionSize = 1;

            uint decompressBufferMin = 0;

            IntPtr context = IntPtr.Zero;

            uint result = LzxNativeMethods.LDICreateDecompression(out dataBlockMax, &config, LzxNativeMethods.mem_alloc, LzxNativeMethods.mem_free, out decompressBufferMin, out context, IntPtr.Zero);

            uint dstResultLength = (uint)output.Length;
            result = LzxNativeMethods.LDIDecompress(context, input, (uint)input.Length, output, out dstResultLength);

            result = LzxNativeMethods.LDIDestroyDecompression(context);

            return dstResultLength;
        }
    }
}
