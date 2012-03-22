using System;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

using NX.Hooks;
using NX.WinApi;

namespace NX.Inputs
{
    /// <summary>
    /// Manages the generation of mouse events
    /// </summary>
    public class MouseInput
    {        
        private readonly int _delta;
        public int Delta { get { return this._delta; } }

        /// <summary>
        /// Constructor
        /// </summary>
        public MouseInput()
        {
            this._delta = 120;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="deltaAmount">MouseWheelDelta amount</param>
        public MouseInput(int deltaAmount)
        {
            this._delta = deltaAmount;
        }

        /// <summary>
        /// Generates a mouse click event
        /// </summary>
        /// <param name="button">Mouse button</param>
        /// <param name="clickCount">Number of clicks</param>
        /// <param name="delayMs">Delay between each click, default is 50ms</param>
        public void MouseClick(MouseButtons button, int clickCount, int delayMs = 50)
        {
            USER32.INPUT[] inputs = new USER32.INPUT[2];

            USER32.INPUT si_down = new USER32.INPUT();
            si_down.type = USER32.INPUTFlags.INPUT_MOUSE;
            si_down.mi = new USER32.MOUSEINPUT();
            si_down.mi.dx = 0;
            si_down.mi.dy = 0;
            si_down.mi.mouseData = 0;
            si_down.mi.time = 0;            
            si_down.mi.dwExtraInfo = USER32.GetMessageExtraInfo();

            USER32.INPUT si_up = new USER32.INPUT();
            si_up.type = USER32.INPUTFlags.INPUT_MOUSE;
            si_up.mi = new USER32.MOUSEINPUT();
            si_up.mi.dx = 0;
            si_up.mi.dy = 0;
            si_up.mi.mouseData = 0;
            si_up.mi.time = 0;            
            si_up.mi.dwExtraInfo = USER32.GetMessageExtraInfo();
            
            if(button == MouseButtons.Left)
            {
                si_down.mi.dwFlags = (uint)USER32.INPUTFlags.MOUSEEVENTF_LEFTDOWN;
                si_up.mi.dwFlags = (uint)USER32.INPUTFlags.MOUSEEVENTF_LEFTUP;
            }
            else if(button == MouseButtons.Middle)
            {
                si_down.mi.dwFlags = (uint)USER32.INPUTFlags.MOUSEEVENTF_MIDDLEDOWN;
                si_up.mi.dwFlags = (uint)USER32.INPUTFlags.MOUSEEVENTF_MIDDLEUP;
            }
            else if(button == MouseButtons.Right)
            {
                si_down.mi.dwFlags = (uint)USER32.INPUTFlags.MOUSEEVENTF_RIGHTDOWN;
                si_up.mi.dwFlags = (uint)USER32.INPUTFlags.MOUSEEVENTF_RIGHTUP;
            }

            inputs[0] = si_down;
            inputs[1] = si_up;

            for (int i = 0; i < clickCount; i++)
            {                
                USER32.SendInput(2, inputs, Marshal.SizeOf(si_down));
                System.Threading.Thread.Sleep(delayMs);
            }
        }

        /// <summary>
        /// Generates a mouse down event
        /// </summary>
        /// <param name="button">Mouse button</param>
        public void MouseDown(MouseButtons button)
        {
            USER32.INPUT[] inputs = new USER32.INPUT[1];

            USER32.INPUT si = new USER32.INPUT();
            si.type = USER32.INPUTFlags.INPUT_MOUSE;
            si.mi = new USER32.MOUSEINPUT();
            si.mi.dx = 0;
            si.mi.dy = 0;
            si.mi.mouseData = 0;
            si.mi.time = 0;
            si.mi.dwExtraInfo = USER32.GetMessageExtraInfo();
            
            switch(button)
            {
                case MouseButtons.Left:
                    si.mi.dwFlags = (uint)USER32.INPUTFlags.MOUSEEVENTF_LEFTDOWN;                
                    break;
                case MouseButtons.Middle:
                    si.mi.dwFlags = (uint)USER32.INPUTFlags.MOUSEEVENTF_MIDDLEDOWN;                
                    break;
                case MouseButtons.Right:
                    si.mi.dwFlags = (uint)USER32.INPUTFlags.MOUSEEVENTF_RIGHTDOWN;                
                    break;
            }                       

            inputs[0] = si;
            USER32.SendInput(1, inputs, Marshal.SizeOf(si));            
        }

        /// <summary>
        /// Generates a mouse up event
        /// </summary>
        /// <param name="button">Mouse button</param>
        public void MouseUp(MouseButtons button)
        {
            USER32.INPUT[] inputs = new USER32.INPUT[1];

            USER32.INPUT si = new USER32.INPUT();
            si.type = USER32.INPUTFlags.INPUT_MOUSE;
            si.mi = new USER32.MOUSEINPUT();
            si.mi.dx = 0;
            si.mi.dy = 0;
            si.mi.mouseData = 0;
            si.mi.time = 0;
            si.mi.dwExtraInfo = USER32.GetMessageExtraInfo();

            switch (button)
            {
                case MouseButtons.Left:
                    si.mi.dwFlags = (uint)USER32.INPUTFlags.MOUSEEVENTF_LEFTUP;
                    break;
                case MouseButtons.Middle:
                    si.mi.dwFlags = (uint)USER32.INPUTFlags.MOUSEEVENTF_MIDDLEUP;
                    break;
                case MouseButtons.Right:
                    si.mi.dwFlags = (uint)USER32.INPUTFlags.MOUSEEVENTF_RIGHTUP;
                    break;
            }

            inputs[0] = si;
            USER32.SendInput(1, inputs, Marshal.SizeOf(si));
        }

        /// <summary>
        /// Generates a mouse move event
        /// </summary>
        /// <param name="x">X position/offset of screen</param>
        /// <param name="y">Y position/offset of screen</param>
        /// <param name="absolute">Use absolute coordinates or relative coordinates</param>
        public void MouseMove(int x, int y, bool absolute=true)
        {
            USER32.INPUT[] inputs = new USER32.INPUT[1];

            USER32.INPUT si = new USER32.INPUT();
            si.type = USER32.INPUTFlags.INPUT_MOUSE;
            si.mi = new USER32.MOUSEINPUT();
            si.mi.dx = x;
            si.mi.dy = y;
            si.mi.mouseData = 0;
            si.mi.time = 0;
            si.mi.dwExtraInfo = USER32.GetMessageExtraInfo();

            if (absolute)
            {
                int _v = USER32.GetSystemMetrics(SystemMetric.SM_CXSCREEN);
                _v = USER32.GetSystemMetrics(SystemMetric.SM_CYSCREEN);
                float wpx = 65535.0f / USER32.GetSystemMetrics(SystemMetric.SM_CXSCREEN);
                float hpx = 65535.0f / USER32.GetSystemMetrics(SystemMetric.SM_CYSCREEN);
                si.mi.dx = (int)(si.mi.dx*wpx);
                si.mi.dy = (int)(si.mi.dy*hpx);
                si.mi.dwFlags = (uint)(USER32.INPUTFlags.MOUSEEVENTF_MOVE | USER32.INPUTFlags.MOUSEEVENTF_ABSOLUTE | USER32.INPUTFlags.MOUSEEVENTF_VIRTUALDESK);
            }
            else            
                si.mi.dwFlags = (uint)USER32.INPUTFlags.MOUSEEVENTF_MOVE;            

            inputs[0] = si;
            USER32.SendInput(1, inputs, Marshal.SizeOf(si));
        }

        /// <summary>
        /// Generates a mouse scroll event
        /// </summary>
        /// <param name="delta">Number of times and direction to scroll</param>
        public void MouseScroll(int delta)
        {
            USER32.INPUT[] inputs = new USER32.INPUT[1];

            USER32.INPUT si = new USER32.INPUT();
            si.type = USER32.INPUTFlags.INPUT_MOUSE;
            si.mi = new USER32.MOUSEINPUT();
            si.mi.dx = 0;
            si.mi.dy = 0;
            si.mi.mouseData = (int)(delta * this._delta);
            si.mi.time = 0;
            si.mi.dwExtraInfo = USER32.GetMessageExtraInfo();
            si.mi.dwFlags = (uint)USER32.INPUTFlags.MOUSEEVENTF_WHEEL;

            inputs[0] = si;
            USER32.SendInput(1, inputs, Marshal.SizeOf(si));
        }

        /// <summary>
        /// Generates a mouse double click event
        /// </summary>
        /// <param name="button">Mouse button</param>
        public void MouseDoubleClick(MouseButtons button)
        {
            this.MouseClick(button, 2, SystemInformation.DoubleClickTime);
        }
    }
}
