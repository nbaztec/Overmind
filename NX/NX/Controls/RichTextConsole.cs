using System;
using System.Text;
using System.Drawing;
using System.Runtime.InteropServices;

namespace NX.Controls
{
    /// <summary>
    /// Represents a console like interface via RichTextEdit control
    /// </summary>
    public class RichTextConsole : System.Windows.Forms.RichTextBox
    {    
        public enum CaretTypeStyle
        {
            None,
            Block
        }

        public CaretTypeStyle CaretStyle { get; set; }

        public bool TimestampEnabled { get; set; }
        public Color TimestampColor { get; set; }
        public string TimestampFormat { get; set; }
        

        public RichTextConsole()
            : base()
        {
            //this.SetStyle(System.Windows.Forms.ControlStyles.SupportsTransparentBackColor, true);
            this.BackColor = System.Drawing.Color.FromArgb(50, 50, 50);
            this.ForeColor = System.Drawing.Color.FromArgb(255, 255, 200);
            this.Font = new System.Drawing.Font("Courier New", 12);
            this.TimestampColor = Color.White;
            this.TimestampFormat = "[MM/dd/yyyy HH:MM:ss] ";
            this.TimestampEnabled = false;
            //this.ReadOnly = false;
            //this.ChangeCaret();
        }

        public void TerminalAppendLine(string text, Color color)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke((System.Windows.Forms.MethodInvoker)delegate() { this.TerminalAppendLine(text, color); });
            }
            else
            {
                this.SelectionStart = this.TextLength;
                if (this.TimestampEnabled)
                {
                    this.SelectionColor = this.TimestampColor;
                    this.SelectedText = DateTime.Now.ToString(this.TimestampFormat);
                }
                this.SelectionColor = color;
                this.SelectedText = (text + Environment.NewLine);
            }            
        }     

        protected override void OnEnter(EventArgs e)
        {
            base.OnEnter(e);
            this.ChangeCaret();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            this.ChangeCaret();
        }

        protected override void OnSelectionChanged(EventArgs e)
        {
            base.OnSelectionChanged(e);
            this.ChangeCaret();
        }

        protected override void OnMouseDown(System.Windows.Forms.MouseEventArgs e)
        {
            base.OnMouseDown(e);
            this.ChangeCaret();
        }

        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);            
            this.BeginInvoke(new System.Windows.Forms.MethodInvoker(ChangeCaret));
        }

        private void ChangeCaret()
        {
            if (this.CaretStyle == CaretTypeStyle.None)
                HideCaret(this.Handle);
            else
            {
                CreateCaret(this.Handle, IntPtr.Zero, 10, Font.Height);
                ShowCaret(this.Handle);
            }
        }

        [DllImport("user32.dll")]
        static extern bool CreateCaret(IntPtr hWnd, IntPtr hBitmap, int nWidth, int nHeight);

        [DllImport("user32.dll")]
        static extern bool ShowCaret(IntPtr hWnd);

        [DllImport("user32.dll")]
        static extern bool HideCaret(IntPtr hWnd);

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // RichTextConsole
            // 
            this.AcceptsTab = true;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(200)))));
            this.ShortcutsEnabled = false;
            this.ResumeLayout(false);

        }
    }
}
