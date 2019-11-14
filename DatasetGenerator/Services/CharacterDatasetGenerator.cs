using BitMiracle.Docotic.Pdf;
using System.IO;
using System.Linq;

namespace DatasetGenerator
{
    class CharacterDatasetGenerator
    {
        readonly string PdfLocation;
        readonly string OutputDirectory;
        public CharacterDatasetGenerator(string inputLocation, string outputFolder)
        {
            PdfLocation = inputLocation;
            OutputDirectory = outputFolder;
        }
        public void GetCharacterFilesFromPdf()
        {
            var pdf = new PdfDocument(PdfLocation);
            var images = pdf.GetImages().ToArray();

            for (int i = 0; i < images.Count(); i++)
            {
                images[i].Save(Path.Combine(OutputDirectory, "char" + (i + 1).ToString()));
            }
        }
    }
}
