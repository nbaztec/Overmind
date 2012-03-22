using System;
using System.Runtime.InteropServices;

namespace NX.WinApi
{
    public class KERNEL32
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr GetModuleHandle(string lpModuleName);
    }
}
