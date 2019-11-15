using System.Collections.Generic;
using System.Runtime.Serialization;
using BitMiracle.Docotic.Pdf;

namespace DatasetGenerator.Models
{
    public class GlyphBlock
    {
        private List<ImageData> Images { get; set; }

        public GlyphBlock()
        {
            //Neeeded for deserialization
        }

        public GlyphBlock(List<ImageData> imgs)
        {
            Images = imgs;
        }

        internal void AddImage(ImageData img)
        {
            Images.Add(img);
        }
    }
}
