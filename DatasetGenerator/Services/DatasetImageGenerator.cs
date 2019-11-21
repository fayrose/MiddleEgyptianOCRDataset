using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using BitMiracle.Docotic.Pdf;
using Ghostscript.NET.Rasterizer;

namespace DatasetGenerator.Services
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
                // TODO : Get the pagenum from file name
                // Get data.Pages[pagenum - 1]
                int pageNum = DatasetLabelGenerator.GetPageNumberFromFileName(file);


                var rasterizer = new GhostscriptRasterizer();
                int desired_x_dpi = 96;
                int desired_y_dpi = 96;
                rasterizer.Open(file);

                //i.
                //rasterizer.
                for (var pageNumber = 1; pageNumber <= rasterizer.PageCount; pageNumber++)
                {

                    var img = rasterizer.GetPage(desired_x_dpi, desired_y_dpi, pageNumber);

                    for(int i = 0; i < Data.Pages[pageNumber].EntryData.Length; i++)
                    {
                        EntryData entryData = Data.Pages[pageNumber].EntryData[i];
                        double xBoundary = entryData.WordBounds.Last() + 5;

                        var pageFilePath = Path.Combine(OutputLocation, string.Format("Page-{0}-Entry-{1}.png", pageNumber,i));
                        var imgRec = new RectangleF((float)0.0, (float)entryData.Coordinates.LineBottom, (float)xBoundary, (float)entryData.Coordinates.LineHeight);
                        Bitmap target = new Bitmap((int)imgRec.Width, (int)imgRec.Height);

                        using (Graphics g = Graphics.FromImage(target))
                        {
                            g.DrawImage(img, new Rectangle(0, 0, target.Width, target.Height),
                                             imgRec,
                                             GraphicsUnit.Pixel);
                        }
                        img.Save(pageFilePath, ImageFormat.Png);
                    }
                }

            }
        }

        public void GetImagesFromPage(PdfPage page, EntryData entryData, string outFileName)
        {
            //ImageCopier imageCopier = new ImageCopier(page);


            double boundary = entryData.WordBounds.Last() + 5;

            //imageCopier.GenerateLineImage(OutputLocation, boundary, outFileName);


            // Open Page File
            // Foreach entry in page
            // Get entryData's rightmost boundary + some buffer
            // Set cropbox's rightmost to be there (add param with rightmost boundary to ImageCopier)
            // Pass into ImageCopier
        }
    }
}
