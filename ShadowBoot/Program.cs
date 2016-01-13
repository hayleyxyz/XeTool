using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using XeLib.ShadowBoot;
using XeLib.Bootloaders;
using XeLib.IO;
using XeLib.Security;

namespace ShadowBoot {
    class Program {

        static void Main(string[] args) {
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
            var shouldDecrypt = (extractOp[0] == "d");
            var file = extractOp[1];
            var dir = extractOp[2];

            var stream = File.Open(file, FileMode.Open);
            var reader = new XeReader(stream);

            var rom = new ShadowBootRom();
            rom.Read(reader);

            #region SB
            if (shouldDecrypt) {
                Bootloader.Decrypt(ref rom.SB.data, StaticKeys.BL1_KEY); // SB
            }

            File.WriteAllBytes(String.Format("{0}\\{1}.bin", dir, rom.SB.GetMagicAsString()), rom.SB.data);
            #endregion

            #region SC
            byte[] scDigest = null;

            if (shouldDecrypt) {
                scDigest = new byte[0x10];
                Bootloader.Decrypt(ref rom.SC.data, new byte[0x10], ref scDigest); // SC
            }

            File.WriteAllBytes(String.Format("{0}\\{1}.bin", dir, rom.SC.GetMagicAsString()), rom.SC.data);
            #endregion

            #region SD
            byte[] sdDigest = null;

            if (shouldDecrypt) {
                sdDigest = new byte[0x10];
                Bootloader.Decrypt(ref rom.SD.data, scDigest, ref sdDigest); // SD
            }

            File.WriteAllBytes(String.Format("{0}\\{1}.bin", dir, rom.SD.GetMagicAsString()), rom.SD.data);
            #endregion

            #region SD
            if (shouldDecrypt) {
                Bootloader.Decrypt(ref rom.SE.data, sdDigest); // SC
            }

            File.WriteAllBytes(String.Format("{0}\\{1}.bin", dir, rom.SE.GetMagicAsString()), rom.SE.data);
            #endregion

            #region SMC
            SMC.Decrypt(ref rom.SMC.data);
            File.WriteAllBytes(String.Format("{0}\\SMC.bin", dir), rom.SMC.data);
            #endregion

            stream.Close();
        }

        static void PrintFileInfo(string file) {
            var stream = File.Open(file, FileMode.Open);
            var reader = new XeReader(stream);

            var rom = new ShadowBootRom();
            rom.Read(reader);

            Console.WriteLine("Version: {0}", rom.version);
            Console.WriteLine("Bootloaders offset: 0x{0:x8}", rom.bootloaderOffset);

            Console.WriteLine("Bootloaders:");

            Console.WriteLine("\t{0}", rom.SB.GetMagicAsString());
            Console.WriteLine("\tVersion: {0}", rom.SB.version);

            Console.WriteLine("\t{0}", rom.SC.GetMagicAsString());
            Console.WriteLine("\tVersion: {0}", rom.SC.version);

            Console.WriteLine("\t{0}", rom.SD.GetMagicAsString());
            Console.WriteLine("\tVersion: {0}", rom.SD.version);

            Console.WriteLine("\t{0}", rom.SE.GetMagicAsString());
            Console.WriteLine("\tVersion: {0}", rom.SE.version);

            Console.WriteLine();

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
