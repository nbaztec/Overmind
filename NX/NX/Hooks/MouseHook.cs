using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

using NX.WinApi;

namespace NX.Hooks
{
    /// <summary>
    /// Manages the LL MouseHook
    /// </summary>
    public class MouseHook
    {
        private static int _oldX;
        private static int _oldY;
        
        // To Detect Double Clicks
        private int _prevClkTime = 0;
        private MouseButtons _prevClkButton = MouseButtons.None;

        private USER32.HookProc mseCallbackDelegate = null;
        private IntPtr mseHook = IntPtr.Zero;

        /// <summary>
        /// Delegate for rasing mouse events
        /// </summary>
        /// <param name="e">Event args</param>
        public delegate void MouseHandler(MouseHookEventArgs e);

        public event MouseHandler MouseDown;
        public event MouseHandler MouseClicked;
        public event MouseHandler MouseDoubleClicked;        
        public event MouseHandler MouseUp;
        public event MouseHandler MouseMove;
        public event MouseHandler MouseWheel;

        /// <summary>
        /// Constructor
        /// </summary>
        public MouseHook()
        {
            this.mseCallbackDelegate = new USER32.HookProc(this.MouseCallback);            
        }

        /// <summary>
        /// Check if hook is enabled
        /// </summary>
        /// <returns>True if hook is enabled</returns>
        public bool IsHooked()
        {
            return this.mseHook != IntPtr.Zero;
        }

        /// <summary>
        /// Enables the hook
        /// </summary>
        /// <returns>True if hook is enabled</returns>
        public bool EnableHook()
        {
            if (this.mseHook != IntPtr.Zero)
                return true;

            this.mseHook = USER32.SetWindowsHookEx(HookType.WH_MOUSE_LL, this.mseCallbackDelegate, KERNEL32.GetModuleHandle(System.Diagnostics.Process.GetCurrentProcess().MainModule.ModuleName), 0);
            return this.mseHook != IntPtr.Zero;
        }

        /// <summary>
        /// Disables the hook
        /// </summary>
        /// <returns>True if hook is disabled</returns>
        public bool DisableHook()
        {
            if (this.mseHook == IntPtr.Zero)
                return true;

            if (USER32.UnhookWindowsHookEx(this.mseHook))
            {
                this.mseHook = IntPtr.Zero;
                return true;
            }

            return false;
        }        

        /// <summary>
        /// Checks if the event is a double-click event
        /// </summary>
        /// <param name="button">Mouse button pressed</param>
        /// <param name="time">Current timestamp</param>
        /// <returns>True if the event is a double-click event</returns>
        private bool IsDoubleClick(MouseButtons button, int time)
        {
            if (button == this._prevClkButton && (time - this._prevClkTime) <= SystemInformation.DoubleClickTime)
            {
                this._prevClkTime = time;
                return true;
            }
            else
            {
                this._prevClkButton = button;
                this._prevClkTime = time;
                return false;
            }
        }

        /// <summary>
        /// Callback for LL MouseHook
        /// </summary>        
        private IntPtr MouseCallback(int code, IntPtr wParam, IntPtr lParam)
        {            
            if (code < 0)
                return USER32.CallNextHookEx(IntPtr.Zero, code, wParam, lParam);

            // Else Process

            bool handled = false;
            USER32.MSLLHOOKSTRUCT mse = (USER32.MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(USER32.MSLLHOOKSTRUCT));            
            int wP = wParam.ToInt32();

            MouseButtons button = MouseButtons.None;
            short mouseDelta = 0;
            int clickCount = 0;
            bool mouseDown = false;
            bool mouseUp = false;
            bool injected = (mse.flags == USER32.MSLLHOOKSTRUCTFlags.LLMHF_INJECTED);
            
            bool isDownAlt = ((USER32.GetKeyState((uint)VirtualKey.VK_MENU) & 0x80) == 0x80 ? true : false);
            bool isDownCtrl = ((USER32.GetKeyState((uint)VirtualKey.VK_CONTROL) & 0x80) == 0x80 ? true : false);
            bool isDownShift = ((USER32.GetKeyState((uint)VirtualKey.VK_SHIFT) & 0x80) == 0x80 ? true : false);
            bool isDownCapslock = (USER32.GetKeyState((uint)VirtualKey.VK_CAPITAL) != 0 ? true : false);
            bool isDownNumlock = (USER32.GetKeyState((uint)VirtualKey.VK_NUMLOCK) != 0 ? true : false);
            bool isDownScrolllock = (USER32.GetKeyState((uint)VirtualKey.VK_SCROLL) != 0 ? true : false);
            
            switch ((WM)wP)
            {
                case WM.LBUTTONDOWN:
                    mouseDown = true;
                    button = MouseButtons.Left;                    
                    break;
                case WM.LBUTTONUP:
                    mouseUp = true;
                    button = MouseButtons.Left;
                    clickCount = 1;
                    break;
                case WM.LBUTTONDBLCLK:
                    button = MouseButtons.Left;
                    clickCount = 2;
                    break;
                case WM.RBUTTONDOWN:
                    mouseDown = true;
                    button = MouseButtons.Right;
                    clickCount = 1;
                    break;
                case WM.RBUTTONUP:
                    mouseUp = true;
                    button = MouseButtons.Right;
                    clickCount = 1;
                    break;
                case WM.RBUTTONDBLCLK:
                    button = MouseButtons.Right;
                    clickCount = 2;
                    break;
                case WM.MBUTTONDOWN:
                    mouseDown = true;
                    button = MouseButtons.Middle;
                    break;
                case WM.MBUTTONUP:
                    mouseUp = true;
                    button = MouseButtons.Middle;
                    clickCount = 1;
                    break;
                case WM.MBUTTONDBLCLK:
                    button = MouseButtons.Middle;
                    clickCount = 2;
                    break;
                case WM.XBUTTONDOWN:
                    mouseDown = true;                    
                    button = (((mse.mouseData >> 16) & 0xffff) == 1)? MouseButtons.XButton1 : MouseButtons.XButton2;
                    break;
                case WM.XBUTTONUP:
                    mouseUp = true;
                    button = (((mse.mouseData >> 16) & 0xffff) == 1) ? MouseButtons.XButton1 : MouseButtons.XButton2;
                    clickCount = 1;
                    break;
                case WM.XBUTTONDBLCLK:
                    button = (((mse.mouseData >> 16) & 0xffff) == 1) ? MouseButtons.XButton1 : MouseButtons.XButton2;
                    clickCount = 2;
                    break;
                case WM.MOUSEWHEEL:                    
                    mouseDelta = (short)((mse.mouseData >> 16) & 0xffff);
                    break;
            }

            MouseHookEventArgs e = new MouseHookEventArgs(injected, button, clickCount, mse.pt.X, mse.pt.Y, mouseDelta, isDownAlt, isDownCtrl, isDownShift, isDownCapslock, isDownNumlock, isDownScrolllock);

            // Mouse up
            if (this.MouseUp!=null && mouseUp)            
                this.MouseUp.Invoke(e);            

            // Mouse down
            if (this.MouseDown != null && mouseDown)            
                this.MouseDown.Invoke(e);            

            // If someone listens to click and a click is heppened
            if (this.MouseClicked != null && mouseUp && clickCount > 0)
            {
                if (this.IsDoubleClick(button, mse.time))                
                    clickCount = 2;                
                else                
                    this.MouseClicked.Invoke(e);
            }

            // If someone listens to double click and a click is heppened
            if (this.MouseDoubleClicked != null && mouseUp && clickCount == 2)            
                this.MouseDoubleClicked.Invoke(e);            

            // Wheel was moved
            if (this.MouseWheel!=null && mouseDelta != 0)            
                this.MouseWheel.Invoke(e);            
            
            // Mouse Move Event if mouse was moved
            if (this.MouseMove != null && ( MouseHook._oldX != mse.pt.X || MouseHook._oldY != mse.pt.Y))
            {
                MouseHook._oldX = mse.pt.X;
                MouseHook._oldY = mse.pt.Y;                
                this.MouseMove.Invoke(e);
            }
        
            if (handled)
                return new IntPtr(-1);

            return USER32.CallNextHookEx(this.mseHook, code, wParam, lParam);
        }
    }
}
