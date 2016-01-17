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

            rom = new ShadowBootRom(stream);

            bl2Version.Text = rom.SB.version.ToString();
            bl3Version.Text = rom.SC.version.ToString();
            bl4Version.Text = rom.SD.version.ToString();
            bl5Version.Text = rom.SE.version.ToString();

            extractAllButton.Enabled = 
                replaceKernelButton.Enabled = true;
        }

        private void extractAllButton_Click(object sender, EventArgs e) {
            var fbd = new FolderBrowserDialog() {
                SelectedPath = Directory.GetParent(currentFile).FullName
            };

            var dstFileFormat = "{0}\\{1}";

            if (fbd.ShowDialog() == DialogResult.OK) {
                var dir = fbd.SelectedPath;

                var fs = File.Create(String.Format(dstFileFormat, fbd.SelectedPath, rom.SB.GetFileName()));
                rom.SB.Decrypt(fs, StaticKeys.BL1_KEY);
                fs.Close();

                fs = File.Create(String.Format(dstFileFormat, fbd.SelectedPath, rom.SC.GetFileName()));
                byte[] bl3Key;
                rom.SC.Decrypt(fs, new byte[0x10], out bl3Key);
                fs.Close();

                fs = File.Create(String.Format(dstFileFormat, fbd.SelectedPath, rom.SD.GetFileName()));
                byte[] bl4Key;
                rom.SD.Decrypt(fs, bl3Key, out bl4Key);
                fs.Close();

                fs = File.Create(String.Format(dstFileFormat, fbd.SelectedPath, rom.SE.GetFileName()));
                rom.SE.Decrypt(fs, bl4Key);
                fs.Close();

                fs = File.Create(String.Format(dstFileFormat, dir, rom.SE.GetKernelFileName()));
                rom.SE.ExtractKernel(fs, bl4Key);
                fs.Close();
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

        private void replaceKernelButton_Click(object sender, EventArgs e) {
            var ofd = new OpenFileDialog();
            var sfd = new SaveFileDialog() {
                Filter = "Binary Files (*.bin)|*.bin|All Files|*"
            };

            if (ofd.ShowDialog() == DialogResult.OK && sfd.ShowDialog() == DialogResult.OK) {
                var inputStream = File.OpenRead(ofd.FileName);
                var ms = new MemoryStream();

                rom.SE.WriteNewKernel(inputStream, ms);
                ms.Seek(0, SeekOrigin.Begin);

                inputStream.Close();

                var outputStream = File.Create(sfd.FileName);

                var key1 = rom.SC.GetDigestKey(new byte[0x10]);
                var key2 = rom.SD.GetDigestKey(key1);

                byte[] digestKey;
                CXBootloader.EncryptDecrypt(ms, outputStream, key2, out digestKey);

                ms.Close();
                outputStream.Close();
            }
        }
    }
}
