using System;
using System.Drawing;
using System.Text;

using NX.Hooks;

namespace NX_Overmind
{
    class ScreenCapture
    {
        //private Queue<System.Drawing.Bitmap> screenLog = new Queue<System.Drawing.Bitmap>();
        Bitmap screenLog = null;
        private System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();                

        public bool CaptureMouse { get; set; }
        public int CaptureInterval { get; set; }

        public ScreenCapture()
        {
            this.CaptureMouse = true;
            this.CaptureInterval = 1000;
            this.timer.Tick += new EventHandler(timer_Tick);
        }

        public ScreenCapture(int interval)
            : this()
        {
            this.CaptureInterval = interval;            
        }

        public bool IsRunning()
        {
            return this.timer.Enabled;
        }

        public void Start()
        {            
            this.timer.Interval = this.CaptureInterval;
            this.timer.Enabled = true;
        }
        
        public void Stop()
        {
            this.timer.Enabled = false;
        }

        void timer_Tick(object sender, EventArgs e)
        {
            this.Capture();
        }

        private void Capture()
        {
            Bitmap bmp = ScreenSnap.ScreenSnapshot();            
            if (this.CaptureMouse)
                bmp = ScreenSnap.PlaceCursor(bmp);            
            //this.screenLog.Enqueue(bmp);           
            this.screenLog = bmp;
        }

        /*public static System.IO.Stream ToStream(System.Drawing.Bitmap bmp, System.Drawing.Imaging.ImageFormat fmt)
        {
            System.IO.MemoryStream stream = new System.IO.MemoryStream();
            if (bmp != null)
            {
                bmp.Save(stream, fmt);
                bmp.Dispose();
                return stream;
            }
            return stream;
        }*/

        public System.Drawing.Bitmap ProcessLog()
        {
            lock (this)
            {                
                System.Drawing.Bitmap log = this.screenLog;
                this.screenLog = null;
                return log;
                             
                //return this.screenLog;
            }
        }
    }
}
