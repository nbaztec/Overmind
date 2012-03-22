using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using NX.WinApi;

namespace NX.Hooks
{
    public class ScreenSnap
    {
        /**
         * USAGE:
         * 
         * Snapper.SnapshotToFile(Snapper.ScreenSnapshot(), "screen.bmp");        
         * Snapper.SnapshotToFile(Snapper.ForegroundSnapshot(), "screen.gif");
         * Snapper.SnapshotToFile(Snapper.DesktopSnapshot(), "screen.gif");
         * Snapper.SnapshotToFile(Snapper.WindowSnapshot(h), "screen.jpg");
         * Snapper.SnapshotToFile(Snapper.ProcessSnapshot("cuteftppro"), "screen.png");
         *
         */

        /*static Bitmap CaptureDesktop()
        {            
            IntPtr hBitmap;
            IntPtr hDC = USER32.GetDC(USER32.GetDesktopWindow());
            IntPtr hMemDC = GDI32.CreateCompatibleDC(hDC);

            int cx = USER32.GetSystemMetrics
                      (SystemMetric.SM_CXSCREEN);

            int cy = USER32.GetSystemMetrics
                      (SystemMetric.SM_CYSCREEN);

            hBitmap = GDI32.CreateCompatibleBitmap(hDC, cx, cy);

            if (hBitmap != IntPtr.Zero)
            {
                IntPtr hOld = GDI32.SelectObject(hMemDC, hBitmap);
                GDI32.BitBlt(hMemDC, 0, 0, cx, cy, hDC, 0, 0, TernaryRasterOperations.SRCCOPY);

                GDI32.SelectObject(hMemDC, hOld);
                GDI32.DeleteDC(hMemDC);
                USER32.ReleaseDC(USER32.GetDesktopWindow(), hDC);
                Bitmap bmp = System.Drawing.Image.FromHbitmap(hBitmap);
                GDI32.DeleteObject(hBitmap);
                GC.Collect();
                return bmp;
            }
            return null;

        }


        static Bitmap CaptureCursor(ref int x, ref int y)
        {
            Bitmap bmp;
            IntPtr hicon;
            USER32.CURSORINFO ci = new USER32.CURSORINFO();
            USER32.ICONINFO icInfo;
            ci.cbSize = System.Runtime.InteropServices.Marshal.SizeOf(ci);
            if (USER32.GetCursorInfo(out ci))
            {
                if (ci.flags == USER32.CURSORINFOFlags.CURSOR_SHOWING)
                {
                    hicon = USER32.CopyIcon(ci.hCursor);
                    if (USER32.GetIconInfo(hicon, out icInfo))
                    {
                        x = ci.ptScreenPos.X - ((int)icInfo.xHotspot);
                        y = ci.ptScreenPos.Y - ((int)icInfo.yHotspot);

                        Icon ic = Icon.FromHandle(hicon);
                        bmp = ic.ToBitmap();
                        return bmp;
                    }
                }
            }

            return null;
        }

        public static Bitmap CaptureDesktopWithCursor()
        {
            int cursorX = 0;
            int cursorY = 0;
            Bitmap desktopBMP;
            Bitmap cursorBMP;

            Graphics g;
            Rectangle r;

            desktopBMP = CaptureDesktop();
            cursorBMP = CaptureCursor(ref cursorX, ref cursorY);
            if (desktopBMP != null)
            {
                if (cursorBMP != null)
                {
                    r = new Rectangle(cursorX, cursorY, cursorBMP.Width, cursorBMP.Height);
                    g = Graphics.FromImage(desktopBMP);
                    g.DrawImage(cursorBMP, r);
                    g.Flush();

                    return desktopBMP;
                }
                else
                    return desktopBMP;
            }

            return null;

        }*/
        
        /// <summary>
        /// Takes a screen snapshot without the cursor
        /// </summary>
        /// <returns>Bitmap object</returns>
        public static Bitmap ScreenSnapshot()
        {
            Rectangle r = System.Windows.Forms.Screen.PrimaryScreen.Bounds;
            Bitmap bitmap = new Bitmap(r.Width, r.Height, PixelFormat.Format24bppRgb);
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.CopyFromScreen(new Point(r.Left, r.Top), Point.Empty, r.Size);
            }            
            return bitmap;
        }
        
        /// <summary>
        /// Takes a snaphot of a window
        /// </summary>
        /// <param name="hwnd">Handle to the window</param>
        /// <returns>Bitmap object</returns>
        public static Bitmap WindowSnapshot(IntPtr hwnd)
        {
            if (hwnd == IntPtr.Zero)
                return null;
            else
            {
                USER32.RECT rc;
                USER32.GetWindowRect(hwnd, out rc);
                Rectangle r = ApiConverter.ToRectangle(rc);
                Bitmap bitmap = new Bitmap(r.Width, r.Height, PixelFormat.Format24bppRgb);
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    IntPtr hdcBitmap = g.GetHdc();
                    USER32.PrintWindow(hwnd, hdcBitmap, 0);
                }
                return bitmap;
            }
        }

        /// <summary>
        /// Takes the snapshot of the window owned by a process
        /// </summary>
        /// <param name="name">The process name</param>
        /// <returns>Bitmap object</returns>
        public static Bitmap ProcessSnapshot(string name)
        {
            IntPtr hwnd = IntPtr.Zero;
            foreach (System.Diagnostics.Process p in System.Diagnostics.Process.GetProcessesByName(name))
            {
                if (p.MainWindowHandle != IntPtr.Zero)
                {
                    hwnd = p.MainWindowHandle;
                    break;
                }
            }

            if (hwnd == IntPtr.Zero)
                return null;
            else
            {
                USER32.RECT rc;
                USER32.GetWindowRect(hwnd, out rc);
                Rectangle r = ApiConverter.ToRectangle(rc);
                Bitmap bitmap = new Bitmap(r.Width, r.Height, PixelFormat.Format24bppRgb);
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    IntPtr hdcBitmap = g.GetHdc();
                    USER32.PrintWindow(hwnd, hdcBitmap, 0);
                }
                return bitmap;
            }            
        }

        /// <summary>
        /// Takes the snapshot of the Desktop
        /// </summary>
        /// <returns>Bitmap object</returns>
        public static Bitmap DesktopSnapshot()
        {
            IntPtr hwnd = USER32.GetShellWindow();
            if (hwnd == IntPtr.Zero)
                return null;
            else
            {
                USER32.RECT rc;
                USER32.GetWindowRect(hwnd, out rc);
                Rectangle r = ApiConverter.ToRectangle(rc);
                Bitmap bitmap = new Bitmap(r.Width, r.Height, PixelFormat.Format24bppRgb);
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    IntPtr hdcBitmap = g.GetHdc();
                    USER32.PrintWindow(hwnd, hdcBitmap, 0);
                }                
                return bitmap;
            }            
        }

        /// <summary>
        /// Takes the snapshot of the foreground window
        /// </summary>
        /// <returns>Bitmap object</returns>
        public static Bitmap ForegroundSnapshot()
        {
            IntPtr hwnd = USER32.GetForegroundWindow();
            if (hwnd == IntPtr.Zero)
                return null;
            else
            {
                USER32.RECT rc;
                USER32.GetWindowRect(hwnd, out rc);
                Rectangle r = ApiConverter.ToRectangle(rc);
                Bitmap bitmap = new Bitmap(r.Width, r.Height, PixelFormat.Format24bppRgb);
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    IntPtr hdcBitmap = g.GetHdc();
                    USER32.PrintWindow(hwnd, hdcBitmap, 0);
                }
                return bitmap;
            }
        }

        private static int _previousCursor = 0;
        /// <summary>
        /// Checks if the cursor has changed sice the last invokation and returns the serialized new cursor icon
        /// </summary>
        /// <returns>Serialized data bytes if icon has changed else null</returns>
        public static IntPtr CheckCursorChanged()
        {
            USER32.CURSORINFO ci = new USER32.CURSORINFO();
            ci.cbSize = System.Runtime.InteropServices.Marshal.SizeOf(ci);
            USER32.GetCursorInfo(out ci);
            
            int val = ci.hCursor.ToInt32();
            if (val != ScreenSnap._previousCursor)
            {
                ScreenSnap._previousCursor = val;
                //return CaptureCursor();               
                return ci.hCursor; 
            }
            return IntPtr.Zero;
        }

        static Icon CaptureCursor()
        {
            USER32.CURSORINFO cursorInfo = new USER32.CURSORINFO();
            cursorInfo.cbSize = System.Runtime.InteropServices.Marshal.SizeOf(cursorInfo);
            if (!USER32.GetCursorInfo(out cursorInfo))
                return null;

            if (cursorInfo.flags != USER32.CURSORINFOFlags.CURSOR_SHOWING)
                return null;

            IntPtr hicon = USER32.CopyIcon(cursorInfo.hCursor);
            if (hicon == IntPtr.Zero)
                return null;

            USER32.ICONINFO iconInfo;
            if (!USER32.GetIconInfo(hicon, out iconInfo))
                return null;

            //x = cursorInfo.ptScreenPos.X - ((int)iconInfo.xHotspot);
            //y = cursorInfo.ptScreenPos.Y - ((int)iconInfo.yHotspot);

            using (Bitmap maskBitmap = Bitmap.FromHbitmap(iconInfo.hbmMask))
            {
                // Is this a monochrome cursor?
                if (maskBitmap.Height == maskBitmap.Width * 2)
                {
                    Bitmap resultBitmap = new Bitmap(maskBitmap.Width, maskBitmap.Width);

                    Graphics desktopGraphics = Graphics.FromHwnd(USER32.GetDesktopWindow());
                    IntPtr desktopHdc = desktopGraphics.GetHdc();

                    IntPtr maskHdc = GDI32.CreateCompatibleDC(desktopHdc);
                    IntPtr oldPtr = GDI32.SelectObject(maskHdc, maskBitmap.GetHbitmap());

                    using (Graphics resultGraphics = Graphics.FromImage(resultBitmap))
                    {
                        IntPtr resultHdc = resultGraphics.GetHdc();

                        // These two operation will result in a black cursor over a white background.
                        // Later in the code, a call to MakeTransparent() will get rid of the white background.
                        GDI32.BitBlt(resultHdc, 0, 0, 32, 32, maskHdc, 0, 32, TernaryRasterOperations.SRCCOPY);
                        GDI32.BitBlt(resultHdc, 0, 0, 32, 32, maskHdc, 0, 0, TernaryRasterOperations.SRCINVERT);

                        resultGraphics.ReleaseHdc(resultHdc);
                    }

                    IntPtr newPtr = GDI32.SelectObject(maskHdc, oldPtr);
                    USER32.DeleteObject(newPtr);
                    GDI32.DeleteDC(maskHdc);
                    desktopGraphics.ReleaseHdc(desktopHdc);

                    // Remove the white background from the BitBlt calls,
                    // resulting in a black cursor over a transparent background.
                    resultBitmap.MakeTransparent(Color.White);
                    return Icon.FromHandle(resultBitmap.GetHicon());
                }
            }

            return Icon.FromHandle(hicon);            
        }

        /// <summary>
        /// Draws cursor onto the bitmap
        /// </summary>
        /// <param name="bmp">Source bitmap</param>
        /// <returns>Bitmap object</returns>
        public static Bitmap PlaceCursor(Bitmap bmp)
        {
            Graphics g = Graphics.FromImage(bmp);
            {
                USER32.CURSORINFO ci = new USER32.CURSORINFO();
                ci.cbSize = System.Runtime.InteropServices.Marshal.SizeOf(ci);
                USER32.GetCursorInfo(out ci);

                USER32.ICONINFO icInfo;
                if (ci.flags == USER32.CURSORINFOFlags.CURSOR_SHOWING && ci.hCursor.ToInt32() != 0)
                {
                    IntPtr hicon = USER32.CopyIcon(ci.hCursor);
                    int x = System.Windows.Forms.Cursor.Position.X;
                    int y = System.Windows.Forms.Cursor.Position.Y;

                    if (USER32.GetIconInfo(hicon, out icInfo))
                    {
                        x = ci.ptScreenPos.X - ((int)icInfo.xHotspot);
                        y = ci.ptScreenPos.Y - ((int)icInfo.yHotspot);                        
                    }

                    try
                    {                                                
                        //ic.ToBitmap();
                        //g.DrawImage(ic.ToBitmap(), System.Windows.Forms.Cursor.Position.X, System.Windows.Forms.Cursor.Position.Y);
                        g.DrawIcon(Icon.FromHandle(ci.hCursor), x, y);                        
                        USER32.DestroyIcon(hicon);
                    }                    
                    catch(Exception){}
                }
                return bmp;
            }
        }

        /// <summary>
        /// Saves the bitmap to a file on disk
        /// </summary>
        /// <param name="bmp">Source bitmap</param>
        /// <param name="file">Destination filename</param>
        public static void SnapshotToFile(Bitmap bmp, string file)
        {
            if (bmp != null)
            {
                bmp.Save(file, GetImageType(file));
                bmp.Dispose();
            }
        }        

        /// <summary>
        /// Saves the bitmap to a file on disk using encoding
        /// </summary>
        /// <param name="bmp">Source bitmap</param>
        /// <param name="file">Destination filename</param>
        /// <param name="quality">Image quality</param>
        /// <param name="shrinkFactor">Shrink factor</param>
        public static void SnapshotToFile(Bitmap bmp, string file, long quality, float shrinkFactor)
        {
            if (bmp != null)
            {
                bmp = ShrinkBitmap(bmp, shrinkFactor);
                
                EncoderParameters imgEncodingParams = new EncoderParameters(1);
                ImageCodecInfo imgCodecInfo = GetEncoder(GetImageType(file));
                imgEncodingParams.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality);                
                bmp.Save(file, imgCodecInfo, imgEncodingParams);                

                bmp.Dispose();
            }
        }

        /// <summary>
        /// Saves the bitmap to a stream
        /// </summary>
        /// <param name="bmp">Source bitmap</param>
        /// <param name="fmt">Image format</param>
        /// <returns>Image stream</returns>
        public static System.IO.Stream SnapshotToStream(Bitmap bmp, ImageFormat fmt)
        {
            System.IO.Stream stream = null;
            if (bmp != null)
            {
                stream = new System.IO.MemoryStream();
                bmp.Save(stream, fmt);
                bmp.Dispose();
            }
            return stream;
        }

        /// <summary>
        /// Saves the bitmap to a stream using encoding
        /// </summary>
        /// <param name="bmp">Source bitmap</param>
        /// <param name="fmt">Image format</param>
        /// <param name="quality">Image quality</param>
        /// <param name="shrinkFactor">Shrink factor</param>
        /// <returns>Image stream</returns>
        public static System.IO.Stream SnapshotToStream(Bitmap bmp, ImageFormat fmt, long quality, float shrinkFactor)
        {
            System.IO.Stream stream = null;
            if (bmp != null)
            {
                stream = new System.IO.MemoryStream();
                bmp = ShrinkBitmap(bmp, shrinkFactor);

                EncoderParameters imgEncodingParams = new EncoderParameters(1);                
                ImageCodecInfo imgCodecInfo = GetEncoder(fmt);
                imgEncodingParams.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality);                
                bmp.Save(stream, imgCodecInfo, imgEncodingParams);
                bmp.Dispose();
            }
            return stream;
        }

        /// <summary>
        /// Shrinks a bitmap by a factor
        /// </summary>
        /// <param name="bmp">Source bitmap</param>
        /// <param name="factor">Normalized(0 to 0.99) factor to shrink the image by</param>
        /// <returns></returns>
        public static Bitmap ShrinkBitmap(Bitmap bmp, float factor)
        {
            if (bmp == null)
                return null;

            float f = Math.Min(1.0f, factor);
            f = Math.Max(0.0f, factor);

            if (f == 0.0f)
                return bmp;
            else if (f == 1.0f)
                throw new ArgumentException("Cannot shrink an image by 100%");
            
            int w = bmp.Width - (int)(bmp.Width * factor);
            int h = bmp.Height - (int)(bmp.Height * factor);
            Bitmap thumb = new Bitmap(w, h);
            
            using (Graphics g = Graphics.FromImage(thumb))
            {                
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.GammaCorrected;
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Bilinear; 
                g.DrawImage(bmp, new Rectangle(0, 0, w, h));
            }
            return thumb;
        }

        /**
          * Helpers
          */

        /// <summary>
        /// Gets image type from file extension
        /// </summary>
        /// <param name="file">Filename</param>
        /// <returns>ImageFormat type</returns>
        private static ImageFormat GetImageType(string file)
        {
            switch (System.IO.Path.GetExtension(file).TrimStart('.').ToLower())
            {
                case "jpeg":
                case "jpg":
                    return ImageFormat.Jpeg;
                case "gif":
                    return ImageFormat.Gif;
                case "png":
                    return ImageFormat.Png;
                case "exif":
                    return ImageFormat.Exif;
                case "ico":
                    return ImageFormat.Icon;
                case "tiff":
                    return ImageFormat.Tiff;
                default:
                    return ImageFormat.Bmp;
            }
        }

        /// <summary>
        /// Gets encoder from ImageFormat
        /// </summary>
        /// <param name="format">ImageFormat type</param>
        /// <returns>ImageCodec</returns>
        private static ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)                
                    return codec;                
            }
            return null;
        }
    }
}
