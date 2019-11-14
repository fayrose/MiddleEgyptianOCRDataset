using BitMiracle.Docotic.Pdf;
using System.IO;
using System.Linq;

namespace DatasetGenerator
{
    class CharacterDatasetGenerator
    {
        readonly string PdfLocation;
        readonly string OutputDirectory;
        readonly PdfDocument Pdf;
        public CharacterDatasetGenerator(string inputLocation, string outputFolder)
        {
            PdfLocation = inputLocation;
            OutputDirectory = outputFolder;
            Pdf = new PdfDocument(PdfLocation);
        }
        public void SaveCharacterFilesFromPdf()
        {
            var images = GetCharacterSetFromPdf();

            for (int i = 0; i < images.Count(); i++)
            {
                images[i].Save(Path.Combine(OutputDirectory, "char" + (i + 1).ToString()));
            }
        }

        public PdfImage[] GetCharacterSetFromPdf()
        {
            return Pdf.GetImages().ToArray();
        }

        public void GetNamesFromSplitPdfs(string dataLocation)
        {
            // Full Character Set
            var images = GetCharacterSetFromPdf().Select(x => x.Id);


            string[] fileNames = Directory.GetFiles(dataLocation);
            foreach (var fileStr in fileNames)
            {
                using (var page = new PdfDocument(fileStr))
                {
                    // Ids from split pages
                    // Can see from intersection size that IDs are identical between
                    // untouched and split PDF. This is fantastic.
                    var splitImages = page.GetImages().Select(x => x.Id);
                    var intersection = images.Intersect(splitImages);

                    /* Options: Augment label generator to take list of PdfPaintedImage, not the PdfPaintedImage.Bounds (type=PdfRectangle)
                     *          and then pass generated PageData / EntryData into this function. Get the painted image ID and see which IDs 
                     *          are always present with certain Gardiner glyphs. Note that page.Pages[0].GetPaintedImages() will not necessarily
                     *          be in the order that the gardiner glyphs are. 
                     *          
                     *          Therefore find all pages that contain a certain image ID, and compare to glyphs found in all of those pages to obtain match.
                    */
                }
            }
        }
    }
}
