using System;
using System.IO;
using XeLib.Compression;
using XeLib.Utilities;
using XeLib.IO;

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

        public void WriteNewKernel(Stream kernel, Stream output) {
            var writer = new XeWriter(output);

            writer.WriteUInt16(magic);
            writer.WriteUInt16(version);
            writer.WriteUInt16(unkWord1);
            writer.WriteUInt16(unkWord2);
            writer.WriteUInt32(entryPoint);
            writer.WriteUInt32(0xdeadbeef); // Will need to go back and update after compression
            writer.Write(hmacSalt, 0, 0x10);

            var meta = new byte[0x10];
            BufferUtils.FromUInt32((uint)kernel.Length, meta, 0x08);

            writer.Write(meta, 0, 0x10);

            var compressedKernelStream = new MemoryStream();
            CompressKernel(kernel, compressedKernelStream);
            compressedKernelStream.Seek(0, SeekOrigin.Begin);
            compressedKernelStream.CopyTo(output);
            compressedKernelStream.Close();

            writer.Seek(0x0c, SeekOrigin.Begin);
            writer.WriteUInt32((uint)writer.Length);
        }

        // Not sure if this compression method is used elsewhere so keep it here for now. Move to more common location if transpires it is used elsewhere.
        public static void CompressKernel(Stream input, Stream output) {
            var writer = new XeWriter(output);

            var lzx = new LzxCompression();

            byte[] compressedBuffer = new byte[0x8000 + 0x1800];
            byte[] uncompressedBuffer = new byte[0x8000];

            while (input.Position < input.Length) {
                int blockSize = 0x8000;
                int remaining = (int)(input.Length - input.Position);

                if (remaining < blockSize) {
                    throw new NotImplementedException();
                }

                input.Read(uncompressedBuffer, 0, blockSize);

                int compressedSize = lzx.CompressSingle(uncompressedBuffer, blockSize, ref compressedBuffer, compressedBuffer.Length);

                writer.WriteUInt16((ushort)compressedSize);
                writer.WriteUInt16((ushort)blockSize);
                writer.Write(compressedBuffer, 0, compressedSize);
            }
        }

        public string GetKernelFileName() {
            return String.Format("xboxkrnl.{0}.exe", this.version);
        }
    }
}
