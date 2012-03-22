using System;
using System.Collections.Generic;
using System.Text;

using NX.Hooks;

namespace NX_Overmind
{
    class MouseCapture
    {
        private MouseHook mh = new MouseHook();
        //private Queue<string> mouseLog = new Queue<string>();
        private LogQueue<byte[]> mouseLog = null;

        public MouseCapture(LogQueue<byte[]> log)
        {
            this.mh.MouseClicked += new MouseHook.MouseHandler(mh_MouseClicked);
            this.mh.MouseDoubleClicked += new MouseHook.MouseHandler(mh_MouseDoubleClicked);
            this.mh.MouseDown += new MouseHook.MouseHandler(mh_MouseDown);
            this.mh.MouseMove += new MouseHook.MouseHandler(mh_MouseMove);
            this.mh.MouseUp += new MouseHook.MouseHandler(mh_MouseUp);
            this.mh.MouseWheel += new MouseHook.MouseHandler(mh_MouseWheel);

            if (log != null)
                this.mouseLog = log;
            else
                this.mouseLog = new LogQueue<byte[]>();
        }

        void mh_MouseWheel(MouseHookEventArgs e)
        {            
            //this.mouseLog.Enqueue(String.Format("[Mouse Scroll: {0} | {1},{2}]", e.ScrollAmount, e.Location.X, e.Location.Y));
            this.mouseLog.Enqueue(HookEventCodec.GetEncodedData(HookEventCodec.EventClass.MouseEvent, HookEventCodec.EventType.MouseWheel, e));
        }

        void mh_MouseUp(MouseHookEventArgs e)
        {
            //this.mouseLog.Enqueue(String.Format("[Mouse Up: {0} | {1},{2}]", e.Button, e.Location.X, e.Location.Y));
        }

        void mh_MouseMove(MouseHookEventArgs e)
        {
            //this.mouseLog.Enqueue(String.Format("[Mouse Move | {0},{1}]", e.Location.X, e.Location.Y));
        }

        void mh_MouseDown(MouseHookEventArgs e)
        {
            //this.mouseLog.Enqueue(String.Format("[Mouse Down: {0} | {1},{2}]", e.Button, e.Location.X, e.Location.Y));
            this.mouseLog.Enqueue(HookEventCodec.GetEncodedData(HookEventCodec.EventClass.MouseEvent, HookEventCodec.EventType.MouseDown, e));
        }

        void mh_MouseDoubleClicked(MouseHookEventArgs e)
        {
            //this.mouseLog.Enqueue(String.Format("[Double Click: {0} | {1},{2}]", e.Button, e.Location.X, e.Location.Y));
            this.mouseLog.Enqueue(HookEventCodec.GetEncodedData(HookEventCodec.EventClass.MouseEvent, HookEventCodec.EventType.MouseDoubleClick, e));
        }

        void mh_MouseClicked(MouseHookEventArgs e)
        {
            //this.mouseLog.Enqueue(String.Format("[Mouse Click: {0} | {1},{2}]", e.Button, e.Location.X, e.Location.Y));
            this.mouseLog.Enqueue(HookEventCodec.GetEncodedData(HookEventCodec.EventClass.MouseEvent, HookEventCodec.EventType.MouseClick, e));
        }

        public bool IsRunning()
        {
            return this.mh.IsHooked();
        }

        public void Start()
        {
            this.mh.EnableHook();
        }

        public void Stop()
        {
            this.mh.DisableHook();
        }

        /*public string[] ProcessLog()
        {
            lock (this)
            {
                string[] log = this.mouseLog.ToArray();
                this.mouseLog.Clear();
                return log;
            }
        }*/
    }
}
