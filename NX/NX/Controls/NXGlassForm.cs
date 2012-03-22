using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace NX.GlassForm
{
    public partial class GlassForm : Form
    {
        public GlassForm()
        {
            InitializeComponent();
        }

        public GlassForm(IContainer container)
        {
            container.Add(this);
            InitializeComponent();
        }

        [DllImport("dwmapi.dll")]
        public static extern IntPtr DwmExtendFrameIntoClientArea(IntPtr hWnd, ref MARGINS pMarInset);
        [DllImport("dwmapi.dll", SetLastError = true)]
        private static extern IntPtr DwmIsCompositionEnabled([MarshalAs(UnmanagedType.Bool)]out bool pfEnabled);
        [StructLayout(LayoutKind.Sequential)]
        public struct MARGINS
        {
            public int cxLeftWidth;
            public int cxRightWidth;
            public int cyTopHeight;
            public int cyBottomHeight;

            public MARGINS(int leftWidth, int rightWidth, int topHeight, int bottomHeight)
            {
                this.cxLeftWidth = leftWidth;
                this.cxRightWidth = rightWidth;
                this.cyTopHeight = topHeight;
                this.cyBottomHeight = bottomHeight;
            }
            public MARGINS(int all)
                : this(all, all, all, all)
            {
            }

        }
        private const int WM_DWMCOMPOSITIONCHANGED = 0x31E;
        private const int WM_DWMNCRENDERINGCHANGED = 0x31F;

        private bool isGlassEnabled
        {
            get
            {
                if (Environment.OSVersion.Version.Major >= 6 && this.GlassEnabled)
                {
                    bool glassEnabled = false;
                    DwmIsCompositionEnabled(out glassEnabled);
                    return glassEnabled;
                }
                return false;
            }
        }

        private void OnDWMCompositionChanged(EventArgs eventArgs)
        {
            if (isGlassEnabled)
            {
                //MARGINS margin = new MARGINS(-1);
                //DwmExtendFrameIntoClientArea(this.Handle, ref margin);                
                if (this.SheetOfGlass == true)
                {
                    this.BackColor = TransColor;
                    this.TransparencyKey = TransColor;
                    MARGINS margins = new MARGINS(-1);
                    DwmExtendFrameIntoClientArea(this.Handle, ref margins);
                }
                else
                {
                    this.BackColor = TransColor;
                    this.TransparencyKey = TransColor;
                    DwmExtendFrameIntoClientArea(this.Handle, ref this.GlassMargins);
                }
            }
            else
            {
                //this.BackColor = Color.White;
                this.BackColor = this.GlassBackColor;
                this.TransparencyKey = Color.Empty;
                MARGINS margins = new MARGINS(0);
                DwmExtendFrameIntoClientArea(this.Handle, ref margins);
            }
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            if (isGlassEnabled)
                e.Graphics.Clear(this.TransparencyKey);
            else
                base.OnPaintBackground(e);
        }


        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case WM_DWMCOMPOSITIONCHANGED:
                case WM_DWMNCRENDERINGCHANGED:
                    OnDWMCompositionChanged(EventArgs.Empty);
                    break;
            }
            base.WndProc(ref m);
        }

        public Color GlassBackColor { get; set; }
        private Color _transcolor = Color.FromArgb(160, 160, 161);
        public Color TransColor
        {
            get { return this._transcolor; }
            set { this._transcolor = value; }
        }
        private bool _glassenabled;
        public bool GlassEnabled
        {
            get { return this._glassenabled; }
            set { this._glassenabled = value; GlassRefresh(); }
        }
        public MARGINS GlassMargins = new MARGINS();
        private bool _sheetofglass;
        public bool SheetOfGlass
        {
            get { return this._sheetofglass; }
            set { this._sheetofglass = value; GlassRefresh(); }
        }

        public void GlassRefresh()
        {
            OnDWMCompositionChanged(null);
        }
    }
}
