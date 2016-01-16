using System;
using System.IO;

namespace XeLib.Compression
{
    public unsafe class LzxCompression {
        protected IntPtr context;

        protected byte[] dst;

        public LzxCompression() {
            uint dataBlockMax = 0x8000;

            var config = new LzxNativeMethods.LZX_CONFIGURATION();
            config.CompressionWindowSize = 0x20000;
            config.SecondPartitionSize = 1;

            context = IntPtr.Zero;

            uint pcbDstBufferMin = 0;

            uint result = LzxNativeMethods.LCICreateCompression(out dataBlockMax, &config, LzxNativeMethods.managed_mem_alloc, LzxNativeMethods.managed_mem_free, out pcbDstBufferMin, out context, delegate (IntPtr fci_data, byte* pbCompressed, uint cbCompressed, uint cbUncompressed) {
                dst = new byte[cbCompressed];

                for (var i = 0; i < cbCompressed; i++) {
                    dst[i] = *pbCompressed;
                    pbCompressed++;
                }
            }, IntPtr.Zero);
        }

        public int CompressSingle(byte[] input, int inputLength, ref byte[] output, int outputLength) {
            uint dstResultLength = 0;

            uint result = LzxNativeMethods.LCICompress(context, input, (uint)inputLength, null, (uint)outputLength, out dstResultLength);

            result = LzxNativeMethods.LCIFlushCompressorOutput(context);

            int compressedLength = dst.Length;
            Buffer.BlockCopy(dst, 0, output, 0, compressedLength);
            dst = null;

            return compressedLength;
        }

        public void CompressContinuous(Stream input, Stream output) {
            throw new NotImplementedException();
            while (input.Position < input.Length) {
                int blockSize = 0x8000;
                
            }
        }
    }
}
