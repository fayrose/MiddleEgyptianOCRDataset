using BitMiracle.Docotic.Pdf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;

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
            HashSet<string> duplicateTracker = new HashSet<string>();
            List<string> badIds = new List<string>();

            for (int i = 0; i < images.Count(); i++)
            {
                string glyphName = characterMap[images[i].Id];
                if (duplicateTracker.Add(glyphName))
                {
                    Console.WriteLine("Saving glyph " + glyphName + "...");
                    images[i].Save(Path.Combine(OutputDirectory, glyphName));
                }
                else
                {
                    badIds.Add(images[i].Id);
                }
            }
            File.WriteAllText(@"C:\Users\lfr2l\U of T\CSC420\project\dataset\badIds.json", JsonSerializer.Serialize(badIds));
            Console.WriteLine("Done!");
        }

        public void SaveCharacterFileFromPdf(string glyphId, string glyphName)
        {
            var images = GetCharacterSetFromPdf();
            var imageWithId = images.Where(x => x.Id == glyphId);
            var firstImageWithId = imageWithId.First();
            firstImageWithId.Save(Path.Combine(OutputDirectory, glyphName));
        }

        public PdfImage[] GetCharacterSetFromPdf()
        {
            return Pdf.GetImages().ToArray();
        }
    }
}
