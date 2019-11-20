using BitMiracle.Docotic.Pdf;
//using DatasetGenerator.Models;
using System.Collections.Immutable;
using System.Runtime.Serialization;

namespace DatasetGenerator
{
    public class EntryData
    {
        public LineCoordinates Coordinates { get; set; }
        public ImmutableArray<string> GardinerSigns { get; set; }
        public ImmutableArray<ImageData> Images { get; set; }
        public PdfPage EntryPdf { get; set; }
        public int EntryIndexInFile { get; set; }
        public ImmutableArray<double> WordBounds { get; set; }
        public ImmutableArray<GlyphBlock> GlyphBlocks { get; set; }
        
        public string ManuelDeCodage { get; set; }
        //public Bitmap EntryImage { get; internal set; }

        public EntryData()
        {

        }
    }
}
