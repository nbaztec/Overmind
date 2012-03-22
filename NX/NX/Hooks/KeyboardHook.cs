using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

using NX.WinApi;

namespace NX.Hooks
{
    /// <summary>
    /// Manages the LL Keyborad Hook
    /// </summary>
    public class KeyboardHook
    {
        private USER32.HookProc kbdCallbackDelegate = null;
        
        private IntPtr kbdHook = IntPtr.Zero;                   // Hook
        
        /// <summary>
        /// Delegate for rasing key events
        /// </summary>
        /// <param name="e">Event args</param>
        public delegate void KeyHandler(KeyHookEventArgs e);                

        public event KeyHandler KeyDown;
        public event KeyHandler KeyPressed;
        public event KeyHandler KeyUp;

        /// <summary>
        /// Constructor
        /// </summary>
        public KeyboardHook()
        {
            this.kbdCallbackDelegate = new USER32.HookProc(this.KeyboardCallback);                        
        }
        
        /// <summary>
        /// Check if hook is enabled
        /// </summary>
        /// <returns>True if hook is enabled</returns>
        public bool IsHooked()
        {
            return this.kbdHook != IntPtr.Zero;
        }

        /// <summary>
        /// Enables the hook
        /// </summary>
        /// <returns>True if hook is enabled</returns>
        public bool EnableHook()
        {
            if (this.kbdHook != IntPtr.Zero)
                return true;

            this.kbdHook = USER32.SetWindowsHookEx(HookType.WH_KEYBOARD_LL, this.kbdCallbackDelegate, KERNEL32.GetModuleHandle(System.Diagnostics.Process.GetCurrentProcess().MainModule.ModuleName), 0);
            return this.kbdHook != IntPtr.Zero;
        }

        /// <summary>
        /// Disables the hook
        /// </summary>
        /// <returns>True if hook is disabled</returns>
        public bool DisableHook()
        {
            if (this.kbdHook == IntPtr.Zero)
                return true;

            if(USER32.UnhookWindowsHookEx(this.kbdHook))
            {
                this.kbdHook = IntPtr.Zero;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Callback for LL Keyboard Hook
        /// </summary>
        private IntPtr KeyboardCallback(int code, IntPtr wParam, IntPtr lParam)
        {            
            if (code < 0)                            
                return USER32.CallNextHookEx(IntPtr.Zero, code, wParam, lParam);            

            // Else Process

            bool handled = false;
            USER32.KBDLLHOOKSTRUCT kbd = (USER32.KBDLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(USER32.KBDLLHOOKSTRUCT));
            int wP = wParam.ToInt32();

            bool isInjected = kbd.flags == USER32.KBDLLHOOKSTRUCTFlags.LLKHF_INJECTED;
            bool isDownAlt = ((USER32.GetKeyState((uint)VirtualKey.VK_MENU) & 0x80) == 0x80 ? true : false);
            bool isDownCtrl = ((USER32.GetKeyState((uint)VirtualKey.VK_CONTROL) & 0x80) == 0x80 ? true : false);
            bool isDownShift = ((USER32.GetKeyState((uint)VirtualKey.VK_SHIFT) & 0x80) == 0x80 ? true : false);
            bool isDownCapslock = (USER32.GetKeyState((uint)VirtualKey.VK_CAPITAL) != 0 ? true : false);
            bool isDownNumlock = (USER32.GetKeyState((uint)VirtualKey.VK_NUMLOCK) != 0 ? true : false);
            bool isDownScrolllock = (USER32.GetKeyState((uint)VirtualKey.VK_SCROLL) != 0 ? true : false);

            // If Key Down
            if (this.KeyDown != null && (wP == (int)WM.SYSKEYDOWN || wP == (int)WM.KEYDOWN))
            {
                KeyHookEventArgs e = new KeyHookEventArgs(kbd.vkCode, isInjected, isDownAlt, isDownCtrl, isDownShift, isDownCapslock, isDownNumlock, isDownScrolllock);
                this.KeyDown.Invoke(e);                
                handled = e.Handled;
            }
            
            // Key Press
            if (this.KeyPressed != null && wP == (int)WM.KEYDOWN)
            {                                
                byte[] keyState = new byte[256];
                USER32.GetKeyboardState(keyState);
                byte[] inBuffer = new byte[2];
                if (USER32.ToAscii(kbd.vkCode, kbd.scanCode, keyState, inBuffer, (uint)kbd.flags) == 1)
                {
                    char key = (char)inBuffer[0];
                    if ((isDownCapslock ^ isDownShift) && Char.IsLetter(key)) 
                        key = Char.ToUpper(key);
                    KeyHookEventArgs e = new KeyHookEventArgs(kbd.vkCode, key, isInjected, isDownAlt, isDownCtrl, isDownShift, isDownCapslock, isDownNumlock, isDownScrolllock);
                    this.KeyPressed.Invoke(e);
                    handled = handled || e.Handled;
                }
            }
            
            // Key Up
            if (this.KeyUp != null && (wP == (int)WM.SYSKEYUP || wP == (int)WM.KEYUP))
            {
                Keys keyData = (Keys)kbd.vkCode;
                KeyHookEventArgs e = new KeyHookEventArgs(kbd.vkCode, isInjected, isDownAlt, isDownCtrl, isDownShift, isDownCapslock, isDownNumlock, isDownScrolllock);
                this.KeyUp.Invoke(e);
                handled = handled || e.Handled;
            }
            if (handled)
                return new IntPtr(-1);
            return USER32.CallNextHookEx(this.kbdHook, code, wParam, lParam);
        }
    }
}
