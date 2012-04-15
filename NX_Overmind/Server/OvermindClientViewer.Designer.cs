namespace NX_Overmind
{
    partial class OvermindClientTab
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OvermindClientTab));
            this.toolStripMenu = new System.Windows.Forms.ToolStrip();
            this.toolStripButtonLogEvent = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonLogConn = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonRecord = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.textEventLog = new NX.Controls.RichTextConsole();
            this.textLog = new NX.Controls.RichTextConsole();
            this.splitLogContainer = new System.Windows.Forms.SplitContainer();
            this.splitContainerMain = new System.Windows.Forms.SplitContainer();
            this.mouseEventTimer = new System.Windows.Forms.Timer(this.components);
            this.keyEventTimer = new System.Windows.Forms.Timer(this.components);
            this.toolStripButtonDisconnect = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonOptions = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonShell = new System.Windows.Forms.ToolStripButton();
            this.toolStripLabelInputIndicator = new System.Windows.Forms.ToolStripLabel();
            this.pictureScreen = new System.Windows.Forms.SelectablePictureBox(this.components);
            this.toolStripMenu.SuspendLayout();
            this.splitLogContainer.Panel1.SuspendLayout();
            this.splitLogContainer.Panel2.SuspendLayout();
            this.splitLogContainer.SuspendLayout();
            this.splitContainerMain.Panel1.SuspendLayout();
            this.splitContainerMain.Panel2.SuspendLayout();
            this.splitContainerMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureScreen)).BeginInit();
            this.SuspendLayout();
            // 
            // toolStripMenu
            // 
            this.toolStripMenu.BackColor = System.Drawing.Color.Transparent;
            this.toolStripMenu.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStripMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonLogEvent,
            this.toolStripButtonLogConn,
            this.toolStripSeparator,
            this.toolStripButtonDisconnect,
            this.toolStripButtonRecord,
            this.toolStripButtonOptions,
            this.toolStripButtonShell,
            this.toolStripSeparator1,
            this.toolStripLabelInputIndicator});
            this.toolStripMenu.Location = new System.Drawing.Point(3, 3);
            this.toolStripMenu.Name = "toolStripMenu";
            this.toolStripMenu.Size = new System.Drawing.Size(1320, 25);
            this.toolStripMenu.TabIndex = 11;
            this.toolStripMenu.Text = "toolStrip";
            // 
            // toolStripButtonLogEvent
            // 
            this.toolStripButtonLogEvent.Checked = true;
            this.toolStripButtonLogEvent.CheckOnClick = true;
            this.toolStripButtonLogEvent.CheckState = System.Windows.Forms.CheckState.Checked;
            this.toolStripButtonLogEvent.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonLogEvent.Name = "toolStripButtonLogEvent";
            this.toolStripButtonLogEvent.Size = new System.Drawing.Size(63, 22);
            this.toolStripButtonLogEvent.Text = "Event Log";
            this.toolStripButtonLogEvent.CheckedChanged += new System.EventHandler(this.toolStrip_ItemChecked);
            // 
            // toolStripButtonLogConn
            // 
            this.toolStripButtonLogConn.Checked = true;
            this.toolStripButtonLogConn.CheckOnClick = true;
            this.toolStripButtonLogConn.CheckState = System.Windows.Forms.CheckState.Checked;
            this.toolStripButtonLogConn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonLogConn.Name = "toolStripButtonLogConn";
            this.toolStripButtonLogConn.Size = new System.Drawing.Size(96, 22);
            this.toolStripButtonLogConn.Text = "Connection Log";
            this.toolStripButtonLogConn.CheckedChanged += new System.EventHandler(this.toolStrip_ItemChecked);
            // 
            // toolStripSeparator
            // 
            this.toolStripSeparator.Name = "toolStripSeparator";
            this.toolStripSeparator.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripButtonRecord
            // 
            this.toolStripButtonRecord.CheckOnClick = true;
            this.toolStripButtonRecord.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonRecord.Name = "toolStripButtonRecord";
            this.toolStripButtonRecord.Size = new System.Drawing.Size(48, 22);
            this.toolStripButtonRecord.Text = "Record";
            this.toolStripButtonRecord.CheckedChanged += new System.EventHandler(this.toolStrip_ItemChecked);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // textEventLog
            // 
            this.textEventLog.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.textEventLog.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textEventLog.CaretStyle = NX.Controls.RichTextConsole.CaretTypeStyle.None;
            this.textEventLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textEventLog.Font = new System.Drawing.Font("Courier New", 12F);
            this.textEventLog.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(200)))));
            this.textEventLog.Location = new System.Drawing.Point(0, 0);
            this.textEventLog.Name = "textEventLog";
            this.textEventLog.ReadOnly = true;
            this.textEventLog.Size = new System.Drawing.Size(388, 324);
            this.textEventLog.TabIndex = 10;
            this.textEventLog.Text = "";
            this.textEventLog.TimestampColor = System.Drawing.Color.White;
            this.textEventLog.TimestampEnabled = false;
            this.textEventLog.TimestampFormat = "[MM/dd/yyyy HH:MM:ss] ";
            // 
            // textLog
            // 
            this.textLog.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.textLog.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textLog.CaretStyle = NX.Controls.RichTextConsole.CaretTypeStyle.None;
            this.textLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textLog.Font = new System.Drawing.Font("Courier New", 12F);
            this.textLog.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(200)))));
            this.textLog.Location = new System.Drawing.Point(0, 0);
            this.textLog.Name = "textLog";
            this.textLog.ReadOnly = true;
            this.textLog.Size = new System.Drawing.Size(388, 175);
            this.textLog.TabIndex = 7;
            this.textLog.Text = "";
            this.textLog.TimestampColor = System.Drawing.Color.White;
            this.textLog.TimestampEnabled = false;
            this.textLog.TimestampFormat = "[MM/dd/yyyy HH:MM:ss] ";
            // 
            // splitLogContainer
            // 
            this.splitLogContainer.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.splitLogContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitLogContainer.Location = new System.Drawing.Point(0, 0);
            this.splitLogContainer.Name = "splitLogContainer";
            this.splitLogContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitLogContainer.Panel1
            // 
            this.splitLogContainer.Panel1.Controls.Add(this.textEventLog);
            // 
            // splitLogContainer.Panel2
            // 
            this.splitLogContainer.Panel2.Controls.Add(this.textLog);
            this.splitLogContainer.Size = new System.Drawing.Size(390, 507);
            this.splitLogContainer.SplitterDistance = 326;
            this.splitLogContainer.TabIndex = 12;
            // 
            // splitContainerMain
            // 
            this.splitContainerMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainerMain.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.splitContainerMain.Location = new System.Drawing.Point(3, 28);
            this.splitContainerMain.Name = "splitContainerMain";
            // 
            // splitContainerMain.Panel1
            // 
            this.splitContainerMain.Panel1.Controls.Add(this.pictureScreen);
            // 
            // splitContainerMain.Panel2
            // 
            this.splitContainerMain.Panel2.Controls.Add(this.splitLogContainer);
            this.splitContainerMain.Size = new System.Drawing.Size(1320, 507);
            this.splitContainerMain.SplitterDistance = 926;
            this.splitContainerMain.TabIndex = 13;
            // 
            // mouseEventTimer
            // 
            this.mouseEventTimer.Interval = 200;
            this.mouseEventTimer.Tick += new System.EventHandler(this.mouseEventTimer_Tick);
            // 
            // keyEventTimer
            // 
            this.keyEventTimer.Interval = 50;
            this.keyEventTimer.Tick += new System.EventHandler(this.keyEventTimer_Tick);
            // 
            // toolStripButtonDisconnect
            // 
            this.toolStripButtonDisconnect.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonDisconnect.Image = global::NX_Overmind.Properties.Resources.cross;
            this.toolStripButtonDisconnect.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonDisconnect.Name = "toolStripButtonDisconnect";
            this.toolStripButtonDisconnect.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonDisconnect.Text = "Disconnect";
            this.toolStripButtonDisconnect.Click += new System.EventHandler(this.toolStrip_ItemChecked);
            // 
            // toolStripButtonOptions
            // 
            this.toolStripButtonOptions.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButtonOptions.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonOptions.Image")));
            this.toolStripButtonOptions.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonOptions.Name = "toolStripButtonOptions";
            this.toolStripButtonOptions.Size = new System.Drawing.Size(53, 22);
            this.toolStripButtonOptions.Text = "Options";
            this.toolStripButtonOptions.Click += new System.EventHandler(this.toolStrip_ItemChecked);
            // 
            // toolStripButtonShell
            // 
            this.toolStripButtonShell.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonShell.Image = global::NX_Overmind.Properties.Resources.terminal;
            this.toolStripButtonShell.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonShell.Name = "toolStripButtonShell";
            this.toolStripButtonShell.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonShell.Text = "toolStripButtonShell";
            this.toolStripButtonShell.ToolTipText = "Open Shell";
            this.toolStripButtonShell.Click += new System.EventHandler(this.toolStrip_ItemChecked);
            // 
            // toolStripLabelInputIndicator
            // 
            this.toolStripLabelInputIndicator.AutoToolTip = true;
            this.toolStripLabelInputIndicator.Image = global::NX_Overmind.Properties.Resources.keyboard_delete;
            this.toolStripLabelInputIndicator.Name = "toolStripLabelInputIndicator";
            this.toolStripLabelInputIndicator.Size = new System.Drawing.Size(16, 22);
            this.toolStripLabelInputIndicator.ToolTipText = "Input Disabled";
            // 
            // pictureScreen
            // 
            this.pictureScreen.Cursor = System.Windows.Forms.Cursors.Default;
            this.pictureScreen.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureScreen.Location = new System.Drawing.Point(0, 0);
            this.pictureScreen.Name = "pictureScreen";
            this.pictureScreen.Padding = new System.Windows.Forms.Padding(5);
            this.pictureScreen.Size = new System.Drawing.Size(924, 505);
            this.pictureScreen.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureScreen.TabIndex = 8;
            this.pictureScreen.TabStop = false;
            // 
            // OvermindClientTab
            // 
            this.Controls.Add(this.toolStripMenu);
            this.Controls.Add(this.splitContainerMain);
            this.Location = new System.Drawing.Point(4, 22);
            this.Name = "OvermindClientViewer";
            this.Padding = new System.Windows.Forms.Padding(3);
            this.Size = new System.Drawing.Size(1326, 541);
            this.Text = "Client #1";
            this.UseVisualStyleBackColor = true;
            this.toolStripMenu.ResumeLayout(false);
            this.toolStripMenu.PerformLayout();
            this.splitLogContainer.Panel1.ResumeLayout(false);
            this.splitLogContainer.Panel2.ResumeLayout(false);
            this.splitLogContainer.ResumeLayout(false);
            this.splitContainerMain.Panel1.ResumeLayout(false);
            this.splitContainerMain.Panel2.ResumeLayout(false);
            this.splitContainerMain.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureScreen)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        
        private NX.Controls.RichTextConsole textLog;
        private NX.Controls.RichTextConsole textEventLog;
        private System.Windows.Forms.ToolStrip toolStripMenu;
        private System.Windows.Forms.ToolStripButton toolStripButtonLogEvent;
        private System.Windows.Forms.ToolStripButton toolStripButtonLogConn;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator;
        private System.Windows.Forms.ToolStripButton toolStripButtonRecord;
        private System.Windows.Forms.SelectablePictureBox pictureScreen;
        private System.Windows.Forms.SplitContainer splitContainerMain;
        private System.Windows.Forms.SplitContainer splitLogContainer;
        private System.Windows.Forms.ToolStripButton toolStripButtonOptions;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripLabel toolStripLabelInputIndicator;
        private System.Windows.Forms.ToolStripButton toolStripButtonShell;
        private System.Windows.Forms.Timer mouseEventTimer;
        private System.Windows.Forms.Timer keyEventTimer;
        private System.Windows.Forms.ToolStripButton toolStripButtonDisconnect;

        public NX.Controls.RichTextConsole NetworkLog { get { return this.textLog; } }
        public NX.Controls.RichTextConsole CaptureLog { get { return this.textEventLog; } }
        public System.Windows.Forms.SelectablePictureBox ScreenImage { get { return this.pictureScreen; } }
    }
}
