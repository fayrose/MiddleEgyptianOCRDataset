using System.Collections.Generic;
using System.Runtime.Serialization;
using BitMiracle.Docotic.Pdf;

namespace DatasetGenerator.Models
{
    [DataContract]
    public class GlyphBlock
    {
        [DataMember]
        private List<PdfPaintedImage> Images { get; }
        public GlyphBlock(List<PdfPaintedImage> imgs)
        {
            Images = imgs;
        }

        internal void AddImage(PdfPaintedImage img)
        {
            Images.Add(img);
        }
    }
}
