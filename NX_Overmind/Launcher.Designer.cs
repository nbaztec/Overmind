namespace NX_Overmind
{
    partial class Launcher
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.launchSelector = new System.Windows.Forms.TabControl();
            this.tabClient = new System.Windows.Forms.TabPage();
            this.checkClientScreenCapture = new System.Windows.Forms.CheckBox();
            this.checkClientMouse = new System.Windows.Forms.CheckBox();
            this.checkClientKeys = new System.Windows.Forms.CheckBox();
            this.comboClientSpeed = new System.Windows.Forms.ComboBox();
            this.comboClientType = new System.Windows.Forms.ComboBox();
            this.textClientName = new System.Windows.Forms.TextBox();
            this.textClientPort = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.buttonClientConnect = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.textClientAddr = new System.Windows.Forms.TextBox();
            this.tabServer = new System.Windows.Forms.TabPage();
            this.textServerPort = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.buttonServerStart = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.textServerAddr = new System.Windows.Forms.TextBox();
            this.labelTitle = new System.Windows.Forms.Label();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.recordViewerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.launchSelector.SuspendLayout();
            this.tabClient.SuspendLayout();
            this.tabServer.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // launchSelector
            // 
            this.launchSelector.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.launchSelector.Controls.Add(this.tabClient);
            this.launchSelector.Controls.Add(this.tabServer);
            this.launchSelector.Location = new System.Drawing.Point(50, 102);
            this.launchSelector.Name = "launchSelector";
            this.launchSelector.SelectedIndex = 0;
            this.launchSelector.Size = new System.Drawing.Size(559, 329);
            this.launchSelector.TabIndex = 0;
            // 
            // tabClient
            // 
            this.tabClient.Controls.Add(this.checkClientScreenCapture);
            this.tabClient.Controls.Add(this.checkClientMouse);
            this.tabClient.Controls.Add(this.checkClientKeys);
            this.tabClient.Controls.Add(this.comboClientSpeed);
            this.tabClient.Controls.Add(this.comboClientType);
            this.tabClient.Controls.Add(this.textClientName);
            this.tabClient.Controls.Add(this.textClientPort);
            this.tabClient.Controls.Add(this.label6);
            this.tabClient.Controls.Add(this.label5);
            this.tabClient.Controls.Add(this.label3);
            this.tabClient.Controls.Add(this.buttonClientConnect);
            this.tabClient.Controls.Add(this.label4);
            this.tabClient.Controls.Add(this.textClientAddr);
            this.tabClient.Location = new System.Drawing.Point(4, 22);
            this.tabClient.Name = "tabClient";
            this.tabClient.Padding = new System.Windows.Forms.Padding(3);
            this.tabClient.Size = new System.Drawing.Size(551, 303);
            this.tabClient.TabIndex = 0;
            this.tabClient.Text = "Client";
            this.tabClient.UseVisualStyleBackColor = true;
            // 
            // checkClientScreenCapture
            // 
            this.checkClientScreenCapture.AutoSize = true;
            this.checkClientScreenCapture.Checked = true;
            this.checkClientScreenCapture.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkClientScreenCapture.Location = new System.Drawing.Point(402, 109);
            this.checkClientScreenCapture.Name = "checkClientScreenCapture";
            this.checkClientScreenCapture.Size = new System.Drawing.Size(105, 17);
            this.checkClientScreenCapture.TabIndex = 10;
            this.checkClientScreenCapture.Text = "Screen Captures";
            this.checkClientScreenCapture.UseVisualStyleBackColor = true;
            // 
            // checkClientMouse
            // 
            this.checkClientMouse.AutoSize = true;
            this.checkClientMouse.Location = new System.Drawing.Point(402, 155);
            this.checkClientMouse.Name = "checkClientMouse";
            this.checkClientMouse.Size = new System.Drawing.Size(103, 17);
            this.checkClientMouse.TabIndex = 10;
            this.checkClientMouse.Text = "Mouse Captures";
            this.checkClientMouse.UseVisualStyleBackColor = true;
            // 
            // checkClientKeys
            // 
            this.checkClientKeys.AutoSize = true;
            this.checkClientKeys.Location = new System.Drawing.Point(402, 132);
            this.checkClientKeys.Name = "checkClientKeys";
            this.checkClientKeys.Size = new System.Drawing.Size(89, 17);
            this.checkClientKeys.TabIndex = 10;
            this.checkClientKeys.Text = "Key Captures";
            this.checkClientKeys.UseVisualStyleBackColor = true;
            // 
            // comboClientSpeed
            // 
            this.comboClientSpeed.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboClientSpeed.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.comboClientSpeed.FormattingEnabled = true;
            this.comboClientSpeed.Items.AddRange(new object[] {
            "Ultra",
            "Optimal",
            "Slow"});
            this.comboClientSpeed.Location = new System.Drawing.Point(309, 109);
            this.comboClientSpeed.Name = "comboClientSpeed";
            this.comboClientSpeed.Size = new System.Drawing.Size(78, 24);
            this.comboClientSpeed.TabIndex = 9;
            // 
            // comboClientType
            // 
            this.comboClientType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboClientType.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.comboClientType.FormattingEnabled = true;
            this.comboClientType.Items.AddRange(new object[] {
            "Live Support",
            "Streaming"});
            this.comboClientType.Location = new System.Drawing.Point(146, 109);
            this.comboClientType.Name = "comboClientType";
            this.comboClientType.Size = new System.Drawing.Size(148, 24);
            this.comboClientType.TabIndex = 9;
            // 
            // textClientName
            // 
            this.textClientName.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textClientName.Location = new System.Drawing.Point(146, 139);
            this.textClientName.Name = "textClientName";
            this.textClientName.Size = new System.Drawing.Size(148, 23);
            this.textClientName.TabIndex = 8;
            this.textClientName.Text = "HOME-PC";
            // 
            // textClientPort
            // 
            this.textClientPort.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textClientPort.Location = new System.Drawing.Point(146, 79);
            this.textClientPort.MaxLength = 5;
            this.textClientPort.Name = "textClientPort";
            this.textClientPort.Size = new System.Drawing.Size(59, 23);
            this.textClientPort.TabIndex = 8;
            this.textClientPort.Text = "28432";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(23, 144);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(67, 13);
            this.label6.TabIndex = 6;
            this.label6.Text = "Client Name:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(23, 114);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(91, 13);
            this.label5.TabIndex = 6;
            this.label5.Text = "Connection Type:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(23, 84);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(69, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Port Number:";
            // 
            // buttonClientConnect
            // 
            this.buttonClientConnect.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonClientConnect.Location = new System.Drawing.Point(146, 202);
            this.buttonClientConnect.Name = "buttonClientConnect";
            this.buttonClientConnect.Size = new System.Drawing.Size(73, 26);
            this.buttonClientConnect.TabIndex = 7;
            this.buttonClientConnect.Text = "Connect";
            this.buttonClientConnect.UseVisualStyleBackColor = true;
            this.buttonClientConnect.Click += new System.EventHandler(this.buttonClientConnect_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(23, 52);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(82, 13);
            this.label4.TabIndex = 5;
            this.label4.Text = "Server Address:";
            // 
            // textClientAddr
            // 
            this.textClientAddr.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textClientAddr.Location = new System.Drawing.Point(146, 46);
            this.textClientAddr.Name = "textClientAddr";
            this.textClientAddr.Size = new System.Drawing.Size(241, 26);
            this.textClientAddr.TabIndex = 4;
            this.textClientAddr.Text = "192.168.1.2";
            // 
            // tabServer
            // 
            this.tabServer.Controls.Add(this.textServerPort);
            this.tabServer.Controls.Add(this.label2);
            this.tabServer.Controls.Add(this.buttonServerStart);
            this.tabServer.Controls.Add(this.label1);
            this.tabServer.Controls.Add(this.textServerAddr);
            this.tabServer.Location = new System.Drawing.Point(4, 22);
            this.tabServer.Name = "tabServer";
            this.tabServer.Padding = new System.Windows.Forms.Padding(3);
            this.tabServer.Size = new System.Drawing.Size(551, 303);
            this.tabServer.TabIndex = 1;
            this.tabServer.Text = "Server";
            this.tabServer.UseVisualStyleBackColor = true;
            // 
            // textServerPort
            // 
            this.textServerPort.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textServerPort.Location = new System.Drawing.Point(118, 72);
            this.textServerPort.MaxLength = 5;
            this.textServerPort.Name = "textServerPort";
            this.textServerPort.Size = new System.Drawing.Size(59, 23);
            this.textServerPort.TabIndex = 8;
            this.textServerPort.Text = "28432";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(43, 75);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(69, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Port Number:";
            // 
            // buttonServerStart
            // 
            this.buttonServerStart.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonServerStart.Location = new System.Drawing.Point(118, 154);
            this.buttonServerStart.Name = "buttonServerStart";
            this.buttonServerStart.Size = new System.Drawing.Size(141, 26);
            this.buttonServerStart.TabIndex = 7;
            this.buttonServerStart.Text = "Start Server";
            this.buttonServerStart.UseVisualStyleBackColor = true;
            this.buttonServerStart.Click += new System.EventHandler(this.buttonServerStart_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(30, 45);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(82, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Server Address:";
            // 
            // textServerAddr
            // 
            this.textServerAddr.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textServerAddr.Location = new System.Drawing.Point(118, 39);
            this.textServerAddr.Name = "textServerAddr";
            this.textServerAddr.Size = new System.Drawing.Size(241, 26);
            this.textServerAddr.TabIndex = 4;
            this.textServerAddr.Text = "nbaztec.no-ip.org";
            // 
            // labelTitle
            // 
            this.labelTitle.BackColor = System.Drawing.Color.Lavender;
            this.labelTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.labelTitle.Font = new System.Drawing.Font("Lucida Sans", 32.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelTitle.ForeColor = System.Drawing.Color.MidnightBlue;
            this.labelTitle.Location = new System.Drawing.Point(0, 24);
            this.labelTitle.Name = "labelTitle";
            this.labelTitle.Size = new System.Drawing.Size(659, 54);
            this.labelTitle.TabIndex = 1;
            this.labelTitle.Text = "Overmind";
            this.labelTitle.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.Color.Transparent;
            this.menuStrip1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.optionsToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(659, 24);
            this.menuStrip1.TabIndex = 2;
            this.menuStrip1.Text = "menuStripLauncher";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(38, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.S)));
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(128, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.recordViewerToolStripMenuItem});
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(62, 20);
            this.optionsToolStripMenuItem.Text = "&Options";
            // 
            // recordViewerToolStripMenuItem
            // 
            this.recordViewerToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.recordViewerToolStripMenuItem.Name = "recordViewerToolStripMenuItem";
            this.recordViewerToolStripMenuItem.Size = new System.Drawing.Size(149, 22);
            this.recordViewerToolStripMenuItem.Text = "Record Viewer";
            this.recordViewerToolStripMenuItem.Click += new System.EventHandler(this.recordViewerToolStripMenuItem_Click);
            // 
            // Launcher
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(659, 456);
            this.Controls.Add(this.labelTitle);
            this.Controls.Add(this.launchSelector);
            this.Controls.Add(this.menuStrip1);
            this.GlassEnabled = true;
            this.MainMenuStrip = this.menuStrip1;
            this.MinimumSize = new System.Drawing.Size(675, 494);
            this.Name = "Launcher";
            this.SheetOfGlass = true;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Launcher";
            this.TransColor = System.Drawing.Color.FromArgb(((int)(((byte)(160)))), ((int)(((byte)(160)))), ((int)(((byte)(161)))));
            this.TransparencyKey = System.Drawing.Color.Maroon;
            this.launchSelector.ResumeLayout(false);
            this.tabClient.ResumeLayout(false);
            this.tabClient.PerformLayout();
            this.tabServer.ResumeLayout(false);
            this.tabServer.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TabControl launchSelector;
        private System.Windows.Forms.TabPage tabClient;
        private System.Windows.Forms.TabPage tabServer;
        private System.Windows.Forms.ComboBox comboClientType;
        private System.Windows.Forms.TextBox textClientName;
        private System.Windows.Forms.TextBox textClientPort;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button buttonClientConnect;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textClientAddr;
        private System.Windows.Forms.TextBox textServerPort;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button buttonServerStart;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textServerAddr;
        private System.Windows.Forms.Label labelTitle;
        private System.Windows.Forms.ComboBox comboClientSpeed;
        private System.Windows.Forms.CheckBox checkClientKeys;
        private System.Windows.Forms.CheckBox checkClientScreenCapture;
        private System.Windows.Forms.CheckBox checkClientMouse;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem recordViewerToolStripMenuItem;

    }
}