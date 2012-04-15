using System;
using System.Drawing;
using NX.WinApi;

namespace NX.Imaging
{
    /// <summary>
    /// Test class for image manipulations
    /// </summary>
    public class ImageConverter
    {
        /// <summary>
        /// FreeImage method
        /// </summary>
        /// <param name="bmp"></param>
        /*
        public static void FI_ConvertSave(Bitmap bmp)
        {
            if (bmp != null)
            {                
                using (FreeImageAPI.FreeImageBitmap fiBitmap = FreeImageAPI.FreeImageBitmap.FromHbitmap(bmp.GetHbitmap()))
                {
                    if (fiBitmap.ColorDepth > 24)
                    {
                        fiBitmap.ConvertColorDepth(FreeImageAPI.FREE_IMAGE_COLOR_DEPTH.FICD_08_BPP);
                    }
                    for (int i = 16; i < 256; i++)
                        fiBitmap.Palette.Data[i] = new FreeImageAPI.RGBQUAD(Color.White);

                    FreeImageAPI.Palette pl = new FreeImageAPI.Palette(256);
                    pl.CreateGrayscalePalette();
                    //quantize using the NeuQuant neural-net quantization algorithm                    
                    fiBitmap.Quantize(FreeImageAPI.FREE_IMAGE_QUANTIZE.FIQ_NNQUANT, 16, pl);



                    fiBitmap.Save("test_FreeImageOutput.png", FreeImageAPI.FREE_IMAGE_FORMAT.FIF_PNG, FreeImageAPI.FREE_IMAGE_SAVE_FLAGS.PNG_Z_BEST_COMPRESSION);
                    //bmp = fiBitmap.ToBitmap();
                    //ms = new MemoryStream();
                    //fiBitmap.Save(ms, FreeImageAPI.FREE_IMAGE_FORMAT.FIF_PNG, FreeImageAPI.FREE_IMAGE_SAVE_FLAGS.PNG_Z_DEFAULT_COMPRESSION);
                }
                bmp.Dispose();
            }
        }
        */

        // From wischik.com
        static Bitmap CopyToBpp(System.Drawing.Bitmap b, int bpp)
        {
            if (bpp != 1 && bpp != 8)
                throw new System.ArgumentException("1 or 8", "bpp");

            // Plan: built into Windows GDI is the ability to convert
            // bitmaps from one format to another. Most of the time, this
            // job is actually done by the graphics hardware accelerator card
            // and so is extremely fast. The rest of the time, the job is done by
            // very fast native code.
            // We will call into this GDI functionality from C#. Our plan:
            // (1) Convert our Bitmap into a GDI hbitmap (ie. copy unmanaged->managed)
            // (2) Create a GDI monochrome hbitmap
            // (3) Use GDI "BitBlt" function to copy from hbitmap into monochrome (as above)
            // (4) Convert the monochrone hbitmap into a Bitmap (ie. copy unmanaged->managed)

            int w = b.Width;
            int h = b.Height;
            IntPtr hbm = b.GetHbitmap(); // this is step (1)
            //
            // Step (2): create the monochrome bitmap.
            // "BITMAPINFO" is an interop-struct which we define below.
            // In GDI terms, it's a BITMAPHEADERINFO followed by an array of two RGBQUADs
            GDI32.BITMAPINFOHEADER bmi = new GDI32.BITMAPINFOHEADER();
            bmi.biSize = 40;  // the size of the BITMAPHEADERINFO struct
            bmi.biWidth = w;
            bmi.biHeight = h;
            bmi.biPlanes = 1; // "planes" are confusing. We always use just 1. Read MSDN for more info.
            bmi.biBitCount = (ushort)bpp; // ie. 1bpp or 8bpp
            bmi.biCompression = (uint)GdiFlags.BI_RGB; // ie. the pixels in our RGBQUAD table are stored as RGBs, not palette indexes
            bmi.biSizeImage = (uint)(((w + 7) & 0xFFFFFFF8) * h / 8);
            bmi.biXPelsPerMeter = 1000000; // not really important
            bmi.biYPelsPerMeter = 1000000; // not really important
            // Now for the colour table.
            uint ncols = (uint)1 << bpp; // 2 colours for 1bpp; 256 colours for 8bpp
            bmi.biClrUsed = ncols;
            bmi.biClrImportant = ncols;
            bmi.cols = new uint[256]; // The structure always has fixed size 256, even if we end up using fewer colours
            if (bpp == 1) { bmi.cols[0] = GDI32.MAKERGB(0, 0, 0); bmi.cols[1] = GDI32.MAKERGB(255, 255, 255); }
            else { for (int i = 0; i < ncols; i++) bmi.cols[i] = GDI32.MAKERGB(i, i, i); }
            // For 8bpp we've created an palette with just greyscale colours.
            // You can set up any palette you want here. Here are some possibilities:
            // greyscale: for (int i=0; i<256; i++) bmi.cols[i]=MAKERGB(i,i,i);
            bmi.biClrUsed = 216; bmi.biClrImportant = 216; int[] colv = new int[6] { 0, 51, 102, 153, 204, 255 };
            for (int i = 0; i < 216; i++)
                bmi.cols[i] = GDI32.MAKERGB(colv[i / 36], colv[(i / 6) % 6], colv[i % 6]);
            // rainbow: bmi.biClrUsed=216; bmi.biClrImportant=216; int[] colv=new int[6]{0,51,102,153,204,255};
            //          for (int i=0; i<216; i++) bmi.cols[i]=GDI32.MAKERGB(colv[i/36],colv[(i/6)%6],colv[i%6]);
            // optimal: a difficult topic: http://en.wikipedia.org/wiki/Color_quantization
            // 
            // Now create the indexed bitmap "hbm0"
            IntPtr bits0; // not used for our purposes. It returns a pointer to the raw bits that make up the bitmap.
            IntPtr hbm0 = GDI32.CreateDIBSection(IntPtr.Zero, ref bmi, (uint)GdiFlags.DIB_RGB_COLORS, out bits0, IntPtr.Zero, 0);
            //
            // Step (3): use GDI's BitBlt function to copy from original hbitmap into monocrhome bitmap
            // GDI programming is kind of confusing... nb. The GDI equivalent of "Graphics" is called a "DC".
            IntPtr sdc = USER32.GetDC(IntPtr.Zero);       // First we obtain the DC for the screen
            // Next, create a DC for the original hbitmap
            IntPtr hdc = GDI32.CreateCompatibleDC(sdc);
            GDI32.SelectObject(hdc, hbm);
            // and create a DC for the monochrome hbitmap
            IntPtr hdc0 = GDI32.CreateCompatibleDC(sdc);
            GDI32.SelectObject(hdc0, hbm0);
            // Now we can do the BitBlt:
            GDI32.BitBlt(hdc0, 0, 0, w, h, hdc, 0, 0, TernaryRasterOperations.SRCCOPY);
            // Step (4): convert this monochrome hbitmap back into a Bitmap:
            System.Drawing.Bitmap b0 = System.Drawing.Bitmap.FromHbitmap(hbm0);
            //
            // Finally some cleanup.
            GDI32.DeleteDC(hdc);
            GDI32.DeleteDC(hdc0);
            USER32.ReleaseDC(IntPtr.Zero, sdc);
            GDI32.DeleteObject(hbm);
            GDI32.DeleteObject(hbm0);
            //
            return b0;
        }
    }
}
