using BitMiracle.Docotic.Pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DatasetGenerator
{
    class DatasetGenerator
    {
        private PdfDocument Pdf;
        private string OutputLocation;
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
                var lineYs = ParsePage(Pdf.Pages[i]);
                var lineDiffs = GetVerticalBuffers(lineYs);
                var buffers = GetTopAndBottomAroundLines(lineYs, lineDiffs);

                PdfObjectCopier copier = new PdfObjectCopier(Pdf, i, buffers, OutputLocation);
                copier.GenerateCroppedPage();
            }
        }

        private List<LineCoordinates> GetTopAndBottomAroundLines(List<double> lineYs, List<double> lineDiffs)
        {
            List<LineCoordinates> verticalBuffers = new List<LineCoordinates>();

            for (int i = 0; i < lineYs.Count; i++)
            {
                var bottomDiff = lineDiffs[i];
                var topDiff = lineDiffs[i + 1];

                var bottomBuff = bottomDiff * (1.0 / 3);
                var topBuff = topDiff * (2.0 / 3);

                LineCoordinates coords = new LineCoordinates
                {
                    LineTop = lineYs[i] + topDiff,
                    LineBottom = lineYs[i] - bottomBuff,
                };

                verticalBuffers.Add(coords);
            }
            return verticalBuffers;
        }

        /// <summary>
        /// Gets a list of all of the sizes between lines.
        /// </summary>
        /// <param name="lineYs">List representing y-coordinates of lines in a PDF.</param>
        /// <returns>Differences between each line, as well as the page number and the last line
        /// and the difference between the first line and y-coordinate 0.</returns>
        private List<double> GetVerticalBuffers(List<double> lineYs)
        {
            List<double> extended = new List<double>() { 0 };
            List<double> differences = new List<double>();
            extended.AddRange(lineYs);
            extended.Add(583);

            for (int i = 0; i < extended.Count - 1; i++)
            {
                differences.Add(extended[i + 1] - extended[i]);
            }
            return differences;
        }

        /// <summary>
        /// Given a PDF page, we want to bucket the y-values of all text
        /// occurring in the pdf into lines and return the average line coordinate
        /// in each bucket.
        /// </summary>
        /// <param name="page">Page of the pdf being examined</param>
        /// <returns>Average y-coordinate of each line.</returns>
        private List<double> ParsePage(PdfPage page)
        {
            var dic = new Dictionary<int, List<double>>();
            foreach (PdfTextData textData in page.Canvas.GetTextData())
            {
                var y_val = textData.Position.Y;
                var y_int = (int)y_val;
                
                // Ensure page number isn't treated as a line
                if (y_int != 583)
                {
                    // See if text exist within a 4-px radius of current location
                    bool found = false;
                    for (int i = -2; i <= 2; i++)
                    {
                        if (dic.ContainsKey(y_int + i))
                        {
                            dic[y_int + i].Add(y_val);
                            found = true;
                            break;
                        }
                    }

                    if (!found)
                    {
                        dic.Add(y_int, new List<double>() { y_val });
                    }
                }
            }
            return dic.Select(x => x.Value.Average()).ToList();
        }

        static void Main(string[] args)
        {
            BitMiracle.Docotic.LicenseManager.AddLicenseData("6EOI3-DXN35-5M8G6-8QGW9-Y18Z5");

            var inLocation = @"C:\Users\lfr2l\U of T\NML340\VYGUS_Dictionary_2018.pdf";
            var outLocation = @"C:\Users\lfr2l\U of T\CSC420\project\dataset\output\";
            var dg = new DatasetGenerator(inLocation, outLocation);
            dg.ParsePdf();
        }
    }
}
