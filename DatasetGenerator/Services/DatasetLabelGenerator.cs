using BitMiracle.Docotic.Pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Collections.Immutable;
using System.Linq;
using System.Diagnostics;
using DatasetGenerator.Models;

namespace DatasetGenerator
{
    class DatasetLabelGenerator
    {
        readonly string FileDirectory;

        public DatasetLabelGenerator(string datasetDirectory)
        {
            FileDirectory = datasetDirectory;
        }

        public DictionaryData ParseAllFiles()
        {
            string[] fileNames = Directory.GetFiles(FileDirectory);
            var PageDataArr = new PageData[fileNames.Length];

            foreach (var fileStr in fileNames)
            {
                int pageNum = GetPageNumberFromFileName(fileStr);
                Console.WriteLine("Processing page #" + pageNum.ToString() + "...");

                using var page = new PdfDocument(fileStr);

                var pageData = ParsePage(page, pageNum);

                pageData.FileLocation = fileStr;
                var metrics = RectangleCreator.GetMetricsOfSplit(page);
                for (int i = 0; i < pageData.EntryData.Length; i++)
                {
                    pageData.EntryData[i].Coordinates = metrics[i];
                }

                PageDataArr[pageNum - 1] = pageData;
            }
            return new DictionaryData() { Pages = PageDataArr.ToImmutableArray() } ;
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
                    GardinerSigns = gardinerList.Split(new string[] { " - ", "-" }, StringSplitOptions.RemoveEmptyEntries)
                                                .ToImmutableArray(),
                    EntryIndexInFile = i,
                };

                entryData.Images = GenerateBoundList(entryData.GardinerSigns, imageList, page.Pages[i], pageNum);
                
                Tuple<List<double>, List<GlyphBlock>> boundsAndImages;
                boundsAndImages = GenerateWordBounds(imageList);
                entryData.WordBounds = boundsAndImages.Item1.ToImmutableArray();
                entryData.GlyphBlocks = boundsAndImages.Item2.ToImmutableArray();

                entryList.Add(entryData);
            }

            return new PageData() { EntryData = entryList.ToArray(), PageNumber = pageNum };
        }

        private Tuple<List<double>, List<GlyphBlock>> GenerateWordBounds(PdfCollection<PdfPaintedImage> imageCollection)
        {

            List<PdfPaintedImage> imageList = imageCollection.ToList<PdfPaintedImage>();
            List<PdfPaintedImage> sortedImages = imageList.OrderBy(image => image.Bounds.X).ToList();
            List<double> boundaries = new List<double>();
            List<GlyphBlock> glyphBlocks = new List<GlyphBlock>();
            //boundaries.Add()
            GlyphBlock curGlyphBlock = null;
            PdfPaintedImage prevImage = null;
            double wordBound = 0;
            for (int i = 0; i < sortedImages.Count; i++)
            {
                PdfPaintedImage image = sortedImages[i];
                if (prevImage == null){
                    prevImage = image;
                    curGlyphBlock = new GlyphBlock(new List<PdfPaintedImage>());
                    curGlyphBlock.AddImage(image);
                    wordBound = prevImage.Bounds.X + prevImage.Bounds.Width;
                    continue;
                }
                if (wordBound <= image.Bounds.X || Math.Abs(image.Bounds.X - wordBound) < (wordBound * .00001))
                {
                    glyphBlocks.Add(curGlyphBlock);
                    curGlyphBlock = new GlyphBlock(new List<PdfPaintedImage>());
                    curGlyphBlock.AddImage(image);
                    double mid = image.Bounds.X - ((image.Bounds.X - wordBound)/2);
                    boundaries.Add(mid);
                    wordBound = image.Bounds.X + image.Bounds.Width;
                }
                else
                {
                    curGlyphBlock.AddImage(image);
                    wordBound = Math.Max(wordBound, image.Bounds.X + image.Bounds.Width);
                }
                prevImage = image;
            }
            glyphBlocks.Add(curGlyphBlock);
            boundaries.Add(prevImage.Bounds.X + prevImage.Bounds.Width);

            return new Tuple<List<double>, List<GlyphBlock>>(boundaries,glyphBlocks);
        }

        private ImmutableArray<PaintedPdfWrapper> GenerateBoundList(ImmutableArray<string> gardinerSigns, PdfCollection<PdfPaintedImage> imageList, PdfPage entry, int pageNum)
        {
            List<PaintedPdfWrapper> BoundList = new List<PaintedPdfWrapper>();
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
                    BoundList.Add(new PaintedPdfWrapper(image));
                }
            }
            return BoundList.ToImmutableArray();
        }

        private List<PaintedPdfWrapper> FixDifferentLengthLists(ImmutableArray<string> gardinerSigns, PdfCollection<PdfPaintedImage> imageList, PdfPage page, int pageNum)
        {
            List<PdfPaintedImage> fixedImageList = imageList.ToList().ConvertAll(image => image); //clone the array
            var coords = page.CropBox;
            var centerOfEntry = page.Height - (coords.Location.Y + (coords.Height / 2));
            var allYValuesOfImages = imageList.Select(x => x.Position.Y).ToArray();
            double stdDev = Stat.StdDev(allYValuesOfImages);

            var comparedYValues = allYValuesOfImages.Select(x => Math.Abs(x - centerOfEntry) / stdDev).ToList();
            
            while (fixedImageList.Count > gardinerSigns.Length)
            {
                var comparedMax = comparedYValues.Max();
                var maxIdx = comparedYValues.IndexOf(comparedMax);
                comparedYValues.RemoveAt(maxIdx);
                fixedImageList.RemoveAt(maxIdx);
            }
            return fixedImageList.Select( x => new PaintedPdfWrapper(x)).ToList();
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
