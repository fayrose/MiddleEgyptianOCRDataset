using BitMiracle.Docotic.Pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using System.Diagnostics;

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
                    pageData.FileLocation = fileStr;

                    var metrics = RectangleCreator.GetMetricsOfSplit(page);
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
            int indexOfPeriod = fileName.IndexOf("Aug");
            // Each file prepended with "page" so skip to 4th char for substring
            int endOfPagePrefix = fileName.IndexOf("page") + 4;
            int length = indexOfPeriod - endOfPagePrefix;
            var substr = fileName.Substring(endOfPagePrefix, length);
            return int.Parse(substr);
        }

        private PageData ParsePage(PdfDocument page, int pageNum)
        {
            List<EntryData> entryList = new List<EntryData>();

            for (int i = 0; i < page.Pages.Count; i++)
            {
                var gardinerList = GetGardinersOnPage(page.Pages[i]);
                var imageList = page.Pages[i].GetPaintedImages();
                var entryData = new EntryData()
                {
                    EntryPdf = page.Pages[i],
                    GardinerSigns = gardinerList.Split(new string[] { " - ", "-" }, StringSplitOptions.RemoveEmptyEntries),
                    EntryIndexInFile = i,
                };
                
                entryData.ImageBounds = GenerateBoundList(entryData.GardinerSigns, imageList);
                entryList.Add(entryData);
            }

            return new PageData() { EntryData = entryList.ToArray(), PageNumber = pageNum };
        }

        private PdfRectangle[] GenerateBoundList(string[] gardinerSigns, PdfCollection<PdfPaintedImage> imageList)
        {
            List<PdfRectangle> BoundList = new List<PdfRectangle>();
            if (gardinerSigns.Length != imageList.Count)
            {
                BoundList = FixDifferentLengthLists(gardinerSigns, imageList);
                Debug.Assert(BoundList.Count == gardinerSigns.Length);
            }
            else
            {
                for (int j = 0; j < gardinerSigns.Length; j++)
                {
                    var image = imageList.GetAt(j);
                    BoundList.Add(image.Bounds);
                }
            }
            return BoundList.ToArray();
        }

        private List<PdfRectangle> FixDifferentLengthLists(string[] gardinerSigns, PdfCollection<PdfPaintedImage> imageList)
        {
            /*var avgY = imageList.Select(x => x.Position.Y).Average();
                for (int k = 0; k < imageList.Count; k++)
                {
                    double yPos = imageList.GetAt(k).Position.Y;
                    if (yPos > avgY - 13 && yPos < avgY + 16)
                    {
                        BoundList.Add(imageList.GetAt(k).Bounds);
                    }
                }*/ // TODO
            return null;
        }

        private string GetGardinersOnPage(PdfPage page)
        {
            char[] startOfGardinerMarkers = new char[] { ']', '}', ')', '?' };

            var lineArr = page.GetText()
                              .Split("\r\n", StringSplitOptions.RemoveEmptyEntries);
            
            var text = lineArr[0];
            int closingBraceIdx = text.LastIndexOfAny(startOfGardinerMarkers);
            int multispaceIdx = text.LastIndexOf("   ");
            int doubleIdx = text.LastIndexOf("  ");
            
            // Get max of three values
            int max = Math.Max(Math.Max(closingBraceIdx + 1, multispaceIdx), doubleIdx);

            // Take substring of after
            return text.Substring(max).Trim();
        }
    }
}
