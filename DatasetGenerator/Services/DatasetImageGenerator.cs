using System;
using System.Collections.Generic;
using System.Text;

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

        public void GetImagesFromPage(PageData page)
        {
            // Open Page File
            // Foreach entry in page
            // Get entryData's rightmost boundary + some buffer
            // Set cropbox's rightmost to be there (add param with rightmost boundary to ImageCopier)
            // Pass into ImageCopier
        }
    }
}
