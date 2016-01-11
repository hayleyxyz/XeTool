using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using NDesk.Options;
using XeLib.ShadowBoot;

namespace ShadowBoot {
    class Program {

        List<string> options;

        static void Main(string[] args) {
            if (args.Length == 0) {
                PrintHelp();
            }

            var files = new List<string>();

            for(int i = 0; i < args.Length; i++) {
                if(args[i] == "-i") {
                    i++;
                    files.Add(args[i]);
                }
            }

            foreach(var file in files) {
                PrintFileInfo(file);
            }

            Console.ReadKey();
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

            stream.Close();
        }
 
        static void PrintHelp() {
            Console.WriteLine("Usage: {0} <options> <xbox rom .bin>", Process.GetCurrentProcess().ProcessName);
        }
    }
}
