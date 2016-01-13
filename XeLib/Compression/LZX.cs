using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace XeLib.Compression
{
    public class LZX
    {
        [DllImport("LZX.dll", EntryPoint = "LZX_Create", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint LZX_Create(ref IntPtr context);
        
        [DllImport("LZX.dll", EntryPoint = "LZX_Decompress", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint LZX_Decompress(IntPtr context, byte[] src, ushort srcLength, byte[] dst, ref uint dstLength);

        [DllImport("LZX.dll", EntryPoint = "LZX_Destroy", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint LZX_Destroy(IntPtr context);
    }
}
