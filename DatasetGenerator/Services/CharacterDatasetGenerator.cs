using BitMiracle.Docotic.Pdf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace DatasetGenerator
{
    class CharacterDatasetGenerator
    {
        readonly string OutputDirectory;
        readonly PdfDocument Pdf;
        public CharacterDatasetGenerator(string inputLocation, string outputFolder)
        {
            OutputDirectory = outputFolder;
            Pdf = new PdfDocument(inputLocation);
        }
        public void SaveCharacterFilesFromPdf(Dictionary<string, string> characterMap)
        {
            var images = GetCharacterSetFromPdf();

            for (int i = 0; i < images.Count(); i++)
            {
                string glyphName = characterMap[images[i].Id];
                Console.WriteLine("Saving glyph " + glyphName + "...");
                images[i].Save(Path.Combine(OutputDirectory, glyphName));
            }
        }

        public PdfImage[] GetCharacterSetFromPdf()
        {
            return Pdf.GetImages().ToArray();
        }
    }
}
