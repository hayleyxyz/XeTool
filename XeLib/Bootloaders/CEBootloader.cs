using System;
using System.IO;
using XeLib.Compression;
using XeLib.Utilities;

namespace XeLib.Bootloaders
{
    public class CEBootloader : CXBootloader
    {
        public CEBootloader(Stream stream) : base(stream) {

        }

        public void ExtractKernel(Stream output, byte[] key) {
            var decrypted = new MemoryStream();
            this.Decrypt(decrypted, key);

            decrypted.Seek(0x30, SeekOrigin.Begin);

            var lzx = new LzxDecompression();
            lzx.DecompressContinuous(decrypted, output);

            decrypted.Close();
        }

        public string GetKernelFileName() {
            return String.Format("xboxkrnl.{0}.exe", this.version);
        }
    }
}
