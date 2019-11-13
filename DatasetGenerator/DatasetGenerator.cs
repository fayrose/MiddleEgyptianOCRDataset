using BitMiracle.Docotic.Pdf;
using System.Collections.Generic;
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

        static void Main(string[] args)
        {
            BitMiracle.Docotic.LicenseManager.AddLicenseData("6EOI3-DXN35-5M8G6-8QGW9-Y18Z5");
            
            var inLocation = @"C:\Users\lfr2l\U of T\NML340\VYGUS_Dictionary_2018.pdf";
            var outLocation = @"C:\Users\lfr2l\U of T\CSC420\project\dataset\aug_output\";
            
            //DatasetGenerator dg = new DatasetGenerator(inLocation, outLocation);
            //dg.ParsePdf();

            var dlg = new DatasetLabelGenerator(outLocation);
            dlg.ParseAllFiles();
        }
    }
}
