using System;
using System.Collections.Generic;
using System.Text;

namespace DatasetGenerator
{
    class GlyphBlockFormatter
    {
        public static string Format(GlyphBlock block, string[] gardinerSignsInBlockInOrder)
        {
            switch (block.Size) 
            {
                case 1:
                    return gardinerSignsInBlockInOrder[0];
                case 2:
                    return TwoGlyphCase(block, gardinerSignsInBlockInOrder);
                case 3:
                    return ThreeGlyphCase(block, gardinerSignsInBlockInOrder);
                case 4:
                    return String.Format("({0}-{1}):({2}-{3})", gardinerSignsInBlockInOrder[0],
                                                                gardinerSignsInBlockInOrder[1],
                                                                gardinerSignsInBlockInOrder[2],
                                                                gardinerSignsInBlockInOrder[3]);
                default:
                    throw new ArgumentException("Block has invalid number of glyphs.");
            }
        }

        private static string TwoGlyphCase(GlyphBlock block, string[] gardiners)
        {
            /*
             * If images in block have similar y-values and different x-values:
             *     - Return gardiner IDs joined by '*'
             *     - First is gardiners[0], second is gardiners[1]
             * If images in block have similar x-values and different y-values:
             *     - Return gardiner IDs joined by ':'
             *     - First is gardiners[0], second is gardiners[1]
             */
            throw new NotImplementedException();
        }

        private static string ThreeGlyphCase(GlyphBlock block, string[] gardiners)
        {
            /*
             * If 2 of the 3 images have similar x-values and different y-values
             *      - Take 2 with similar x-values and String.Join with ':', then add parentheses around
             *      - If outlier image's gardiner == gardiners[0], return string 0-(1:2) formatted with gardiner indexes 0..2
             *      - Else if outlier image's gardiner == gardiners[-1], return string (0:1)-2 formatted with gardiner indexes 0..2
             *      
             * If 2 of the 3 images have similar y-values and different x-values:
             *      - Take 2 with similar y-values and String.Join with '-', then add parentheses around
             *      - If outlier image's gardiner == gardiners[0], return string 0:(1-2) formatted with gardiner indexes 0..2
             *      - Else if outlier image's gardiner == gardiners[-1], return string (0-1):2 formatted with gardiner indexes 0..2
             */
            throw new NotImplementedException();
        }
    }
}
