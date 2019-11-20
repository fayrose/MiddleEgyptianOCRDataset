using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using BitMiracle.Docotic.Pdf;

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

                var entry = Data.Pages[pageNum - 1];
                var pdf = new PdfDocument(file);
                for (int i = 0; i < pdf.Pages.Count; i++)
                {
                    string outFileName = pageNum.ToString() + i.ToString();

                    GetImagesFromPage(pdf.Pages[i],entry.EntryData[i], outFileName);
                }
            }
        }

        public void GetImagesFromPage(PdfPage page, EntryData entryData, string outFileName)
        {
            ImageCopier imageCopier = new ImageCopier(page);
            double boundary = entryData.WordBounds.Last() + 5;

            imageCopier.GenerateLineImage(OutputLocation, boundary, outFileName);


            // Open Page File
            // Foreach entry in page
            // Get entryData's rightmost boundary + some buffer
            // Set cropbox's rightmost to be there (add param with rightmost boundary to ImageCopier)
            // Pass into ImageCopier
        }
    }
}
