using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.IO;
using System.Diagnostics;
using XeLib.IO;

namespace LzxManaged {
    public partial class Form1 : Form {

        static unsafe class NativeMethods {
            public struct LZX_CONFIGURATION {
                public uint CompressionWindowSize;
                public uint SecondPartitionSize;
            }

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate IntPtr pfnma(uint cb);

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void pfnmf(IntPtr pv);

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void pfnlzx_output_callback(IntPtr fci_data, byte* pbCompressed, uint cbCompressed, uint cbUncompressed);

            [DllImport("XeLibNative.dll", CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr mem_alloc(uint cb);

            [DllImport("XeLibNative.dll", CallingConvention = CallingConvention.Cdecl)]
            public static extern void mem_free(IntPtr pv);

            [DllImport("XeLibNative.dll", CallingConvention = CallingConvention.Cdecl)]
            public static extern uint LCICreateCompression(out uint pcbDataBlockMax, LZX_CONFIGURATION* pvConfiguration, pfnma pfnma, pfnmf pfnmf, out uint pcbDstBufferMin, out IntPtr pmchHandle, pfnlzx_output_callback pfnCallback, IntPtr fci_data);

            [DllImport("XeLibNative.dll", CallingConvention = CallingConvention.Cdecl)]
            public static extern uint LCICompress(IntPtr hmc, byte[] pbSrc, uint cbSrc, byte[] pbDst, uint cbDst, out uint pcbResult);

            [DllImport("XeLibNative.dll", CallingConvention = CallingConvention.Cdecl)]
            public static extern uint LCIFlushCompressorOutput(IntPtr hmc);

            [DllImport("XeLibNative.dll", CallingConvention = CallingConvention.Cdecl)]
            public static extern uint LCIDestroyCompression(IntPtr hmc);

            //[DllImport("XeLibNative.dll", CallingConvention = CallingConvention.Cdecl)]
            //public static extern uint LDICreateDecompression(out uint pcbDataBlockMax, LZX_CONFIGURATION* pvConfiguration, pfnma pfnma, pfnmf pfnmf, out uint pcbSrcBufferMin, out IntPtr pmdhHandle, IntPtr pvmem);

            /*

                DWORD LDICreateDecompression(
		            LPDWORD pcbDataBlockMax,
		            LPVOID pvConfiguration,
		            LPVOID pfnma,
		            LPVOID pfnmf,
		            LPVOID pcbSrcBufferMin,
		            LPVOID* ppContext,
		            LPDWORD pcbDecompressed
	            );


            */

            [DllImport("XeLibNative.dll", CallingConvention = CallingConvention.Cdecl)]
            public static extern uint LDICreateDecompression(out uint pcbDataBlockMax, LZX_CONFIGURATION* pvConfiguration, pfnma pfnma, pfnmf pfnmf, out uint pcbSrcBufferMin, out IntPtr pmchHandle, IntPtr pcbDecompressed);

            /*
                DWORD LDIDecompress(
		            LPVOID context,
		            LPBYTE pbSrc,
		            WORD cbSrc,
		            LPBYTE pdDst,
		            LPDWORD pcbDecompressed
		        );
            */

            [DllImport("XeLibNative.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "LDIDecompress")]
            public static extern uint LDIDecompress(IntPtr hmc, byte[] pbSrc, uint cbSrc, byte[] pbDst, out uint pcbResult);

            //[DllImport("XeLibNative.dll", CallingConvention = CallingConvention.Cdecl)]
            //public static extern uint LDIDecompress(IntPtr hmd, byte[] pbSrc, uint cbSrc, byte[] pbDst, uint cbDst, uint* pcbResult);

            [DllImport("XeLibNative.dll", CallingConvention = CallingConvention.Cdecl)]
            public static extern uint LDIDestroyDecompression(IntPtr hmc);
        }

        public Form1() {
            InitializeComponent();
        }

        public static IntPtr mem_alloc(uint cb) {
            return NativeMethods.mem_alloc(cb);
        }

        public static void mem_free(IntPtr pv) {
            NativeMethods.mem_free(pv);
        }

        private unsafe void button1_Click(object sender, EventArgs e) {
            var sw = Stopwatch.StartNew();

            uint dataBlockMax = 0x8000;

            var config = new NativeMethods.LZX_CONFIGURATION();
            config.CompressionWindowSize = 0x20000;
            config.SecondPartitionSize = 1;

            IntPtr context = IntPtr.Zero;

            uint pcbDstBufferMin = 0;

            byte[] dst = null;

            uint result = NativeMethods.LCICreateCompression(out dataBlockMax, &config, mem_alloc, mem_free, out pcbDstBufferMin, out context, delegate (IntPtr fci_data, byte* pbCompressed, uint cbCompressed, uint cbUncompressed) {
                dst = new byte[cbCompressed];

                for (var i = 0; i < cbCompressed; i++) {
                    dst[i] = *pbCompressed;
                    pbCompressed++;
                }
            }, IntPtr.Zero);

            Debug.Print("LCICreateCompression() - {0:x}", result);

            var src = new byte[0x8000];
            uint dstResultLength = 0;

            var kernel = System.IO.File.ReadAllBytes(@"C:\temp\xboxkrnl.exe");
            Buffer.BlockCopy(kernel, 0, src, 0, src.Length);

            result = NativeMethods.LCICompress(context, src, (uint)src.Length, null, pcbDstBufferMin, out dstResultLength);

            Debug.Print("LCICompress() - {0:x}", result);

            result = NativeMethods.LCIFlushCompressorOutput(context);

            Debug.Print("LCIFlushCompressorOutput() - {0:x}", result);

            result = NativeMethods.LCIDestroyCompression(context);

            sw.Stop();

            Debug.Print("LCIDestroyCompression() - {0:x}", result);

            Debug.Print("{0}ms", sw.ElapsedMilliseconds);

            var lzxFile = @"C:\temp\xboxkrnl.lzx";
            if (File.Exists(lzxFile)) File.Delete(lzxFile);
            File.WriteAllBytes(lzxFile, dst);
        }

        private unsafe void button2_Click(object sender, EventArgs e) {
            var bl = File.OpenRead(@"C:\temp\SE.1888.bin");
            var reader = new XeReader(bl);

            reader.Seek(0x30, SeekOrigin.Begin);
            var compressedSize = reader.ReadUInt16();
            var decompressedSize = reader.ReadUInt16();

            var compressed = new byte[compressedSize];
            var decompressed = new byte[decompressedSize];

            reader.Read(compressed, 0, compressedSize);

            bl.Close();

            uint dataBlockMax = 0x8000;

            var config = new NativeMethods.LZX_CONFIGURATION();
            config.CompressionWindowSize = 0x20000;
            config.SecondPartitionSize = 1;

            uint decompressBufferMin = 0;

            IntPtr context = IntPtr.Zero;

            uint result = NativeMethods.LDICreateDecompression(out dataBlockMax, &config, mem_alloc, mem_free, out decompressBufferMin, out context, IntPtr.Zero);

            Debug.Print("LDICreateDecompression() - {0:x}", result);

            uint dstResultLength = decompressedSize;
            result = NativeMethods.LDIDecompress(context, compressed, compressedSize, decompressed, out dstResultLength);

            Debug.Print("LDIDecompress() - {0:x}", result);

            result = NativeMethods.LDIDestroyDecompression(context);

            Debug.Print("LDIDestroyDecompression() - {0:x}", result);

            var blDecompressed = @"C:\\temp\\decompressed_SE.bin";
            if (File.Exists(blDecompressed)) File.Delete(blDecompressed);
            File.WriteAllBytes(blDecompressed, decompressed);
        }
    }
}
