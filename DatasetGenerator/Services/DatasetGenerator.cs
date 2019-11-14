using BitMiracle.Docotic.Pdf;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DatasetGenerator
{
    class DatasetGenerator
    {
        private PdfDocument Pdf;
        private readonly string OutputLocation;
        private readonly Dictionary<int, Dictionary<int, List<double>>> PagesToLineData;

        public DatasetGenerator(string pdfLocation, string outputLocation)
        {
            PagesToLineData = new Dictionary<int, Dictionary<int, List<double>>>();
            Pdf = new PdfDocument(pdfLocation);
            OutputLocation = outputLocation;
        }

        public void ParsePdf()
        {
            for (int i = 0; i < Pdf.Pages.Count; i++)
            {
                ParsePage(i);   
            }
        }

        public void ParsePage(int pageIdx)
        {
            var buffers = RectangleCreator.GetLineMetrics(Pdf, pageIdx);

            PdfObjectCopier copier = new PdfObjectCopier(Pdf, pageIdx, buffers, OutputLocation);
            copier.GenerateCroppedPage();
        }
    }
}
