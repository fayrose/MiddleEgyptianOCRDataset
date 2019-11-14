using BitMiracle.Docotic.Pdf;
using DatasetGenerator.Models;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Drawing;
using System.Text;

namespace DatasetGenerator
{
    public class EntryData
    {
        public LineCoordinates Coordinates { get; internal set; }
        public ImmutableArray<string> GardinerSigns { get; internal set; }
        public ImmutableArray<PdfPaintedImage> Images { get; internal set; }
        public PdfPage EntryPdf { get; internal set; }
        public int EntryIndexInFile { get; internal set; }
        public ImmutableArray<double> WordBounds { get; internal set; }
        public ImmutableArray<GlyphBlock> GlyphBlocks { get; internal set; }
        //public Bitmap EntryImage { get; internal set; }
    }
}
