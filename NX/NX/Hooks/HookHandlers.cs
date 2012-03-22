using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Text;

namespace NX.Hooks
{    
    /// <summary>
    /// EventArgs for Hook Events
    /// </summary>
    public class HookEventArgs
    {
        private bool _alt;
        private bool _ctrl;
        private bool _shift;
        private bool _caps;
        private bool _num;
        private bool _scroll;

        public bool Alt
        {
            get
            {
                return this._alt;
            }
        }
        public bool Shift
        {
            get
            {
                return this._shift;
            }
        }
        public bool Control
        {
            get
            {
                return this._ctrl;
            }
        }
        public bool CapsLock
        {
            get
            {
                return this._caps;
            }
        }

        public bool NumLock
        {
            get
            {
                return this._num;
            }
        }
        public bool ScrollLock
        {
            get
            {
                return this._scroll;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="alt">ALT key was pressed</param>
        /// <param name="ctrl">CTRL key was pressed</param>
        /// <param name="shift">SHIFT key was pressed</param>
        /// <param name="caps">CAPS LOCK was toggled</param>
        /// <param name="num">NUM LOCK was toggled</param>
        /// <param name="scroll">SCROLL LOCK was toggled</param>
        public HookEventArgs(bool alt, bool ctrl, bool shift, bool caps, bool num, bool scroll)
        {
            this._alt = alt;
            this._ctrl = ctrl;
            this._shift = shift;
            this._caps = caps;
            this._num = num;
            this._scroll = scroll;
        }
    }

    public class KeyHookEventArgs : HookEventArgs
    {
        private bool _injected;
        private Keys _keyCode;
        private int _keyValue;
        private char _keyChar;

        public bool Handled { get; set; }
        
        public bool Injected
        {
            get
            {
                return this._injected;
            }
        }
        public Keys KeyCode
        {
            get
            {
                return this._keyCode;
            }
        }
        public int KeyValue
        {
            get
            {
                return this._keyValue;
            }
        }
        public char KeyChar
        {
            get
            {
                return this._keyChar;
            }
        }
        
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="keyData">Key code</param>
        /// <param name="injected">The event was injected</param>
        /// <param name="alt">ALT key was pressed</param>
        /// <param name="ctrl">CTRL key was pressed</param>
        /// <param name="shift">SHIFT key was pressed</param>
        /// <param name="caps">CAPS LOCK was toggled</param>
        /// <param name="num">NUM LOCK was toggled</param>
        /// <param name="scroll">SCROLL LOCK was toggled</param>
        public KeyHookEventArgs(uint keyData, bool injected, bool alt, bool ctrl, bool shift, bool caps, bool num, bool scroll)
            : base(alt, ctrl, shift, caps, num, scroll)
        {
            this._injected = injected;
            this._keyCode = (Keys)keyData;
            this._keyValue = (int)keyData;            
            this._keyChar = '\0';   
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="keyData">Key code</param>
        /// <param name="keyChar">Key character</param>
        /// <param name="injected">The event was injected</param>
        /// <param name="alt">ALT key was pressed</param>
        /// <param name="ctrl">CTRL key was pressed</param>
        /// <param name="shift">SHIFT key was pressed</param>
        /// <param name="caps">CAPS LOCK was toggled</param>
        /// <param name="num">NUM LOCK was toggled</param>
        /// <param name="scroll">SCROLL LOCK was toggled</param>
        public KeyHookEventArgs(uint keyData, char keyChar, bool injected, bool alt, bool ctrl, bool shift, bool caps, bool num, bool scroll)
            : this(keyData, injected, alt, ctrl, shift, caps, num, scroll)
        {            
            this._keyChar = keyChar;         
        }
    }

    public class MouseHookEventArgs : HookEventArgs
    {
        private bool _injected;
        private MouseButtons _button;
        private int _clicks;
        private System.Drawing.Point _loc;
        private int _delta;

        public bool Injected
        {
            get
            {
                return this._injected;
            }
        }
        public MouseButtons Button
        {
            get
            {
                return this._button;
            }
        }
        public int ClickCount
        {
            get
            {
                return this._clicks;
            }
        }
        public System.Drawing.Point Location
        {
            get
            {
                return this._loc;
            }
        }
        public int ScrollAmount
        {
            get
            {
                return this._delta;
            }
        }
        public int MouseDelta
        {
            get { return 120; }
        }
        public bool Handled { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="injected">The event was injected</param>
        /// <param name="button">The mouse button pressed</param>
        /// <param name="clickCount">Number of clicks</param>
        /// <param name="x">X position of cursor</param>
        /// <param name="y">Y position of cursor</param>
        /// <param name="mouseDelta">Number of scrolls</param>        
        /// <param name="alt">ALT key was pressed</param>
        /// <param name="ctrl">CTRL key was pressed</param>
        /// <param name="shift">SHIFT key was pressed</param>
        /// <param name="caps">CAPS LOCK was toggled</param>
        /// <param name="num">NUM LOCK was toggled</param>
        /// <param name="scroll">SCROLL LOCK was toggled</param>
        public MouseHookEventArgs(bool injected, MouseButtons button, int clickCount, int x, int y, int mouseDelta, bool alt, bool ctrl, bool shift, bool caps, bool num, bool scroll)
            : base(alt, ctrl, shift, caps, num, scroll)
        {
            this._injected = injected;
            this._button = button;
            this._clicks = clickCount;
            this._loc = new System.Drawing.Point(x, y);
            this._delta = mouseDelta/this.MouseDelta;
           
        }
    }

    /// <summary>
    /// Can hold simultaneously pressed keys and the type of event
    /// </summary>
    public class KeyHookEventArgsEx : KeyHookEventArgs
    {
        private HookEventCodec.EventType _type;
        public HookEventCodec.EventType Type { get { return this._type; } }
        private List<System.Windows.Forms.Keys> _pressedKeys = null;
        public List<System.Windows.Forms.Keys> PressedKeys { get { return this._pressedKeys; } }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="args">Original KeyHookEventArgs object</param>
        /// <param name="type">Type of event</param>
        /// <param name="pressedKeys">List of keys pressed simultaneously</param>
        /// <param name="pressedChar">Key chracter pressed</param>
        public KeyHookEventArgsEx(KeyHookEventArgs args, HookEventCodec.EventType type, List<System.Windows.Forms.Keys> pressedKeys, char pressedChar)
            : base((uint)args.KeyValue, pressedChar, args.Injected, args.Alt, args.Control, args.Shift, args.CapsLock, args.NumLock, args.ScrollLock)
        {
            this._type = type;
            if (pressedKeys == null)
                this._pressedKeys = new List<System.Windows.Forms.Keys>();
            else
                this._pressedKeys = pressedKeys;
        }
    }

    /// <summary>
    /// Can hold the type of event
    /// </summary>
    public class MouseHookEventArgsEx : MouseHookEventArgs
    {
        private HookEventCodec.EventType _type;
        public HookEventCodec.EventType Type { get { return this._type; } }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="args">Original MouseHookEventArgs object</param>
        /// <param name="type">Type of event</param>
        public MouseHookEventArgsEx(MouseHookEventArgs args, HookEventCodec.EventType type)
            : base(args.Injected, args.Button, args.ClickCount, args.Location.X, args.Location.Y, args.MouseDelta, args.Alt, args.Control, args.Shift, args.CapsLock, args.NumLock, args.ScrollLock)
        {
            this._type = type;
        }
    }
}
