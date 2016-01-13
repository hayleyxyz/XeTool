using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using XeLib.ShadowBoot;
using XeLib.Bootloader;

namespace ShadowBoot {
    class Program {

        List<string> options;

        static void Main(string[] args) {
            //fuckBitches();return;

            if (args.Length == 0) {
                PrintHelp();
            }

            var info = new List<string>();
            var extract = new List<string[]>();

            for(int i = 0; i < args.Length; i++) {
                if(args[i] == "-i") {
                    i++;
                    info.Add(args[i]);
                }
                else if(args[i] == "-x") {
                    var extractOp = new string[3];
                    Array.Copy(args, i + 1, extractOp, 0, 3);
                    extract.Add(extractOp);
                    i += 3;
                }
            }

            foreach(var file in info) {
                PrintFileInfo(file);
            }

            foreach(var extractOp in extract) {
                ExtractFile(extractOp);
            }

            Console.ReadKey();
        }

        private static void ExtractFile(string[] extractOp) {
            var decrypted = (extractOp[0] == "d");
            var file = extractOp[1];
            var dir = extractOp[2];

            var stream = File.Open(file, FileMode.Open);
            var rom = new ShadowBootRom(stream);
            rom.Read();

            var bl2 = rom.bootloaders[0];
            var bl3 = rom.bootloaders[1];
            var bl4 = rom.bootloaders[2];
            var bl5 = rom.bootloaders[3];

            var bl1Key = new byte[] { 0xDD, 0x88, 0xAD, 0x0C, 0x9E, 0xD6, 0x69, 0xE7, 0xB5, 0x67, 0x94, 0xFB, 0x68, 0x56, 0x3E, 0xFA };

            var bl2Digest = new byte[0x10];

            Bootloader.Decrypt(ref bl2.data, bl1Key, ref bl2Digest); // SB

            var bl3Digest = new byte[0x10];
            var bl4Digest = new byte[0x10];

            Bootloader.Decrypt(ref bl3.data, new byte[0x10], ref bl3Digest); // SC
            Bootloader.Decrypt(ref bl4.data, bl3Digest, ref bl4Digest); // SD
            Bootloader.Decrypt(ref bl5.data, bl4Digest); // SE

            File.WriteAllBytes(String.Format("{0}\\{1}_Dec.bin", dir, bl2.GetMagicAsString()), bl2.data);
            File.WriteAllBytes(String.Format("{0}\\{1}_Dec.bin", dir, bl3.GetMagicAsString()), bl3.data);
            File.WriteAllBytes(String.Format("{0}\\{1}_Dec.bin", dir, bl4.GetMagicAsString()), bl4.data);
            File.WriteAllBytes(String.Format("{0}\\{1}_Dec.bin", dir, bl5.GetMagicAsString()), bl5.data);


            foreach (var bl in rom.bootloaders) {
                var dest = String.Format("{0}\\{1}.bin", dir, bl.GetMagicAsString());

                if(decrypted) {
                    throw new NotImplementedException();
                }

                File.WriteAllBytes(dest, bl.data);
            }
        }

        static void fuckBitches() {
            //var cb = File.OpenRead(@"Z:\Xbox360\Bootloaders\XBR_Xenon_8955_3\Encrypted\CB.bin");
            //var cd = File.OpenRead(@"Z:\Xbox360\Bootloaders\XBR_Xenon_8955_3\Encrypted\CD.bin");

            var cb = File.OpenRead(@"Z:\Xbox360\ShadowBoot\11775\Encrypted\SB.bin");
            var cd = File.OpenRead(@"Z:\Xbox360\ShadowBoot\11775\Encrypted\SC.bin");

            var bl1Key = new byte[] { 0xDD, 0x88, 0xAD, 0x0C, 0x9E, 0xD6, 0x69, 0xE7, 0xB5, 0x67, 0x94, 0xFB, 0x68, 0x56, 0x3E, 0xFA };

            var bl2 = new Bootloader(cb);
            bl2.Read();

            var bl2Digest = new byte[0x10];

            Bootloader.Decrypt(ref bl2.data, bl1Key, ref bl2Digest);

            var bl3 = new Bootloader(cd);
            bl3.Read();

            var bl3Digest = new byte[0x10];

            Bootloader.Decrypt(ref bl3.data, bl2Digest, ref bl3Digest);

            File.WriteAllBytes(@"Z:\Xbox360\ShadowBoot\11775\Encrypted\SB_decrypted.bin", bl2.data);
            File.WriteAllBytes(@"Z:\Xbox360\ShadowBoot\11775\Encrypted\SC_decrypted.bin", bl3.data);

            cb.Close();
            cd.Close();
        }

        static void PrintFileInfo(string file) {
            var stream = File.Open(file, FileMode.Open);
            var rom = new ShadowBootRom(stream);
            rom.Read();

            Console.WriteLine("Version: {0}", rom.version);
            Console.WriteLine("Bootloaders offset: 0x{0:x8}", rom.bootloaderOffset);

            Console.WriteLine("Bootloaders:");

            foreach(var bl in rom.bootloaders) {
                Console.WriteLine("\t{0}", bl.GetMagicAsString());
                Console.WriteLine("\tVersion: {0}", bl.version);
            }

            var sbKey = new byte[] { 0xDD, 0x88, 0xAD, 0x0C, 0x9E, 0xD6, 0x69, 0xE7, 0xB5, 0x67, 0x94, 0xFB, 0x68, 0x56, 0x3E, 0xFA };

            var sbDigest = new byte[0x10];

            //SXBootloader.Decrypt(ref rom.bootloaders[0].data, sbKey, ref sbDigest);

            //File.WriteAllBytes("Z:\\Xbox360\\ShadowBoot\\11775\\SB.bin", rom.bootloaders[0].data);

            var scDigest = new byte[0x10];

            //SXBootloader.Decrypt(ref rom.bootloaders[1].data, sbDigest, ref scDigest);

            //File.WriteAllBytes("Z:\\Xbox360\\ShadowBoot\\11775\\SC.bin", rom.bootloaders[1].data);

            stream.Close();
        }
 
        static void PrintHelp() {
            Console.WriteLine("Usage: {0} <options>", Process.GetCurrentProcess().ProcessName);
            Console.WriteLine("Options:");
            Console.WriteLine("\t-i <xbox rom .bin>                                             Print information about a shadowboot rom file.");
            Console.WriteLine("\t-x <d(ecrypted)|e(ncrypted)> <xbox rom .bin> <output dir>      Extract files from file.");
        }
    }
}
