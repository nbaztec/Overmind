using System;
using System.Windows.Forms;
using System.IO;

using NX.Inputs;
using NX.Hooks;

namespace NX_Overmind.Actions
{
    public class EventManager
    {
        private KeyInput _ki = null;
        private MouseInput _mi = null;

        public int NormalizedPrecisionFactor { get; set; }

        /// <summary>
        /// Dispatch data over the network
        /// </summary>
        public event ActionCenter.ActionCenterInternalEventHandler DispatchData = null;

        public EventManager(ActionCenter.ActionCenterInternalEventHandler dispatchData)
        {
            this._ki = new KeyInput();
            this._mi = new MouseInput();
            this.DispatchData = dispatchData;
            this.NormalizedPrecisionFactor = 4;
            //this._mi.MouseMove(1920, 1080);
        }

        public void EncodeEvent(int id, EventArgs args, HookEventCodec.EventType eventType)
        {
            if (args is KeyEventArgs)
            {
                KeyEventArgs ke = args as KeyEventArgs;
                this.DispatchData(id, ActionCenter.ActionType.KeyEvent, HookEventCodec.GetEncodedData(HookEventCodec.EventClass.KeyEvent, eventType, new KeyHookEventArgs((uint)ke.KeyValue, true, ke.Alt, ke.Control, ke.Shift, false, false, false), new int[]{ke.KeyValue}));
            }
            else if (args is MouseEventArgs)
            {
                MouseEventArgs me = args as MouseEventArgs;
                this.DispatchData(id, ActionCenter.ActionType.MouseEvent, HookEventCodec.GetEncodedData(HookEventCodec.EventClass.MouseEvent, eventType, new MouseHookEventArgs(true, me.Button, me.Clicks, me.X, me.Y, me.Delta, false, false, false, false, false, false)));
            }
        }

        public void DecodeEvent(int owner, byte[] data)
        {           
            HookEventCodec.EventClass ec;
            HookEventCodec.EventType et;
            HookEventArgs ea;
            byte[] _b;
            char _c;
                
            HookEventCodec.GetDecodedData(0, data, out ec, out et, out ea, out _b, out _c);
            if(ec == HookEventCodec.EventClass.KeyEvent)
            {
                KeyHookEventArgs ke = ea as KeyHookEventArgs;
                switch (et)
                {
                    case HookEventCodec.EventType.KeyDown:
                        Log.WriteLine("[D]->" + ke.KeyCode);
                        this._ki.KeyDown(ke.KeyCode);
                        break;

                    case HookEventCodec.EventType.KeyPress:
                        Log.WriteLine("[P]->" + ke.KeyCode);
                        this._ki.KeyPress(ke.KeyCode, ke.Alt, ke.Control, ke.Shift);
                        break;

                    case HookEventCodec.EventType.KeyUp:
                        Log.WriteLine("[U]->" + ke.KeyCode);
                        this._ki.KeyUp(ke.KeyCode);
                        break;
                }
            }
            else if (ec == HookEventCodec.EventClass.MouseEvent)
            {
                MouseHookEventArgs me = ea as MouseHookEventArgs;
                KeyHookEventArgs ke = ea as KeyHookEventArgs;
                switch (et)
                {
                    case HookEventCodec.EventType.MouseClick:
                        Log.WriteLine("[*C]");
                        //this._mi.MouseClick(me.Button, me.ClickCount);
                        break;

                    case HookEventCodec.EventType.MouseDoubleClick:
                        Log.WriteLine("[DC]");
                        this._mi.MouseDoubleClick(me.Button);
                        break;

                    case HookEventCodec.EventType.MouseDown:
                        Log.WriteLine("[D]");
                        this._mi.MouseDown(me.Button);
                        break;

                    case HookEventCodec.EventType.MouseMove:
                        //int x = (int)(System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width * (me.Location.X / Math.Pow(10, this.NormalizedPrecisionFactor)));
                        //int y = (int)(System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height * (me.Location.Y / Math.Pow(10, this.NormalizedPrecisionFactor)));
                        int x = (int)(System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width * ((ushort)me.Location.X / 65535.0f));
                        int y = (int)(System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height * ((ushort)me.Location.Y / 65535.0f));
                        Log.WriteLine(String.Format("{0} -> {1}, {2} -> {3}", (ushort)me.Location.X, x, (ushort)me.Location.Y, y));
                        /*using (StreamWriter sw = new StreamWriter("log.txt",true))
                        {
                            sw.WriteLine(String.Format("[{0},{1}] | {2}, {3}", me.Location.X.ToString(), me.Location.Y.ToString(), x.ToString(), y.ToString()));
                        }*/
                        this._mi.MouseMove(x, y);                        
                        break;

                    case HookEventCodec.EventType.MouseUp:
                        Log.WriteLine("[U]");
                        this._mi.MouseUp(me.Button);
                        break;

                    case HookEventCodec.EventType.MouseWheel:
                        this._mi.MouseScroll(me.ScrollAmount);
                        break;                    
                }
            }                       
        }
    }
}
