using System;
using System.Windows.Forms;
using System.Text;
using System.Runtime.InteropServices;

using NX.WinApi;

namespace NX.Inputs
{
    /// <summary>
    /// Manages the generation of key events
    /// </summary>
    public class KeyInput
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public KeyInput()
        {
        }

        /// <summary>
        /// Generates a key press event
        /// </summary>
        /// <param name="vkCode">Virtual code of key</param>
        /// <param name="alt">ALT is pressed</param>
        /// <param name="ctrl">CTRL is pressed</param>
        /// <param name="shift">SHIFT is pressed</param>
        public void KeyPress(Keys vkCode, bool alt, bool ctrl, bool shift)
        {
            USER32.INPUT[] inputs = new USER32.INPUT[2];

            USER32.INPUT si = new USER32.INPUT();
            si.type = USER32.INPUTFlags.INPUT_KEYBOARD;
            si.ki = new USER32.KEYBDINPUT();
            si.ki.wVk = (ushort)vkCode;
            si.ki.wScan = (ushort)USER32.MapVirtualKey((ushort)vkCode, USER32.MapType.MAPVK_VK_TO_VSC);
            si.ki.time = 0;
            si.ki.dwFlags = 0;
            //si.ki.dwExtraInfo = USER32.GetMessageExtraInfo();
            inputs[0] = si;

            si = new USER32.INPUT();
            si.type = USER32.INPUTFlags.INPUT_KEYBOARD;
            si.ki = new USER32.KEYBDINPUT();
            si.ki.wVk = (ushort)vkCode;
            si.ki.wScan = (ushort)USER32.MapVirtualKey((ushort)vkCode, USER32.MapType.MAPVK_VK_TO_VSC);
            si.ki.time = 0;
            si.ki.dwFlags = (uint)USER32.INPUTFlags.KEYEVENTF_KEYUP;
            //si.ki.dwExtraInfo = USER32.GetMessageExtraInfo();
            inputs[1] = si;

            USER32.SendInput(2, inputs, Marshal.SizeOf(si));
            // ALTERNATE:
            //this.KeyDown(vkCode);
            //this.KeyUp(vkCode);
        }        

        /// <summary>
        /// Generates a key down event
        /// </summary>
        /// <param name="vkCode">Virtual code of key</param>
        public void KeyDown(Keys vkCode)
        {
            USER32.INPUT[] inputs = new USER32.INPUT[1];
            USER32.INPUT si = new USER32.INPUT();
            si.type = USER32.INPUTFlags.INPUT_KEYBOARD;
            si.ki = new USER32.KEYBDINPUT();
            si.ki.wVk = (ushort)vkCode;
            si.ki.wScan = (ushort)USER32.MapVirtualKey((ushort)vkCode, USER32.MapType.MAPVK_VK_TO_VSC);
            si.ki.time = 0;
            si.ki.dwFlags = 0;
            si.ki.dwExtraInfo = USER32.GetMessageExtraInfo();
            inputs[0] = si;
            
            USER32.SendInput(1, inputs, Marshal.SizeOf(si));
        }

        /// <summary>
        /// Generates a key up event
        /// </summary>
        /// <param name="vkCode">Virtual code of key</param>
        public void KeyUp(Keys vkCode)
        {
            USER32.INPUT[] inputs = new USER32.INPUT[1];
            USER32.INPUT si = new USER32.INPUT();
            si.type = USER32.INPUTFlags.INPUT_KEYBOARD;
            si.ki = new USER32.KEYBDINPUT();
            si.ki.wVk = (ushort)vkCode;
            si.ki.wScan = (ushort)USER32.MapVirtualKey((ushort)vkCode, USER32.MapType.MAPVK_VK_TO_VSC);
            si.ki.time = 0;
            si.ki.dwFlags = (uint)USER32.INPUTFlags.KEYEVENTF_KEYUP;
            si.ki.dwExtraInfo = USER32.GetMessageExtraInfo();
            inputs[0] = si;

            USER32.SendInput(1, inputs, Marshal.SizeOf(si));
        }

        /// <summary>
        /// Checks if the specified key is pressed
        /// </summary>
        /// <param name="vkCode">Virtual code of key</param>
        /// <returns>True if the key is pressed</returns>
        private bool CheckPressState(Keys vkCode)
        {
            return ((USER32.GetKeyState((uint)vkCode) & 0x80) == 0x80 ? true : false);
        }

        /// <summary>
        /// Checks if the specified key is toggled
        /// </summary>
        /// <param name="vkCode">Virtual code of key</param>
        /// <returns>True if the key is toggled</returns>
        private bool CheckToggleState(Keys vkCode)
        {
            return ((USER32.GetKeyState((uint)vkCode) != 0) ? true : false);
        }
    }
}
