using BitMiracle.Docotic.Pdf;
using System;
using System.IO;
using System.Linq;

namespace DatasetGenerator
{
    class DatasetImageGenerator
    {
        DictionaryData Data;
        string SplitVygusLocation;
        string OutputLocation;
        public DatasetImageGenerator(string pathToSplitVygusFolder, string pathToOutputFolder, DictionaryData data)
        {
            Data = data;
            SplitVygusLocation = pathToSplitVygusFolder;
            OutputLocation = pathToOutputFolder;
        }

        public void GetImagesFromFolder()
        {
            foreach (var file in Directory.EnumerateFiles(SplitVygusLocation))
            {
                int pageNum = DatasetLabelGenerator.GetPageNumberFromFileName(file);
                Console.WriteLine(String.Format("Printing page {0}...", pageNum));
                using PdfDocument doc = new PdfDocument(file);

                PdfDrawOptions options = PdfDrawOptions.Create();
                options.BackgroundColor = new PdfRgbColor(255, 255, 255);
                options.Compression = ImageCompressionOptions.CreateBitonalTiff();
                options.HorizontalResolution = 1000;
                options.VerticalResolution = 1000;
                
                for (int i = 0; i < doc.PageCount; i++)
                {
                    GetImageFromEntry(pageNum, i, options, doc);
                }
            }
        }

        private void GetImageFromEntry(int pageNum, int i, PdfDrawOptions options, PdfDocument doc)
        {
            var outFileName = String.Format("page{0}_entry{1}.tiff", pageNum, i);
            var entry = Data.Pages[pageNum - 1].EntryData[i];
            var imageWithLargestRight = entry.Images.Max(x => x.X + x.Width);
            double clippedRight = Math.Max(entry.WordBounds[entry.WordBounds.Length - 1], imageWithLargestRight) + 15;
            doc.Pages[i].CropBox = new PdfBox(doc.Pages[i].CropBox.Left, doc.Pages[i].CropBox.Bottom, clippedRight, doc.Pages[i].CropBox.Top);
            doc.Pages[i].Save(Path.Combine(OutputLocation, outFileName), options);
        }
    }
}
