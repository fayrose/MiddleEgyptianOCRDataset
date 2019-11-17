using System;
using System.Collections.Generic;
using System.Text;

namespace DatasetGenerator
{
    class GlyphBlockFormatter
    {
        public static string FormatGlyphBlock(GlyphBlock block)
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
            throw new NotImplementedException();
        }

        private static string ThreeGlyphCase(GlyphBlock block)
        {
            throw new NotImplementedException();
        }

        private static string FourGlyphCase(GlyphBlock block)
        {
            throw new NotImplementedException();
        }

    }
}
