using BitMiracle.Docotic.Pdf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DatasetGenerator
{
    class RectangleCreator
    {
        public static LineCoordinates[] GetLineMetrics(PdfDocument Pdf, int pageNumber)
        {
            var pageHeight = (int)Pdf.Pages[pageNumber].Height;
            var lineYs = GetLineYCoordinates(Pdf.Pages[pageNumber], pageHeight);
            var lineDiffs = GetVerticalBuffers(lineYs, pageHeight);
            return GetTopAndBottomAroundLines(lineYs, lineDiffs).ToArray();
        }

        public static LineCoordinates[] GetMetricsOfSplit(PdfDocument Pdf)
        {
            List<LineCoordinates> lineList = new List<LineCoordinates>();
            foreach (var entry in Pdf.Pages)
            {
                var cropBox = entry.CropBox;
                lineList.Add(new LineCoordinates()
                {
                    LineBottom = cropBox.Bottom,
                    LineTop = cropBox.Top,
                    LineHeight = cropBox.Height
                });
            }
            return lineList.ToArray();
        }


        private static List<LineCoordinates> GetTopAndBottomAroundLines(List<double> lineYs, List<double> lineDiffs)
        {
            List<LineCoordinates> verticalBuffers = new List<LineCoordinates>();

            for (int i = 0; i < lineYs.Count; i++)
            {
                var topDiff = lineDiffs[i];
                var bottomDiff = lineDiffs[i + 1];

                var bottomBuff = bottomDiff * (0.75);
                var topBuff = topDiff * (0.5);

                LineCoordinates coords = new LineCoordinates
                {
                    TextHeight = lineYs[i],
                    LineBottom = lineYs[i] - bottomBuff,
                    LineTop = lineYs[i] + topBuff,
                    LineHeight = topBuff + bottomBuff
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
        private static List<double> GetVerticalBuffers(List<double> lineYs, int pageHeight)
        {
            List<double> extended = new List<double>() { pageHeight };
            List<double> differences = new List<double>();
            extended.AddRange(lineYs);
            extended.Add(0);

            for (int i = 0; i < extended.Count - 1; i++)
            {
                differences.Add(extended[i] - extended[i + 1]);
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
        private static List<double> GetLineYCoordinates(PdfPage page, int pageHeight)
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
                        int potentialKey = pageHeight - (y_int + i);
                        if (dic.ContainsKey(potentialKey))
                        {
                            dic[potentialKey].Add(pageHeight - y_val);
                            found = true;
                            break;
                        }
                    }

                    if (!found)
                    {
                        dic.Add(pageHeight - y_int, new List<double>() { pageHeight - y_val });
                    }
                }
            }
            return dic.Select(x => x.Value.Average()).ToList();
        }

    }
}
