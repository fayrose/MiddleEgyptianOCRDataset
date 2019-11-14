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

        public void GetNamesFromSplitPdfs(PageData[] datasetData)
        {
            // Full Character Set
            var imageIds = GetCharacterSetFromPdf().Select(x => x.Id);

            foreach (var id in imageIds)
            {
                var entriesContainingCharacter = datasetData.SelectMany(x => x.EntryData)
                                                            .Where(x => x.GlyphImages.Select(y => y.Image.Id).Contains(id));
                var gardinersOfEntries = entriesContainingCharacter.Select(x => x.GardinerSigns).ToArray();
                var intersected = gardinersOfEntries[0];
                for (int i = 1; i < gardinersOfEntries.Length; i++)
                {
                    intersected = intersected.Intersect(gardinersOfEntries[i]).ToArray();
                }
                // Choose best of intersected
                // Special case where entriesContainingCharacter.length == 1 and you want to find the only gardiner not occurring elsewhere
            }
            
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
