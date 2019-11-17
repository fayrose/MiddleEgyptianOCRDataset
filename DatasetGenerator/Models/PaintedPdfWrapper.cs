using BitMiracle.Docotic.Pdf;
using System;
using System.Collections.Generic;
using System.Text;

namespace DatasetGenerator.Models
{
    public class PaintedPdfWrapper
    {
        internal string Id { get; private set; }
        public PdfPaintedImage Image { get; }

        public PaintedPdfWrapper(PdfPaintedImage image)
        {
            Image = image;
        }

        public void AddId(string id)
        {
            if (String.IsNullOrEmpty(Id))
            {
                Id = id;
            }
        }

    }
}
