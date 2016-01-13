using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace XeLib.Compression
{
    public class LZX
    {
        [DllImport("LZX.dll", EntryPoint = "CreateLZX")]
        public static extern uint CreateLZX();
    }
}
