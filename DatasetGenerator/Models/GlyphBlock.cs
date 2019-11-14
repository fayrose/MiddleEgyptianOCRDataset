using System;
using System.Collections.Generic;
using BitMiracle.Docotic.Pdf;

namespace DatasetGenerator.Models
{
    public class GlyphBlock
    {
        private List<PdfPaintedImage> Images;
        public GlyphBlock(List<PdfPaintedImage> imgs)
        {
            Images = imgs;
        }

        public void addImage(PdfPaintedImage img)
        {
            Images.Add(img);
        }

        public List<PdfPaintedImage> getImages()
        {
            return Images;
        }
    }
}
