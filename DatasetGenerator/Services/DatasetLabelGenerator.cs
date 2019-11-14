﻿using BitMiracle.Docotic.Pdf;
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
                
                entryData.ImageBounds = GenerateBoundList(entryData.GardinerSigns, imageList, page.Pages[i], pageNum);
                entryList.Add(entryData);
            }

            return new PageData() { EntryData = entryList.ToArray(), PageNumber = pageNum };
        }

        private PdfRectangle[] GenerateBoundList(string[] gardinerSigns, PdfCollection<PdfPaintedImage> imageList, PdfPage entry, int pageNum)
        {
            List<PdfRectangle> BoundList = new List<PdfRectangle>();
            if (gardinerSigns.Length != imageList.Count)
            {
                BoundList = FixDifferentLengthLists(gardinerSigns, imageList, entry, pageNum);
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

        private List<PdfRectangle> FixDifferentLengthLists(string[] gardinerSigns, PdfCollection<PdfPaintedImage> imageList, PdfPage page, int pageNum)
        {
            List<PdfRectangle> boundList = imageList.Select(x => x.Bounds).ToList();
            var coords = page.CropBox;
            var centerOfEntry = page.Height - (coords.Location.Y + (coords.Height / 2));
            var allYValuesOfImages = imageList.Select(x => x.Position.Y).ToArray();
            double stdDev = Stat.StdDev(allYValuesOfImages);

            var comparedYValues = allYValuesOfImages.Select(x => Math.Abs(x - centerOfEntry) / stdDev).ToList();
            
            while (boundList.Count > gardinerSigns.Length)
            {
                var comparedMax = comparedYValues.Max();
                var maxIdx = comparedYValues.IndexOf(comparedMax);
                comparedYValues.RemoveAt(maxIdx);
                boundList.RemoveAt(maxIdx);
            }
            return boundList;
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
