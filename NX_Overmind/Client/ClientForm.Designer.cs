namespace NX_Overmind
{
    partial class ClientForm
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
            this.captureTimer = new System.Windows.Forms.Timer(this.components);
            this.connTimer = new System.Windows.Forms.Timer(this.components);
            this.textLog = new NX.Controls.RichTextConsole();
            this.menuStripClientMain = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveLogToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.capturesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.screenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.keyEventsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mouseEventsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.stopToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStripClientMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // captureTimer
            // 
            this.captureTimer.Interval = 500;
            this.captureTimer.Tick += new System.EventHandler(this.timerClock_Tick);
            // 
            // connTimer
            // 
            this.connTimer.Interval = 1000;
            this.connTimer.Tick += new System.EventHandler(this.connTimer_Tick);
            // 
            // textLog
            // 
            this.textLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textLog.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.textLog.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textLog.CaretStyle = NX.Controls.RichTextConsole.CaretTypeStyle.None;
            this.textLog.Font = new System.Drawing.Font("Courier New", 12F);
            this.textLog.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(200)))));
            this.textLog.Location = new System.Drawing.Point(12, 27);
            this.textLog.Name = "textLog";
            this.textLog.ReadOnly = true;
            this.textLog.Size = new System.Drawing.Size(1164, 472);
            this.textLog.TabIndex = 6;
            this.textLog.Text = "";
            this.textLog.TimestampColor = System.Drawing.Color.White;
            this.textLog.TimestampEnabled = true;
            this.textLog.TimestampFormat = "[MM/dd/yyyy HH:MM:ss] ";
            // 
            // menuStripClientMain
            // 
            this.menuStripClientMain.BackColor = System.Drawing.Color.Transparent;
            this.menuStripClientMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.optionsToolStripMenuItem});
            this.menuStripClientMain.Location = new System.Drawing.Point(0, 0);
            this.menuStripClientMain.Name = "menuStripClientMain";
            this.menuStripClientMain.Size = new System.Drawing.Size(1192, 24);
            this.menuStripClientMain.TabIndex = 8;
            this.menuStripClientMain.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveLogToolStripMenuItem,
            this.stopToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // saveLogToolStripMenuItem
            // 
            this.saveLogToolStripMenuItem.Name = "saveLogToolStripMenuItem";
            this.saveLogToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.saveLogToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.saveLogToolStripMenuItem.Text = "Save Log";
            this.saveLogToolStripMenuItem.Click += new System.EventHandler(this.saveLogToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.X)));
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.capturesToolStripMenuItem});
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.optionsToolStripMenuItem.Text = "Options";
            // 
            // capturesToolStripMenuItem
            // 
            this.capturesToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.screenToolStripMenuItem,
            this.keyEventsToolStripMenuItem,
            this.mouseEventsToolStripMenuItem});
            this.capturesToolStripMenuItem.Name = "capturesToolStripMenuItem";
            this.capturesToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.capturesToolStripMenuItem.Text = "Captures";
            // 
            // screenToolStripMenuItem
            // 
            this.screenToolStripMenuItem.CheckOnClick = true;
            this.screenToolStripMenuItem.Name = "screenToolStripMenuItem";
            this.screenToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.screenToolStripMenuItem.Text = "Screen";
            this.screenToolStripMenuItem.CheckedChanged += new System.EventHandler(this.checkBox_CheckCaptureEventChanged);
            // 
            // keyEventsToolStripMenuItem
            // 
            this.keyEventsToolStripMenuItem.CheckOnClick = true;
            this.keyEventsToolStripMenuItem.Name = "keyEventsToolStripMenuItem";
            this.keyEventsToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.keyEventsToolStripMenuItem.Text = "Key Events";
            this.keyEventsToolStripMenuItem.CheckedChanged += new System.EventHandler(this.checkBox_CheckCaptureEventChanged);
            // 
            // mouseEventsToolStripMenuItem
            // 
            this.mouseEventsToolStripMenuItem.CheckOnClick = true;
            this.mouseEventsToolStripMenuItem.Name = "mouseEventsToolStripMenuItem";
            this.mouseEventsToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.mouseEventsToolStripMenuItem.Text = "Mouse Events";
            this.mouseEventsToolStripMenuItem.CheckedChanged += new System.EventHandler(this.checkBox_CheckCaptureEventChanged);
            // 
            // saveFileDialog
            // 
            this.saveFileDialog.DefaultExt = "txt";
            // 
            // stopToolStripMenuItem
            // 
            this.stopToolStripMenuItem.Name = "stopToolStripMenuItem";
            this.stopToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.T)));
            this.stopToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.stopToolStripMenuItem.Text = "Stop";
            this.stopToolStripMenuItem.Click += new System.EventHandler(this.stopToolStripMenuItem_Click);
            // 
            // ClientForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.ClientSize = new System.Drawing.Size(1192, 513);
            this.Controls.Add(this.textLog);
            this.Controls.Add(this.menuStripClientMain);
            this.MainMenuStrip = this.menuStripClientMain;
            this.Name = "ClientForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Overmind Client";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ClientForm_FormClosing);
            this.menuStripClientMain.ResumeLayout(false);
            this.menuStripClientMain.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private NX.Controls.RichTextConsole textLog;
        private System.Windows.Forms.Timer captureTimer;
        private System.Windows.Forms.Timer connTimer;
        private System.Windows.Forms.MenuStrip menuStripClientMain;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveLogToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem capturesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem screenToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem keyEventsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mouseEventsToolStripMenuItem;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private System.Windows.Forms.ToolStripMenuItem stopToolStripMenuItem;

    }
}

