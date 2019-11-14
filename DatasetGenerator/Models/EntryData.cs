using BitMiracle.Docotic.Pdf;
using DatasetGenerator.Models;
using System.Collections.Immutable;
using System.Runtime.Serialization;

namespace DatasetGenerator
{
    [DataContract]
    public class EntryData
    {
        [DataMember]
        public LineCoordinates Coordinates { get; internal set; }
        [DataMember]
        public ImmutableArray<string> GardinerSigns { get; internal set; }
        [DataMember]
        public ImmutableArray<PdfPaintedImage> Images { get; internal set; }
        [IgnoreDataMember]
        public PdfPage EntryPdf { get; internal set; }
        [DataMember]
        public int EntryIndexInFile { get; internal set; }
        [DataMember]
        public ImmutableArray<double> WordBounds { get; internal set; }
        [DataMember]
        public ImmutableArray<GlyphBlock> GlyphBlocks { get; internal set; }
        //public Bitmap EntryImage { get; internal set; }
    }
}
