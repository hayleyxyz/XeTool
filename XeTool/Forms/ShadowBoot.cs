using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using XeLib.IO;
using XeLib.ShadowBoot;
using XeLib.Security;
using XeLib.Bootloaders;
using XeLib.Compression;

namespace XeTool.Forms {
    public partial class ShadowBoot : Form {

        protected ShadowBootRom rom;

        protected string currentFile;

        public ShadowBoot() {
            InitializeComponent();
        }

        private void openMenuItem_Click(object sender, EventArgs e) {
            var ofd = new OpenFileDialog();

            if(ofd.ShowDialog() == DialogResult.OK) {
                loadFile(ofd.FileName);
            }
        }

        protected void loadFile(string file) {
            currentFile = file;

            var stream = File.OpenRead(file);
            var reader = new XeReader(stream);

            rom = new ShadowBootRom();
            rom.Read(reader);

            bl2Version.Text = rom.SB.version.ToString();
            bl3Version.Text = rom.SC.version.ToString();
            bl4Version.Text = rom.SD.version.ToString();
            bl5Version.Text = rom.SE.version.ToString();

            extractAllButton.Enabled = true;

            stream.Close();
        }

        private void extractAllButton_Click(object sender, EventArgs e) {
            var fbd = new FolderBrowserDialog();

            fbd.SelectedPath = Directory.GetParent(currentFile).FullName;

            if (fbd.ShowDialog() == DialogResult.OK) {
                var dir = fbd.SelectedPath;

                #region SB
                Bootloader.Decrypt(ref rom.SB.data, StaticKeys.BL1_KEY); // SB

                File.WriteAllBytes(String.Format("{0}\\{1}.{2}.bin", dir, rom.SB.GetMagicAsString(), rom.SB.version), rom.SB.data);
                #endregion

                #region SC
                byte[] scDigest = new byte[0x10];
                Bootloader.Decrypt(ref rom.SC.data, new byte[0x10], ref scDigest); // SC

                File.WriteAllBytes(String.Format("{0}\\{1}.{2}.bin", dir, rom.SC.GetMagicAsString(), rom.SC.version), rom.SC.data);
                #endregion

                #region SD
                byte[] sdDigest = new byte[0x10];
                Bootloader.Decrypt(ref rom.SD.data, scDigest, ref sdDigest); // SD

                File.WriteAllBytes(String.Format("{0}\\{1}.{2}.bin", dir, rom.SD.GetMagicAsString(), rom.SD.version), rom.SD.data);
                #endregion

                #region SE
                Bootloader.Decrypt(ref rom.SE.data, sdDigest); // SE

                //rom.SE.data[0x4c] = (byte)~rom.SE.data[0x4c];

                var decryptedSe = new byte[rom.SE.data.Length];
                Buffer.BlockCopy(rom.SE.data,0 , decryptedSe, 0, decryptedSe.Length);

                File.WriteAllBytes(String.Format("{0}\\{1}.{2}.bin", dir, rom.SE.GetMagicAsString(), rom.SE.version), rom.SE.data);

                Bootloader.Decrypt(ref rom.SE.data, sdDigest); // SE

                File.WriteAllBytes(String.Format("{0}\\{1}.{2}.enc.bin", dir, rom.SE.GetMagicAsString(), rom.SE.version), rom.SE.data);
                #endregion

                #region Kernel/HV
                var kernelFile = String.Format("{0}\\xboxkrnl.exe", dir);
                var output = File.Open(kernelFile, FileMode.Create);
                var input = new MemoryStream(decryptedSe);

                input.Seek(0x30, SeekOrigin.Begin);

                var lzx = new LZX();
                lzx.DecompressContinuous(input, output);

                output.Seek(0, SeekOrigin.Begin);
                var hv = new XeLib.Binaries.Hypervisor();
                hv.Read(new XeReader(output));

                output.Close();

                var kernelFileWithVersion = String.Format("{0}\\xboxkrnld.{1}.exe", dir, hv.version);

                if(File.Exists(kernelFileWithVersion)) {
                    File.Delete(kernelFileWithVersion);
                }

                File.Move(kernelFile, kernelFileWithVersion);
                #endregion
            }
        }

        private void ShadowBoot_DragEnter(object sender, DragEventArgs e) {
            e.Effect = DragDropEffects.Copy;
        }

        private void ShadowBoot_DragDrop(object sender, DragEventArgs e) {
            var files = (string[])e.Data.GetData(DataFormats.FileDrop, false);

            if(files.Length > 0) {
                loadFile(files[0]);
            }
        }
    }
}
