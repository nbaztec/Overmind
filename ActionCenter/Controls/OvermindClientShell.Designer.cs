namespace NX_Overmind.Actions
{
    partial class OvermindClientShell
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
            this.richShell = new NX.Controls.RichTextConsole();
            this.SuspendLayout();
            // 
            // richShell
            // 
            this.richShell.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.richShell.CaretStyle = NX.Controls.RichTextConsole.CaretTypeStyle.Block;
            this.richShell.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richShell.Font = new System.Drawing.Font("Courier New", 12F);
            this.richShell.ForeColor = System.Drawing.Color.GhostWhite;
            this.richShell.Location = new System.Drawing.Point(0, 0);
            this.richShell.Name = "richShell";
            this.richShell.Size = new System.Drawing.Size(605, 325);
            this.richShell.TabIndex = 0;
            this.richShell.Text = "";
            this.richShell.TimestampColor = System.Drawing.Color.White;
            this.richShell.TimestampEnabled = false;
            this.richShell.TimestampFormat = "[MM/dd/yyyy HH:MM:ss] ";
            this.richShell.KeyDown += new System.Windows.Forms.KeyEventHandler(this.richShell_KeyDown);
            this.richShell.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.richShell_KeyPress);
            // 
            // OvermindClientShell
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(605, 325);
            this.Controls.Add(this.richShell);
            this.MinimizeBox = false;
            this.Name = "OvermindClientShell";
            this.Text = "OvermindClientShell";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OvermindClientShell_FormClosing);
            this.ResumeLayout(false);

        }

        #endregion

        private NX.Controls.RichTextConsole richShell;
    }
}