using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace NX.WinApi
{
    /// <summary>
    /// Converts structures from WINAPI to .NET and vice-versa
    /// </summary>
    public class ApiConverter
    {
        /// <summary>
        /// Converts Rectangle to RECT
        /// </summary>        
        public static USER32.RECT ToRECT(Rectangle r)
        {
            USER32.RECT rect = new USER32.RECT();
            rect.Top = r.Top;
            rect.Right = r.Right;
            rect.Bottom = r.Bottom;
            rect.Left = r.Left;
            return rect;
        }
        /// <summary>
        /// Converts RECT to Rectangle
        /// </summary>
        public static Rectangle ToRectangle(USER32.RECT r)
        {
            return new Rectangle(r.Left, r.Top, r.Right - r.Left, r.Bottom - r.Top);
        }    
    }
}
