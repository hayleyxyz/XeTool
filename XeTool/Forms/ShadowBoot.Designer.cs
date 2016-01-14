namespace XeTool.Forms {
    partial class ShadowBoot {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            this.mainMenu1 = new System.Windows.Forms.MainMenu(this.components);
            this.menuItem1 = new System.Windows.Forms.MenuItem();
            this.openMenuItem = new System.Windows.Forms.MenuItem();
            this.label1 = new System.Windows.Forms.Label();
            this.bl2Version = new System.Windows.Forms.TextBox();
            this.bl3Version = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.bl4Version = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.bl5Version = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.extractAllButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // mainMenu1
            // 
            this.mainMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem1});
            // 
            // menuItem1
            // 
            this.menuItem1.Index = 0;
            this.menuItem1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.openMenuItem});
            this.menuItem1.Text = "File";
            // 
            // openMenuItem
            // 
            this.openMenuItem.Index = 0;
            this.openMenuItem.Text = "Open...";
            this.openMenuItem.Click += new System.EventHandler(this.openMenuItem_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(49, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "2BL [SB]";
            // 
            // bl2Version
            // 
            this.bl2Version.Location = new System.Drawing.Point(12, 25);
            this.bl2Version.Name = "bl2Version";
            this.bl2Version.ReadOnly = true;
            this.bl2Version.Size = new System.Drawing.Size(100, 20);
            this.bl2Version.TabIndex = 1;
            // 
            // bl3Version
            // 
            this.bl3Version.Location = new System.Drawing.Point(12, 74);
            this.bl3Version.Name = "bl3Version";
            this.bl3Version.ReadOnly = true;
            this.bl3Version.Size = new System.Drawing.Size(100, 20);
            this.bl3Version.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 58);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(49, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "3BL [SC]";
            // 
            // bl4Version
            // 
            this.bl4Version.Location = new System.Drawing.Point(15, 121);
            this.bl4Version.Name = "bl4Version";
            this.bl4Version.ReadOnly = true;
            this.bl4Version.Size = new System.Drawing.Size(100, 20);
            this.bl4Version.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(15, 105);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(50, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "4BL [SD]";
            // 
            // bl5Version
            // 
            this.bl5Version.Location = new System.Drawing.Point(15, 170);
            this.bl5Version.Name = "bl5Version";
            this.bl5Version.ReadOnly = true;
            this.bl5Version.Size = new System.Drawing.Size(100, 20);
            this.bl5Version.TabIndex = 7;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(15, 154);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(109, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "5BL [SE] (HV+Kernel)";
            // 
            // extractAllButton
            // 
            this.extractAllButton.Enabled = false;
            this.extractAllButton.Location = new System.Drawing.Point(15, 227);
            this.extractAllButton.Name = "extractAllButton";
            this.extractAllButton.Size = new System.Drawing.Size(75, 23);
            this.extractAllButton.TabIndex = 8;
            this.extractAllButton.Text = "Extract all";
            this.extractAllButton.UseVisualStyleBackColor = true;
            this.extractAllButton.Click += new System.EventHandler(this.extractAllButton_Click);
            // 
            // ShadowBoot
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.extractAllButton);
            this.Controls.Add(this.bl5Version);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.bl4Version);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.bl3Version);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.bl2Version);
            this.Controls.Add(this.label1);
            this.Menu = this.mainMenu1;
            this.Name = "ShadowBoot";
            this.Text = "ShadowBoot";
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.ShadowBoot_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.ShadowBoot_DragEnter);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MainMenu mainMenu1;
        private System.Windows.Forms.MenuItem menuItem1;
        private System.Windows.Forms.MenuItem openMenuItem;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox bl2Version;
        private System.Windows.Forms.TextBox bl3Version;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox bl4Version;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox bl5Version;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button extractAllButton;
    }
}