using BitMiracle.Docotic.Pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DatasetGenerator
{
    class DatasetLabelGenerator
    {
        readonly string FileDirectory;

        public DatasetLabelGenerator(string datasetDirectory)
        {
            FileDirectory = datasetDirectory;
        }

        public PageData[] ParseAllFiles()
        {
            var allEntryBounds = new List<PageData>();
            string[] fileNames = Directory.GetFiles(FileDirectory);
            foreach (var fileStr in fileNames)
            {
                int pageNum = GetPageNumberFromFileName(fileStr);
                Console.WriteLine("Processing page #" + pageNum.ToString() + "...");

                using (var page = new PdfDocument(fileStr))
                {
                    var pageData = ParsePage(page, pageNum);

                    var metrics = RectangleCreator.GetLineMetrics(page, pageNum);
                    for (int i = 0; i < pageData.EntryData.Length; i++)
                    {
                        pageData.EntryData[i].Coordinates = metrics[i];
                    }

                    allEntryBounds.Add(pageData);
                }
            }
            return allEntryBounds.ToArray();
        }

        /// <summary>
        /// Gets the 1-indexed number of the page given a dataset file's name.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private int GetPageNumberFromFileName(string fileName)
        {   
            int indexOfPeriod = fileName.IndexOf('.');
            // Each file prepended with "page" so skip to 4th char for substring
            int endOfPagePrefix = fileName.IndexOf("page") + 4;
            int length = indexOfPeriod - endOfPagePrefix;
            var substr = fileName.Substring(endOfPagePrefix, length);
            return int.Parse(substr);
        }

        private PageData ParsePage(PdfDocument page, int pageNum)
        {
            int imageIndex = 0;
            List<EntryData> entryList = new List<EntryData>();
            // Only need to pass in first entry, as all entries contain all text & images
            var gardinerList = GetGardinersOnPage(page.Pages[0]);
            var imageList = page.Pages[0].GetPaintedImages();

            for (int i = 0; i < page.Pages.Count; i++)
            {
                var entryData = new EntryData()
                {
                    EntryPdf = page.Pages[i],
                    GardinerSigns = gardinerList[i].Split(" - "),
                    EntryIndexInFile = i,
                };

                List<PdfRectangle> BoundList = new List<PdfRectangle>();
                for (int j = 0; j < entryData.GardinerSigns.Length; j++)
                {
                    var idxToGet = imageIndex + j;
                    var image = imageList.GetAt(idxToGet);
                    BoundList.Add(image.Bounds);
                }
                entryData.ImageBounds = BoundList.ToArray();
                entryList.Add(entryData);
                imageIndex += entryData.GardinerSigns.Length;
            }

            return new PageData() { EntryData = entryList.ToArray(), PageNumber = pageNum, FileLocation = "page" + pageNum.ToString() + ".pdf" };
        }

        private List<string> GetGardinersOnPage(PdfPage page)
        {
            List<string> gardinerList = new List<string>();
            char[] startOfGardinerMarkers = new char[] { ']', '}', ')', '?' };

            var lineArr = page.GetText()
                              .Split("\r\n", StringSplitOptions.RemoveEmptyEntries);

            // Don't iterate over the last line, as it's the page #
            for (int i = 0; i < lineArr.Length; i++)
            {
                string gardiner;
                int closingBraceIdx = lineArr[i].LastIndexOfAny(startOfGardinerMarkers);
                int multispaceIdx = lineArr[i].LastIndexOf("   ");
                int doubleIdx = lineArr[i].LastIndexOf("  ");

                if (i == lineArr.Length - 1 && int.TryParse(lineArr[i], out _))
                {
                    break;
                }
                else if (i == lineArr.Length - 1)
                {
                    int pageNumIdx = Math.Max(multispaceIdx, doubleIdx);
                    lineArr[i] = lineArr[i].Substring(0, pageNumIdx);
                    multispaceIdx = lineArr[i].LastIndexOf("   ");
                    doubleIdx = lineArr[i].LastIndexOf("  ");
                }

                // Get max of three values
                int max = Math.Max(Math.Max(closingBraceIdx + 1, multispaceIdx), doubleIdx);

                // Take substring of after
                gardiner = lineArr[i].Substring(max).Trim();
                
                gardinerList.Add(gardiner.Trim());
            }
            return gardinerList;
        }
    }
}
