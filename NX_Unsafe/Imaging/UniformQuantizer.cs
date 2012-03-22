using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Imaging;

namespace NX.Imaging
{
    /// <summary>
    /// In uniform quantization each axis of the color space is treated independently. 
    /// Each axis is then divided into equal sized segments. The planes perpendicular to 
    /// the axis' that pass through the division points then define regions in the color 
    /// space. The number of these regions is dependent on the scheme used for dividing the 
    /// color space. One possible scheme is to divide the red and green axis into 8 segments 
    /// each and the blue axis into 4 resulting in 256 regions. Another possibility is dividing 
    /// the red and blue into 6 and the green into 7 segments resulting in 252 regions [3]. Each 
    /// one of these regions will produce a color for the color map.
    /// 
    /// Once the color space has been divided each of the original colors is then mapped to the 
    /// region which it falls in. The representative colors for each region is then the average 
    /// of all the colors mapped to that region. Because each of the regions represents an entry
    /// in the color map, the same process for mapping the original colors to a region can be 
    /// repeated for mapping the original colors to colors in the color map. While this algorithm 
    /// is quick and easy to implement it does not yield very good results. Often region in the 
    /// color space will not have any colors mapped to them resulting in color map entries to be
    /// wasted.
    ///
    /// This algorithm can also be applied in a non-uniform manner if the axis are broken on a 
    /// logarithmic scale instead of linear. This will produce slightly better results because 
    /// the human eye cannot distinguish dark colors as well as bright ones.
    /// </summary>
    public unsafe class UniformQuantizer : Quantizer
    {
        internal struct UniformColorSlot
        {
            private Int32 value;
            private Int32 pixelCount;

            /// <summary>
            /// Adds the value to the slot.
            /// </summary>
            /// <param name="component">The color component value.</param>
            public void AddValue(Int32 component)
            {
                value += component;
                pixelCount++;
            }

            /// <summary>
            /// Gets the average, just simple value divided by pixel presence.
            /// </summary>
            /// <returns>The average color component value.</returns>
            public Int32 GetAverage()
            {
                Int32 result = 0;

                if (pixelCount > 0)
                {
                    result = pixelCount == 1 ? value : value / pixelCount;
                }

                return result;
            }
        }

        #region | Fields |

        private UniformColorSlot[] redSlots;
        private UniformColorSlot[] greenSlots;
        private UniformColorSlot[] blueSlots;
        //private int _maxColors = 255;

        #endregion

        #region << IColorQuantizer >>

        public UniformQuantizer(/*int maxColors, int maxColorBits*/)
            : base(false)
        {
            /*if (maxColors != 256)
                throw new ArgumentOutOfRangeException("maxColors", maxColors, "The number of colors should be 256");

            if ((maxColorBits < 1) | (maxColorBits > 8))
                throw new ArgumentOutOfRangeException("maxColorBits", maxColorBits, "This should be between 1 and 8");
            this._maxColors = maxColors - 1;
            */            
        }

        public override void Init()
        {
            this.redSlots = new UniformColorSlot[8];
            this.greenSlots = new UniformColorSlot[8];
            this.blueSlots = new UniformColorSlot[4];            
        }
        /// <summary>
        /// See <see cref="IColorQuantizer.AddColor"/> for more details.
        /// </summary>
        protected override void InitialQuantizePixel(Color32* pixel)
        {
            pixel = Quantizer.ConvertAlpha(pixel);

            int redIndex = pixel->Red >> 5;
            int greenIndex = pixel->Green >> 5;
            int blueIndex = pixel->Blue >> 6;

            redSlots[redIndex].AddValue(pixel->Red);
            greenSlots[greenIndex].AddValue(pixel->Green);
            blueSlots[blueIndex].AddValue(pixel->Blue);
        }

        protected override ColorPalette GetPalette(ColorPalette original)
        {
            ArrayList palette = new ArrayList();

            // NOTE: I was considering either Lambda, or For loop (which should be the fastest), 
            // NOTE: but I used the ForEach loop for the sake of readability. Feel free to convert it.
            /*foreach (UniformColorSlot redSlot in redSlots)
                foreach (UniformColorSlot greenSlot in greenSlots)
                    foreach (UniformColorSlot blueSlot in blueSlots)
                    {
                        Int32 red = redSlot.GetAverage();
                        Int32 green = greenSlot.GetAverage();
                        Int32 blue = blueSlot.GetAverage();

                        Color color = Color.FromArgb(255, red, green, blue);
                        palette.Add(color);
                    }
            */
            int rAvg = 0;
            int gAvg = 0;
            for (int i = 0; i < this.redSlots.Length; i++)
            {
                rAvg = this.redSlots[i].GetAverage();
                for (int j = 0; j < this.greenSlots.Length; j++)
                {
                    gAvg = this.greenSlots[j].GetAverage();
                    for (int k = 0; k < this.blueSlots.Length; k++)
                    {
                        palette.Add(Color.FromArgb(255, rAvg, gAvg, this.blueSlots[k].GetAverage()));
                    }
                }
            }
            // Then convert the palette based on those colors
            for (int index = 0; index < palette.Count; index++)
                original.Entries[index] = (Color)palette[index];

            // Add the transparent color
            //original.Entries[_maxColors] = Color.FromArgb(0, 0, 0, 0);

            return original;
        }

        /// <summary>
        /// Override this to process the pixel in the second pass of the algorithm
        /// </summary>
        /// <param name="pixel">The pixel to quantize</param>
        /// <returns>The quantized value</returns>
        protected override byte QuantizePixel(Color32* pixel)
        {
            pixel = Quantizer.ConvertAlpha(pixel);
            int redIndex = pixel->Red >> 5;
            int greenIndex = pixel->Green >> 5;
            int blueIndex = pixel->Blue >> 6;
            return (byte)((redIndex << 5) + (greenIndex << 2) + blueIndex);
        }

        #endregion
    }
}
