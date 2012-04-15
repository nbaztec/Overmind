using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using NX_Overmind.Actions;

namespace NX_Overmind
{
    public partial class OvermindClientTab : TabPage
    {
        public delegate void ClientEventHandler(int id, object sender, EventArgs e);
        
        public new event ClientEventHandler KeyDown = null;
        public new event ClientEventHandler KeyPress = null;
        public new event ClientEventHandler KeyUp = null;
        public new event ClientEventHandler MouseClick = null;
        public new event ClientEventHandler MouseDoubleClick = null;
        public new event ClientEventHandler MouseDown = null;
        public new event ClientEventHandler MouseEnter = null;
        public new event ClientEventHandler MouseHover = null;
        public new event ClientEventHandler MouseLeave = null;
        public new event ClientEventHandler MouseMove = null;
        public new event ClientEventHandler MouseUp = null;
        public new event ClientEventHandler MouseWheel = null;   


        public delegate void ClientTabEventHandler(object sender, bool enabled);
        public event ClientTabEventHandler OnRecordChanged;
        public event ClientEventHandler OnDisconnectClicked;

        private CaptureManager _captureMgr;
        private ShellManager _shellMgr;
        private OvermindClientShell _overmindShell;

        private int _clientId;
        public int ClientId
        {
            get
            {
                return this._clientId;
            }

        }
        
        public int NormalizedPrecisionFactor { get; set; }
       
        public OvermindClientTab(int clientId, string text, CaptureManager captureMgr, ShellManager shellMgr)
        {
            InitializeComponent();
            
            this.Text = text;
            this._clientId = clientId;
            this._captureMgr = captureMgr;
            this._shellMgr = shellMgr;
            this.NormalizedPrecisionFactor = 4;

            this.toolStripLabelInputIndicator.Image = Properties.Resources.keyboard_delete;
            
            this.pictureScreen.GotFocus += new EventHandler(pictureScreen_GotFocus);
            this.pictureScreen.LostFocus += new EventHandler(pictureScreen_LostFocus);
            
            this.pictureScreen.KeyDown += new KeyEventHandler(pictureScreen_KeyDown);
            this.pictureScreen.KeyPress += new KeyPressEventHandler(pictureScreen_KeyPress);
            this.pictureScreen.KeyUp += new KeyEventHandler(pictureScreen_KeyUp);

            this.pictureScreen.MouseClick += new MouseEventHandler(pictureScreen_MouseClick);
            this.pictureScreen.MouseDoubleClick += new MouseEventHandler(pictureScreen_MouseDoubleClick);
            this.pictureScreen.MouseDown += new MouseEventHandler(pictureScreen_MouseDown);
            this.pictureScreen.MouseEnter += new EventHandler(pictureScreen_MouseEnter);
            this.pictureScreen.MouseHover += new EventHandler(pictureScreen_MouseHover);
            this.pictureScreen.MouseLeave += new EventHandler(pictureScreen_MouseLeave);
            this.pictureScreen.MouseMove += new MouseEventHandler(pictureScreen_MouseMove);
            this.pictureScreen.MouseUp += new MouseEventHandler(pictureScreen_MouseUp);
            this.pictureScreen.MouseWheel += new MouseEventHandler(pictureScreen_MouseWheel);

            Bitmap black_screen = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);    
            Bitmap logo = Properties.Resources.overmind_logo;            
            using (Graphics g = Graphics.FromImage(black_screen))
            {
                float shrink = (black_screen.Width/(float)logo.Width);
                g.FillRectangle(Brushes.Black, 0, 0, black_screen.Width, black_screen.Height);
                g.DrawImage(logo, (black_screen.Width - logo.Width * shrink) / 2, (black_screen.Height - logo.Height * shrink) / 2, logo.Width * shrink, logo.Height * shrink);
            }
            this.pictureScreen.Image = black_screen;

            this._overmindShell = new OvermindClientShell();
            this._overmindShell.TextInputReceived += new OvermindClientShell.DataEventHandler(_overmindShell_TextInputReceived);
        }

        public void AppendToShell(string text)
        {
            this._overmindShell.AppendLine(text);
        }

        void _overmindShell_TextInputReceived(string text)
        {
            this._shellMgr.EncodeEvent(this._clientId, text);   
        }

        void pictureScreen_LostFocus(object sender, EventArgs e)
        {
            this.toolStripLabelInputIndicator.Image = Properties.Resources.keyboard_delete;
            this.toolStripLabelInputIndicator.ToolTipText = "Input Disabled";
            //Cursor.Show();
        }

        void pictureScreen_GotFocus(object sender, EventArgs e)
        {
            this.toolStripLabelInputIndicator.Image = Properties.Resources.keyboard;
            this.toolStripLabelInputIndicator.ToolTipText = "Input Enabled";
            //Cursor.Hide();
        }

        void pictureScreen_MouseWheel(object sender, MouseEventArgs e)
        {
            if (this.MouseWheel != null)
                this.MouseWheel(this._clientId, this, e);
        }

        void pictureScreen_MouseMove(object sender, MouseEventArgs e)
        {
            Point p = NormalizePictureMousePosition(this.pictureScreen.Image, new Point(e.X, e.Y));
            int x = p.X;// (int)(e.X - (this.pictureScreen.Width - this.pictureScreen.Image.Width) / 2.0) - this.pictureScreen.Padding.Horizontal;
            int y = p.Y;// (int)(e.Y - (this.pictureScreen.Height - this.pictureScreen.Image.Height) / 2.0) - -this.pictureScreen.Padding.Vertical;
            
            e = new MouseEventArgs(e.Button, e.Clicks, x, y, e.Delta);
            if (this.MouseMove != null)
            {
                this.MouseMove(this._clientId, this, e);
                /*this._moveEvent = e;
                this.timerMouseMove.Stop();
                this.timerMouseMove.Enabled = true;
                this.timerMouseMove.Start();*/
            }
        }

        protected Point NormalizePictureMousePosition(Image img, Point coordinates)
        {
            // test to make sure our image is not null
            if (img == null) 
                return coordinates;

            float containerWidth = this.pictureScreen.Width;
            float containerHeight = this.pictureScreen.Height;
            float imageWidth = img.Width;
            float imageHeight = img.Height;

            // Make sure our control width and height are not 0 and our 
            // image width and height are not 0
            if (containerWidth == 0 || containerHeight == 0 || imageWidth == 0 || imageHeight == 0) 
                return coordinates;
            // This is the one that gets a little tricky. Essentially, need to check 
            // the aspect ratio of the image to the aspect ratio of the control
            // to determine how it is being rendered           

            float imageAspect = imageWidth / imageHeight;
            float controlAspect = containerWidth / containerHeight;
            float newX = coordinates.X;// +this.pictureScreen.Padding.Left;
            float newY = coordinates.Y;// +this.pictureScreen.Padding.Top;
            float displayWidth = imageWidth;
            float displayHeight = imageHeight;
            
            if (imageAspect > controlAspect)
            {
                // This means that we are limited by width, 
                // meaning the image fills up the entire control from left to right                
                float scale = containerWidth / imageWidth;
                newX /= scale;                
                displayHeight *= scale;
                float diffHeight = containerHeight - displayHeight;
                newY -= (diffHeight/2.0f);
                newY /= scale;
            }
            else
            {
                // This means that we are limited by height, 
                // meaning the image fills up the entire control from top to bottom
                //float ratioHeight = (float)img.Height / this.pictureScreen.Height;
                float scale = containerHeight / imageHeight;
                newY /= scale;                
                displayWidth *= scale;
                float diffWidth = containerWidth - displayWidth;                
                newX -= (diffWidth/2.0f);
                newX /= scale;
            }

            //newX = (int)(Math.Round(newX / img.Width, this.NormalizedPrecisionFactor )* Math.Pow(10, this.NormalizedPrecisionFactor));
            //newY = (int)(Math.Round(newY / img.Height, this.NormalizedPrecisionFactor) * Math.Pow(10, this.NormalizedPrecisionFactor));
                        
            newX = this.NormalizeClip(newX / imageWidth) * 65535;
            newY = this.NormalizeClip(newY / imageHeight) * 65535;

            return new Point((int)newX, (int)newY);
        }

        protected float NormalizeClip(float f)
        {
            f = Math.Min(1.0f, f);
            f = Math.Max(0.0f, f);
            return f;
        }

        void pictureScreen_MouseLeave(object sender, EventArgs e)
        {
            if (this.MouseLeave != null)
                this.MouseLeave(this._clientId, this, e);
            //Cursor.Show();
        }

        void pictureScreen_MouseHover(object sender, EventArgs e)
        {
            if (this.MouseHover != null)
                this.MouseHover(this._clientId, this, e);
        }

        void pictureScreen_MouseEnter(object sender, EventArgs e)
        {
            //Cursor.Hide();
            //this.Select();
            
            if (this.MouseEnter != null)
                this.MouseEnter(this._clientId, this, e);
        }

        private bool _supressMouseEvent = false;
        void pictureScreen_MouseDown(object sender, MouseEventArgs e)
        {
            /*if (this.MouseDown != null)
                this.MouseDown(this._clientId, this, e);*/
            //System.Diagnostics.Trace.WriteLine("Down");
            this.mouseEventTimer.Stop();
            this.__clientHandler = this.MouseDown;
            this.__eventArgs = e;
            this.mouseEventTimer.Start();
        }

        void pictureScreen_MouseUp(object sender, MouseEventArgs e)
        {
            /*if (this.MouseUp != null)
                this.MouseUp(this._clientId, this, e);*/            
            if (!this._supressMouseEvent)
            {
                //System.Diagnostics.Trace.WriteLine("Up");
                this.mouseEventTimer.Stop();
                this.__clientHandler = this.MouseUp;
                this.__eventArgs = e;
                this.mouseEventTimer.Start();
                this._supressMouseEvent = false;
            }
        }

        void pictureScreen_MouseClick(object sender, MouseEventArgs e)
        {
            /*if (this.MouseClick != null)
                this.MouseClick(this._clientId, this, e);
             */
            //System.Diagnostics.Trace.WriteLine("Click");
            this._supressMouseEvent = true;
            this.mouseEventTimer.Stop();
            this.__clientHandler = this.MouseClick;
            this.__eventArgs = e;
            this.mouseEventTimer.Start();
        }

        void pictureScreen_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            /*if (this.MouseDoubleClick != null)
                this.MouseDoubleClick(this._clientId, this, e);
             */
            //System.Diagnostics.Trace.WriteLine("Double Click");
            this.mouseEventTimer.Stop();
            this.__clientHandler = this.MouseDoubleClick;
            this.__eventArgs = e;
            this.mouseEventTimer.Start();
        }

        void pictureScreen_KeyUp(object sender, KeyEventArgs e)
        {
            this.__suppressNextKeyDown = false;
            e.Handled = true;
            if (this.KeyUp != null)
                this.KeyUp(this._clientId, this, e);
        }

        void pictureScreen_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;            
            if (this.KeyPress != null)
                this.KeyPress(this._clientId, this, e);
        }

        private volatile bool __suppressNextKeyDown = false;
        private int __previousKey = 0;
        void pictureScreen_KeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;
            if (this.__previousKey == e.KeyValue && this.__suppressNextKeyDown)
                return;

            if(this.KeyDown != null)
                this.KeyDown(this._clientId, this, e);
            
            this.__previousKey = e.KeyValue;
            this.__suppressNextKeyDown = true;
            this.keyEventTimer.Start();
        }

        private void toolStrip_ItemChecked(object sender, EventArgs e)
        {
            ToolStripButton button = sender as ToolStripButton;
            if (button == this.toolStripButtonRecord)
            {
                if (this.OnRecordChanged != null)
                    this.OnRecordChanged(this, button.Checked);
            }
            else if (button == this.toolStripButtonOptions)
            {
                OvermindClientViewerOptionsForm ovof = new OvermindClientViewerOptionsForm(this._captureMgr.CaptureQuantizePalette, this._captureMgr.CaptureQuantizeDepth, this._captureMgr.CaptureShrinkFactor);
                ovof.ShowDialog();                
                if (ovof.HasValueChanged)
                {
                    this._captureMgr.CaptureQuantizePalette = ovof.ImagePaletteSize;
                    this._captureMgr.CaptureQuantizeDepth = ovof.ImageBitDepth;
                    this._captureMgr.CaptureShrinkFactor = ovof.ImageShrinkFactor;
                }
                ovof.Dispose();
                this._captureMgr.EncodeCaptureQualityInfo(this._clientId);
            }
            else if (button == this.toolStripButtonShell)
            {
                this._overmindShell.ShowDialog();                
            }
            else if (button == this.toolStripButtonDisconnect)
            {
                if (this.OnDisconnectClicked != null)
                    this.OnDisconnectClicked(this._clientId, this, null);
            }
            else
            {
                this.splitLogContainer.Panel1Collapsed = !this.toolStripButtonLogEvent.Checked;
                this.splitLogContainer.Panel2Collapsed = !this.toolStripButtonLogConn.Checked;
                this.splitContainerMain.Panel2Collapsed = !(this.toolStripButtonLogEvent.Checked || this.toolStripButtonLogConn.Checked);
            }
        }

        private EventArgs __eventArgs;
        private ClientEventHandler __clientHandler;
        private void mouseEventTimer_Tick(object sender, EventArgs e)
        {
            //System.Diagnostics.Trace.WriteLine("Dispatch: "+this.__meArgs.Clicks.ToString());
            this.mouseEventTimer.Enabled = false;
            if (this.__clientHandler != null)
                this.__clientHandler(this._clientId, this, this.__eventArgs);            
        }
        
        private void keyEventTimer_Tick(object sender, EventArgs e)
        {
            this.keyEventTimer.Enabled = false;
            this.__suppressNextKeyDown = false;
        }
    }
}
