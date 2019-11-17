using System.Collections.Generic;
using System.Runtime.Serialization;
using BitMiracle.Docotic.Pdf;

namespace DatasetGenerator.Models
{
    public class GlyphBlock
    {
        public List<ImageData> Images { get; set; }
        public int Size => Images.Count;
        public GlyphBlock()
        {
            //Neeeded for deserialization
        }

        public GlyphBlock(List<ImageData> imgs)
        {
            Images = imgs;
        }

        public void AddImage(ImageData img)
        {
            Images.Add(img);
        }
    }
}
