using System;

namespace NX.Controls
{
    /// <summary>
    /// A Glassform supporting the Windows Aero theme. Backward compatible with Windows XP
    /// </summary>
    public partial class GlassForm : System.Windows.Forms.Form
    {
        private bool _isSupported = false;
        /// <summary>
        /// Checks if the GlassForm is supported in this version of Windows
        /// </summary>
        public bool IsGlassSupported
        {
            get
            {
                return this._isSupported;
            }
        }

        /// <summary>
        /// Gets or sets the background color for the form
        /// </summary>
        public System.Drawing.Color GlassBackColor { get; set; }
        
        private System.Drawing.Color _transcolor = System.Drawing.Color.FromArgb(160, 161, 161);
        /// <summary>
        /// Gets or sets the transparent color for the form
        /// </summary>
        public System.Drawing.Color TransColor
        {
            get { return this._transcolor; }
            set { this._transcolor = value; }
        }


        private bool _glassEnabled;
        /// <summary>
        /// Checks whether the glass is enabled
        /// </summary>
        public bool GlassEnabled
        {
            get { return this._glassEnabled; }
            set { this._glassEnabled = value; GlassRefresh(); }
        }

        private MARGINS _glassMargins = new MARGINS();
        /// <summary>
        /// Margins from the corner of glass form
        /// </summary>
        public MARGINS GlassMargins
        {
            get
            {
                return this._glassMargins;
            }
            set
            {
                this._glassMargins = value;
            }
        }

        private bool _sheetOfGlass;
        /// <summary>
        /// Makes the entire form of glass
        /// </summary>
        public bool SheetOfGlass
        {
            get { return this._sheetOfGlass; }
            set { this._sheetOfGlass = value; GlassRefresh(); }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public GlassForm()
        {
            InitializeComponent();            
            this._isSupported = Environment.OSVersion.Version.Major >= 6;            
            if (this._isSupported)            
                DwmIsCompositionEnabled(out this._isSupported);            
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="container">Parent container</param>
        public GlassForm(System.ComponentModel.IContainer container)
        {
            container.Add(this);
            InitializeComponent();
        }

        protected override void OnPaintBackground(System.Windows.Forms.PaintEventArgs e)
        {
            if (this._isSupported && this._glassEnabled)
                e.Graphics.Clear(this.TransparencyKey);
            else
                base.OnPaintBackground(e);
        }


        protected override void WndProc(ref System.Windows.Forms.Message m)
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

        /// <summary>
        /// Callback on Desktop Window Manager changed
        /// </summary>
        /// <param name="eventArgs">Event arguments</param>
        private void OnDWMCompositionChanged(EventArgs eventArgs)
        {
            if (this.IsGlassSupported && this.GlassEnabled)
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
                    DwmExtendFrameIntoClientArea(this.Handle, ref this._glassMargins);
                }
            }
            else
            {
                this.BackColor = this.GlassBackColor;
                this.TransparencyKey = System.Drawing.Color.Empty;
                MARGINS margins = new MARGINS(0);
                DwmExtendFrameIntoClientArea(this.Handle, ref margins);                
            }
        }
        
        public void GlassRefresh()
        {
            if(this._isSupported)
                OnDWMCompositionChanged(null);
        }

        /**
         * Wrappers
         * 
         */ 

        private const int WM_DWMCOMPOSITIONCHANGED = 0x31E;
        private const int WM_DWMNCRENDERINGCHANGED = 0x31F;

        [System.Runtime.InteropServices.DllImport("dwmapi.dll")]
        public static extern IntPtr DwmExtendFrameIntoClientArea(IntPtr hWnd, ref MARGINS pMarInset);
        [System.Runtime.InteropServices.DllImport("dwmapi.dll", SetLastError = true)]
        private static extern IntPtr DwmIsCompositionEnabled([System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.Bool)]out bool pfEnabled);
        [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
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
    }
}
