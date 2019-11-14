using BitMiracle.Docotic.Pdf;
using System.Collections.Generic;
using System.Diagnostics;
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

        public Dictionary<string, string> GetNamesFromSplitPdfs(PageData[] datasetData)
        {
            // Full Character Set
            var imageIds = GetCharacterSetFromPdf().Select(x => x.Id);
            var entries = datasetData.SelectMany(x => x.EntryData);

            Dictionary<string, string> ImageIdToGardinerValue = new Dictionary<string, string>();
            foreach (var id in imageIds)
            {
                var gardiner = GetNameFromEntries(entries, id);
                ImageIdToGardinerValue.Add(id, gardiner);
            }
            return ImageIdToGardinerValue;
        }

        private string GetNameFromEntries(IEnumerable<EntryData> entries, string id)
        {
            string Gardiner;

            var entriesContainingCharacter = entries.Where(x => x.Images.Select(y => y.Image.Id).Contains(id));
            if (entriesContainingCharacter.Count() > 1)
            {
                // Intersect the gardiner ID lists of all entries containing this image ID
                // This should hopefully produce a singular gardiner value.
                var gardinersOfEntries = entriesContainingCharacter.Select(x => x.GardinerSigns).ToArray();
                IEnumerable<string> intersected = gardinersOfEntries[0].ToArray();
                for (int i = 1; i < gardinersOfEntries.Length; i++)
                {
                    intersected = intersected.Intersect(gardinersOfEntries[i]);
                }
                Debug.Assert(intersected.Count() == 1);
                Gardiner = intersected.Single();
                // Choose best of intersected
            }
            // Special case where entriesContainingCharacter.length == 1 and you want to find the only gardiner not occurring elsewhere
            else if (entriesContainingCharacter.Count() == 1)
            {
                var candidates = entriesContainingCharacter.Single().GardinerSigns;
                var closeCandidates = new List<string>();
                foreach (var sign in candidates)
                {
                    var entriesContainingSign = entries.Where(x => x.GardinerSigns.Contains(sign));
                    if (entriesContainingSign.Count() == 1)
                    {
                        closeCandidates.Add(sign);
                    }
                }
                Debug.Assert(closeCandidates.Count() == 1);
                Gardiner = closeCandidates.Single();
            }
            else // Image never occurs in any entry of PDF
            {
                throw new System.Exception("Should never happen");
            }
            return Gardiner;
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
