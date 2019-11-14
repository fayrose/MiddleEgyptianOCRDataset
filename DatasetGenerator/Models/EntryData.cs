using BitMiracle.Docotic.Pdf;
using DatasetGenerator.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace DatasetGenerator
{
    public class EntryData
    {
        public LineCoordinates Coordinates { get; internal set; }
        public string[] GardinerSigns { get; internal set; }
        public PdfRectangle[] ImageBounds { get; internal set; }
        public PdfPaintedImage[] Images { get; internal set; }
        public PdfPage EntryPdf { get; internal set; }
        public int EntryIndexInFile { get; internal set; }
        public double[] WordBounds { get; internal set; }
        public GlyphBlock[] GlyphBlocks{ get; internal set; }
        //public Bitmap EntryImage { get; internal set; }
    }
}
