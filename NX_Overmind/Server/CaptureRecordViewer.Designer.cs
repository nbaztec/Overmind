namespace NX_Overmind
{
    partial class CaptureRecordViewer
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CaptureRecordViewer));
            this.trackBarTimeline = new System.Windows.Forms.TrackBar();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.resetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.eraseEventLogToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.autoplayToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.startStopToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripComboBoxAutoplaySpeed = new System.Windows.Forms.ToolStripComboBox();
            this.pictureScreen = new System.Windows.Forms.PictureBox();
            this.textLog = new NX.Controls.RichTextConsole();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.timerPlay = new System.Windows.Forms.Timer(this.components);
            this.progressBarLoading = new System.Windows.Forms.ProgressBar();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarTimeline)).BeginInit();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureScreen)).BeginInit();
            this.SuspendLayout();
            // 
            // trackBarTimeline
            // 
            this.trackBarTimeline.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.trackBarTimeline.Location = new System.Drawing.Point(13, 27);
            this.trackBarTimeline.Name = "trackBarTimeline";
            this.trackBarTimeline.Size = new System.Drawing.Size(913, 45);
            this.trackBarTimeline.TabIndex = 0;
            this.trackBarTimeline.TickStyle = System.Windows.Forms.TickStyle.Both;
            this.trackBarTimeline.ValueChanged += new System.EventHandler(this.trackBarTimeline_ValueChanged);
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.Color.Transparent;
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.optionsToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(938, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.openToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
            this.openToolStripMenuItem.Text = "Open...";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.X)));
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.resetToolStripMenuItem,
            this.eraseEventLogToolStripMenuItem,
            this.autoplayToolStripMenuItem});
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.optionsToolStripMenuItem.Text = "&Options";
            // 
            // resetToolStripMenuItem
            // 
            this.resetToolStripMenuItem.Name = "resetToolStripMenuItem";
            this.resetToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.R)));
            this.resetToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.resetToolStripMenuItem.Text = "Reset";
            this.resetToolStripMenuItem.Click += new System.EventHandler(this.resetToolStripMenuItem_Click);
            // 
            // eraseEventLogToolStripMenuItem
            // 
            this.eraseEventLogToolStripMenuItem.Checked = true;
            this.eraseEventLogToolStripMenuItem.CheckOnClick = true;
            this.eraseEventLogToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.eraseEventLogToolStripMenuItem.Name = "eraseEventLogToolStripMenuItem";
            this.eraseEventLogToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.eraseEventLogToolStripMenuItem.Text = "Erase Event Log";
            // 
            // autoplayToolStripMenuItem
            // 
            this.autoplayToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.startStopToolStripMenuItem,
            this.toolStripComboBoxAutoplaySpeed});
            this.autoplayToolStripMenuItem.Name = "autoplayToolStripMenuItem";
            this.autoplayToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.autoplayToolStripMenuItem.Text = "Autoplay";
            // 
            // startStopToolStripMenuItem
            // 
            this.startStopToolStripMenuItem.CheckOnClick = true;
            this.startStopToolStripMenuItem.Name = "startStopToolStripMenuItem";
            this.startStopToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.P)));
            this.startStopToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
            this.startStopToolStripMenuItem.Text = "Start/Stop";
            this.startStopToolStripMenuItem.CheckedChanged += new System.EventHandler(this.startStopToolStripMenuItem_CheckedChanged);
            // 
            // toolStripComboBoxAutoplaySpeed
            // 
            this.toolStripComboBoxAutoplaySpeed.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.toolStripComboBoxAutoplaySpeed.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.toolStripComboBoxAutoplaySpeed.Items.AddRange(new object[] {
            "Ultra Slow",
            "Slow",
            "Medium",
            "Fast",
            "Ultra Fast"});
            this.toolStripComboBoxAutoplaySpeed.Name = "toolStripComboBoxAutoplaySpeed";
            this.toolStripComboBoxAutoplaySpeed.Size = new System.Drawing.Size(121, 23);
            this.toolStripComboBoxAutoplaySpeed.SelectedIndexChanged += new System.EventHandler(this.toolStripComboBoxAutoplaySpeed_SelectedIndexChanged);
            // 
            // pictureScreen
            // 
            this.pictureScreen.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureScreen.Location = new System.Drawing.Point(13, 78);
            this.pictureScreen.Name = "pictureScreen";
            this.pictureScreen.Size = new System.Drawing.Size(913, 261);
            this.pictureScreen.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureScreen.TabIndex = 3;
            this.pictureScreen.TabStop = false;
            // 
            // textLog
            // 
            this.textLog.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textLog.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(62)))), ((int)(((byte)(62)))));
            this.textLog.CaretStyle = NX.Controls.RichTextConsole.CaretTypeStyle.None;
            this.textLog.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textLog.ForeColor = System.Drawing.Color.MintCream;
            this.textLog.Location = new System.Drawing.Point(13, 345);
            this.textLog.Name = "textLog";
            this.textLog.ReadOnly = true;
            this.textLog.Size = new System.Drawing.Size(913, 173);
            this.textLog.TabIndex = 2;
            this.textLog.Text = "";
            this.textLog.TimestampColor = System.Drawing.Color.White;
            this.textLog.TimestampEnabled = false;
            this.textLog.TimestampFormat = "[MM/dd/yyyy HH:MM:ss] ";
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // timerPlay
            // 
            this.timerPlay.Interval = 1000;
            this.timerPlay.Tick += new System.EventHandler(this.timerPlay_Tick);
            // 
            // progressBarLoading
            // 
            this.progressBarLoading.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBarLoading.Location = new System.Drawing.Point(13, 315);
            this.progressBarLoading.Name = "progressBarLoading";
            this.progressBarLoading.Size = new System.Drawing.Size(913, 23);
            this.progressBarLoading.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBarLoading.TabIndex = 4;
            this.progressBarLoading.Visible = false;
            // 
            // CaptureRecordViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.GhostWhite;
            this.ClientSize = new System.Drawing.Size(938, 530);
            this.Controls.Add(this.progressBarLoading);
            this.Controls.Add(this.pictureScreen);
            this.Controls.Add(this.textLog);
            this.Controls.Add(this.trackBarTimeline);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "CaptureRecordViewer";
            this.Text = "Record Viewer";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.CaptureRecordViewer_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.trackBarTimeline)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureScreen)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TrackBar trackBarTimeline;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.PictureBox pictureScreen;
        private NX.Controls.RichTextConsole textLog;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.ToolStripMenuItem resetToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem eraseEventLogToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem autoplayToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem startStopToolStripMenuItem;
        private System.Windows.Forms.ToolStripComboBox toolStripComboBoxAutoplaySpeed;
        private System.Windows.Forms.Timer timerPlay;
        private System.Windows.Forms.ProgressBar progressBarLoading;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
    }
}