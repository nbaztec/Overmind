using System;
using System.Windows.Forms;
using System.IO;

using NX.Inputs;
using NX.Hooks;
using NX.Log.NeuroLog;

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
        
        private OvermindLogManager _neuroLog = new OvermindLogManager("NeuroLog", OvermindLogManager.LogEntryType.ActionCenter_Event);

        public EventManager(ActionCenter.ActionCenterInternalEventHandler dispatchData)
        {
            this._ki = new KeyInput();
            this._mi = new MouseInput();
            this.DispatchData = dispatchData;
            this.NormalizedPrecisionFactor = 4;
            this._neuroLog.WriteFormat("Initialization", "Normalized Precision Factor: {0}", this.NormalizedPrecisionFactor);
            //this._mi.MouseMove(1920, 1080);
        }

        public void EncodeEvent(int id, EventArgs args, HookEventCodec.EventType eventType)
        {            
            this._neuroLog.Write("Encode Event");
            if (args is KeyEventArgs)
            {                
                KeyEventArgs ke = args as KeyEventArgs;                
                this.DispatchData(id, ActionCenter.ActionType.KeyEvent, HookEventCodec.GetEncodedData(HookEventCodec.EventClass.KeyEvent, eventType, new KeyHookEventArgs((uint)ke.KeyValue, true, ke.Alt, ke.Control, ke.Shift, false, false, false), new int[]{ke.KeyValue}));
                this._neuroLog.WriteFormat("Dispatched Key Event", "Id: {0}\nType: {1}\nKey Value: {2}\nAlt: {3}\nCtrl: {4}\nShift: {5}", 
                    id, eventType, ke.KeyValue, ke.Alt, ke.Control, ke.Shift);
            }
            else if (args is MouseEventArgs)
            {
                MouseEventArgs me = args as MouseEventArgs;
                this.DispatchData(id, ActionCenter.ActionType.MouseEvent, HookEventCodec.GetEncodedData(HookEventCodec.EventClass.MouseEvent, eventType, new MouseHookEventArgs(true, me.Button, me.Clicks, me.X, me.Y, me.Delta, false, false, false, false, false, false)));
                if(eventType == HookEventCodec.EventType.MouseDoubleClick)
                    this._neuroLog.WriteFormat("Dispatched Mouse Event", "Id: {0}\nType: {1}\nButton: {2}\nClick Count: {3}\nLocation: {4}, {5}\nScroll Delta: {6}",
                        id, eventType, me.Button, me.Clicks, me.X, me.Y, me.Delta);
            }
        }

        public void DecodeEvent(int owner, byte[] data)
        {
            this._neuroLog.WriteFormat("Decode Event", "Owner: {0}\n\nData:\n{1}", owner, data);
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
                        //Log.WriteLine("[KeyDown]->" + ke.KeyCode);
                        this._neuroLog.WriteFormat("Key Down", "Key Code: {0}", ke.KeyCode);
                        this._ki.KeyDown(ke.KeyCode);
                        break;

                    case HookEventCodec.EventType.KeyPress:
                        //Log.WriteLine("[KeyPress]->" + ke.KeyCode);
                        this._neuroLog.WriteFormat("Key Press", "Key Code: {0}", ke.KeyCode);
                        this._ki.KeyPress(ke.KeyCode, ke.Alt, ke.Control, ke.Shift);
                        break;

                    case HookEventCodec.EventType.KeyUp:
                        //Log.WriteLine("[KeyUp]->" + ke.KeyCode);
                        this._neuroLog.WriteFormat("Key Up", "Key Code: {0}", ke.KeyCode);
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
                        //Log.WriteLine("[Click]");
                        this._neuroLog.WriteFormat("Mouse Click", "Button: {0}\nCount: {1}", me.Button, me.ClickCount);
                        this._mi.MouseClick(me.Button, me.ClickCount);
                        break;

                    case HookEventCodec.EventType.MouseDoubleClick:
                        //Log.WriteLine("[DoubleClick]");
                        this._neuroLog.WriteFormat("Mouse Double Click", "Button: {0}", me.Button);
                        this._mi.MouseDoubleClick(me.Button);
                        break;

                    case HookEventCodec.EventType.MouseDown:
                        //Log.WriteLine("[MouseDown]");
                        this._neuroLog.WriteFormat("Mouse Down", "Button: {0}", me.Button);
                        this._mi.MouseDown(me.Button);
                        break;

                    case HookEventCodec.EventType.MouseMove:
                        //int x = (int)(System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width * (me.Location.X / Math.Pow(10, this.NormalizedPrecisionFactor)));
                        //int y = (int)(System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height * (me.Location.Y / Math.Pow(10, this.NormalizedPrecisionFactor)));
                        int x = (int)(System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width * ((ushort)me.Location.X / 65535.0f));
                        int y = (int)(System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height * ((ushort)me.Location.Y / 65535.0f));
                        //this._neuroLog.WriteFormat("Mouse Move", "Normalized Location: {0}, {1}\nClient Location: {2}, {3}", me.Location.X, me.Location.Y, x, y);                        
                        //Log.WriteLine(String.Format("{0} -> {1}, {2} -> {3}", (ushort)me.Location.X, x, (ushort)me.Location.Y, y));
                        /*using (StreamWriter sw = new StreamWriter("log.txt",true))
                        {
                            sw.WriteLine(String.Format("[{0},{1}] | {2}, {3}", me.Location.X.ToString(), me.Location.Y.ToString(), x.ToString(), y.ToString()));
                        }*/
                        this._mi.MouseMove(x, y);                        
                        break;

                    case HookEventCodec.EventType.MouseUp:
                        //Log.WriteLine("[MouseUp]");
                        this._neuroLog.WriteFormat("Mouse Up", "Button: {0}", me.Button);
                        this._mi.MouseUp(me.Button);
                        break;

                    case HookEventCodec.EventType.MouseWheel:
                        //Log.WriteLine("[Scroll]");
                        this._neuroLog.WriteFormat("Mouse Scroll", "Amount: {0}", me.ScrollAmount);
                        this._mi.MouseScroll(me.ScrollAmount);
                        break;                    
                }
            }                       
        }
    }
}
