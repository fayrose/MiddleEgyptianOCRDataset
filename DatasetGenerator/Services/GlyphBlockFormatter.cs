using System;
using System.Collections.Generic;
using System.Text;

namespace DatasetGenerator
{
    class GlyphBlockFormatter
    {
        public static string Format(GlyphBlock block)
        {
            switch (block.Size) 
            {
                case 1:
                    return block.Images[0].GardinerSign;
                case 2:
                    return TwoGlyphCase(block);
                case 3:
                    return ThreeGlyphCase(block);
                case 4:
                    return FourGlyphCase(block);
                default:
                    throw new ArgumentException("Block has invalid number of glyphs.");
            }
        }

        private static string TwoGlyphCase(GlyphBlock block)
        {
            /*
             * If images in block have similar y-values and different x-values:
             *     - Return gardiner IDs joined by '*'
             *     - Lower x-value gardiner 1st
             * If images in block have similar x-values and different y-values:
             *     - Return gardiner IDs joined by ':'
             *     - Glyph closer to top of line first (watch for y-coord inversion, not sure if it'll happen)
             */
            throw new NotImplementedException();
        }

        private static string ThreeGlyphCase(GlyphBlock block)
        {
            /*
             * If 2 of the 3 images have similar x-values and different y-values
             *      - Take 2 with similar x-values and String.Join with ':'
             *      - Take closer to top of the line first
             *      - Connect remaining glyph to front or rear of joined string using '-'
             *      depending on x-value of outlier compared to ingroup (lower x-value first)
             *      
             * If 2 of the 3 images have similar y-values and different x-values:
             *      - Take 2 with similar y-values and String.Join with '-'
             *      - Take lower x-value first
             *      - Connect remaining glyph to front or rear of joined string using ':'
             *      after comparing outlier y-value to in-group
             *      - Here take y-value closer to top of the line first
             */
            throw new NotImplementedException();
        }

        private static string FourGlyphCase(GlyphBlock block)
        {
            /*
             * Define the following image variables based on their coordinates:
             *  upperLeft = low x-value, y-value close to line top
             *  upperRight = high x-value, y-value close to line top
             *  lowerLeft = low x-value, y-value close to line bottom
             *  lowerRight = high x-value, y-value close to line bottom
             *  
             *  Then return "(" + upperLeft + "-" + upperRight + "):(" + lowerLeft + "-" + lowerRight + ")", or
             *  (upperLeft-upperRight):(lowerLeft-LowerRight)
             */
            throw new NotImplementedException();
        }

    }
}
