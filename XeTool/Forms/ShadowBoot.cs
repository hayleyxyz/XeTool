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
                extractKernelButton.Enabled = true;
        }

        private void extractAllButton_Click(object sender, EventArgs e) {
            var fbd = new FolderBrowserDialog();

            fbd.SelectedPath = Directory.GetParent(currentFile).FullName;

            if (fbd.ShowDialog() == DialogResult.OK) {
                var dir = fbd.SelectedPath;

                var fs = File.Create(String.Format("{0}\\{1}", fbd.SelectedPath, rom.SB.GetFileName()));
                rom.SB.Decrypt(fs, StaticKeys.BL1_KEY);
                fs.Close();

                fs = File.Create(String.Format("{0}\\{1}", fbd.SelectedPath, rom.SC.GetFileName()));
                byte[] bl3Key;
                rom.SC.Decrypt(fs, new byte[0x10], out bl3Key);
                fs.Close();

                fs = File.Create(String.Format("{0}\\{1}", fbd.SelectedPath, rom.SD.GetFileName()));
                byte[] bl4Key;
                rom.SD.Decrypt(fs, bl3Key, out bl4Key);
                fs.Close();

                fs = File.Create(String.Format("{0}\\{1}", fbd.SelectedPath, rom.SE.GetFileName()));
                rom.SE.Decrypt(fs, bl4Key);
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

        private void extractKernelButton_Click(object sender, EventArgs e) {
            var fbd = new FolderBrowserDialog() {
                SelectedPath = Directory.GetParent(currentFile).FullName
            };

            if (fbd.ShowDialog() == DialogResult.OK) {
                var dir = fbd.SelectedPath;
                   
                var key1 = rom.SC.GetDigestKey(new byte[0x10]);
                var key2 = rom.SD.GetDigestKey(key1);

                var stream = File.Create(String.Format("{0}\\{1}", dir, rom.SE.GetKernelFileName()));

                rom.SE.ExtractKernel(stream, key2);

                stream.Close();
            }
        }
    }
}
