namespace NX_Overmind
{
    partial class ServerForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ServerForm));
            this.connTimer = new System.Windows.Forms.Timer(this.components);
            this.captureTimer = new System.Windows.Forms.Timer(this.components);
            this.snapShow = new System.Windows.Forms.Timer(this.components);
            this.tabControl = new System.Windows.Forms.TabControl();
            this.textLog = new NX.Controls.RichTextConsole();
            this.textDebug = new NX.Controls.RichTextConsole();
            this.SuspendLayout();
            // 
            // snapShow
            // 
            this.snapShow.Interval = 250;
            this.snapShow.Tick += new System.EventHandler(this.snapShow_Tick);
            // 
            // tabControl
            // 
            this.tabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl.Location = new System.Drawing.Point(16, 45);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(1330, 438);
            this.tabControl.TabIndex = 14;
            // 
            // textLog
            // 
            this.textLog.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.textLog.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.textLog.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textLog.CaretStyle = NX.Controls.RichTextConsole.CaretTypeStyle.None;
            this.textLog.Font = new System.Drawing.Font("Courier New", 12F);
            this.textLog.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(200)))));
            this.textLog.Location = new System.Drawing.Point(16, 489);
            this.textLog.Name = "textLog";
            this.textLog.ReadOnly = true;
            this.textLog.Size = new System.Drawing.Size(1330, 137);
            this.textLog.TabIndex = 7;
            this.textLog.Text = "";
            this.textLog.TimestampColor = System.Drawing.Color.White;
            this.textLog.TimestampEnabled = true;
            this.textLog.TimestampFormat = "[MM/dd/yyyy HH:MM:ss] ";
            // 
            // textDebug
            // 
            this.textDebug.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textDebug.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.textDebug.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textDebug.CaretStyle = NX.Controls.RichTextConsole.CaretTypeStyle.None;
            this.textDebug.Font = new System.Drawing.Font("Courier New", 12F);
            this.textDebug.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(200)))));
            this.textDebug.Location = new System.Drawing.Point(1057, 489);
            this.textDebug.Name = "textDebug";
            this.textDebug.ReadOnly = true;
            this.textDebug.Size = new System.Drawing.Size(289, 137);
            this.textDebug.TabIndex = 12;
            this.textDebug.Text = "";
            this.textDebug.TimestampColor = System.Drawing.Color.White;
            this.textDebug.TimestampEnabled = false;
            this.textDebug.TimestampFormat = "[MM/dd/yyyy HH:MM:ss] ";
            this.textDebug.Visible = false;
            // 
            // ServerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1358, 638);
            this.Controls.Add(this.textDebug);
            this.Controls.Add(this.textLog);
            this.Controls.Add(this.tabControl);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ServerForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Overmind Server";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ServerForm_FormClosing);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Timer connTimer;
        private System.Windows.Forms.Timer captureTimer;
        private NX.Controls.RichTextConsole textDebug;
        private System.Windows.Forms.Timer snapShow;
        private System.Windows.Forms.TabControl tabControl;
        private NX.Controls.RichTextConsole textLog;
    }
}

