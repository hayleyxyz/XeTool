using System;
using System.Runtime.InteropServices;

namespace XeLib.Compression
{
    internal unsafe static class LzxNativeMethods
    {
        public struct LZX_CONFIGURATION {
            public uint CompressionWindowSize;
            public uint SecondPartitionSize;
        }

        #region Compression
        [DllImport("XeLibNative.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint LCICreateCompression(out uint pcbDataBlockMax, LZX_CONFIGURATION* pvConfiguration, pfnma pfnma, pfnmf pfnmf, out uint pcbDstBufferMin, out IntPtr pmchHandle, pfnlzx_output_callback pfnCallback, IntPtr fci_data);

        [DllImport("XeLibNative.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint LCICompress(IntPtr hmc, byte[] pbSrc, uint cbSrc, byte[] pbDst, uint cbDst, out uint pcbResult);

        [DllImport("XeLibNative.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint LCIFlushCompressorOutput(IntPtr hmc);

        [DllImport("XeLibNative.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint LCIDestroyCompression(IntPtr hmc);
        #endregion

        #region Decompression
        [DllImport("XeLibNative.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint LDICreateDecompression(out uint pcbDataBlockMax, LZX_CONFIGURATION* pvConfiguration, pfnma pfnma, pfnmf pfnmf, out uint pcbSrcBufferMin, out IntPtr pmchHandle, IntPtr pcbDecompressed);

        [DllImport("XeLibNative.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "LDIDecompress")]
        public static extern uint LDIDecompress(IntPtr hmc, byte[] pbSrc, uint cbSrc, byte[] pbDst, out uint pcbResult);

        [DllImport("XeLibNative.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint LDIDestroyDecompression(IntPtr hmc);
        #endregion

        #region Helpers
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IntPtr pfnma(uint cb);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void pfnmf(IntPtr pv);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void pfnlzx_output_callback(IntPtr fci_data, byte* pbCompressed, uint cbCompressed, uint cbDecompressed);

        [DllImport("XeLibNative.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr mem_alloc(uint cb);

        [DllImport("XeLibNative.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void mem_free(IntPtr pv);

        public static IntPtr managed_mem_alloc(uint cb) {
            return mem_alloc(cb);
        }

        public static void managed_mem_free(IntPtr pv) {
            mem_free(pv);
        }
        #endregion
    }
}
