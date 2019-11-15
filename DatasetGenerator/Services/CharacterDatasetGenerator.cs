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

        private void MapFullVygusIdsToSplitVygusIds(DictionaryData data)
        {
            var paintedImagesPerPage = Pdf.Pages.Select(x => x.GetPaintedImages()).ToArray();
            var fullVygusIds = paintedImagesPerPage.SelectMany(x => x) // Flattens before getting Ids
                                                   .Select(x => x.Image.Id);
            var fullVygusCounts = GenerateCountDictionary(fullVygusIds);

            for (int i = 0; i < data.Pages.Length; i++)
            {
                MapPictureIdsForPage(paintedImagesPerPage[i], data.Pages[i]);
            }
        }

        public Dictionary<string, string> GetNamesFromSplitPdfs(DictionaryData datasetData)
        {
            MapFullVygusIdsToSplitVygusIds(datasetData);

            // Get count of each image ID
            var entries = datasetData.Pages.SelectMany(x => x.EntryData);
            var imageIds = entries.SelectMany(x => x.Images.Select(y => y.Id));
            var entryIdCounts = GenerateCountDictionary(imageIds);

            // Get count of each gardiner ID
            var gardinerIds = entries.SelectMany(x => x.GardinerSigns);
            var gardinerCounts = GenerateCountDictionary(gardinerIds);

            Dictionary<string, string> ImageIdToGardinerValue = new Dictionary<string, string>();
            foreach (var id in entryIdCounts.Keys)
            {
                var gardiner = GetNameFromEntries(entries, id, entryIdCounts, gardinerCounts);
                ImageIdToGardinerValue.Add(id, gardiner);
            }
            return ImageIdToGardinerValue;
        }

        private string GetNameFromEntries(IEnumerable<EntryData> entries,
                                          string id,
                                          Dictionary<string, int> entryIdCounts,
                                          Dictionary<string, int> gardinerCounts)
        {
            var idCount = entryIdCounts[id];
            var gardinersWithIdCount = gardinerCounts.Where(x => x.Value == idCount).Select(x => x.Key);
            var numberOfMatches = gardinersWithIdCount.Count();
            if (numberOfMatches == 1)
            {
                return gardinersWithIdCount.Single();
            }
            else if (numberOfMatches > 1)
            {
                var closeCandidates = new Dictionary<string, int>();
                foreach (var candidate in gardinersWithIdCount)
                {
                    var entriesWithCandidate = entries.Where(x => x.GardinerSigns.Contains(candidate));
                    var entriesWithId = entries.Where(x => x.Images.Select(y => y.Id).Contains(id));
                    var intersection = entriesWithCandidate.Intersect(entriesWithId).ToList();
                    if (intersection.Count > 1)
                    {
                        closeCandidates.Add(candidate, intersection.Count);
                    }
                }
                if (closeCandidates.Count == 1)
                {
                    return closeCandidates.Single().Key;
                }
                else if (closeCandidates.Count > 1)
                {
                    var maxVal = closeCandidates.Values.Max();
                    return closeCandidates.Where(x => x.Value == maxVal).First().Key;
                }
                else
                {
                    var entriesWithId = entries.Where(x => x.Images.Select(y => y.Id).Contains(id));
                    var gardinersWithId = entriesWithId.SelectMany(x => x.GardinerSigns).ToArray();
                    if (gardinersWithId.Length > 1)
                    {
                        var countDict = GenerateCountDictionary(gardinersWithId);
                        var maxVal = countDict.Values.Max();
                        var bestCandidate = countDict.Where(x => x.Value == maxVal).Select(x => x.Key).ToArray();
                        if (bestCandidate.Length == 1)
                        {
                            return bestCandidate.Single();
                        }
                        else
                        {
                            // Return candidate with gardinerCounts[bestCandidate] closest to gardinersWithId.Length
                            var bestCandidateOccurrances = bestCandidate.Select(x => gardinerCounts[x]);
                            var candidateOccurrancesDiffFromIdOccurances = bestCandidateOccurrances.Select(x => Math.Abs(entriesWithId.Count() - x));
                            var lowestError = candidateOccurrancesDiffFromIdOccurances.Min();
                            return bestCandidate.Where(x => Math.Abs(entriesWithId.Count() - gardinerCounts[x]) == lowestError).First();
                        }
                    }
                    else if (gardinersWithId.Length == 1)
                    {
                        return gardinersWithId.Single();
                    }
                    else
                    {

                    }
                }
                return null;
            }
            else
            {
                var entriesWithId = entries.Where(x => x.Images.Select(y => y.Id).Contains(id));
                var gardinersWithId = entriesWithId.SelectMany(x => x.GardinerSigns).ToArray();
                if (gardinersWithId.Length > 1)
                {
                    var countDict = GenerateCountDictionary(gardinersWithId);
                    var maxVal = countDict.Values.Max();
                    var bestCandidate = countDict.Where(x => x.Value == maxVal).Select(x => x.Key).ToArray();
                    return bestCandidate.Single();
                }
                else
                {

                }
            }
            return null;
        }

        private void MapPictureIdsForPage(PdfCollection<PdfPaintedImage> vygusPaintedImages, PageData pageData)
        {
            var imagesByEntry = pageData.EntryData.Select(x => x.Images);
            foreach (var entryImages in imagesByEntry)
            {
                foreach (var imageWrapper in entryImages)
                {
                    var image = imageWrapper.Image;
                    var matching = vygusPaintedImages.Where(x => Math.Abs(x.Position.X - image.Position.X) <= 0.5
                                                              && Math.Abs(x.Position.Y - image.Position.Y) <= 0.5)
                                                     .ToList();
                    imageWrapper.AddId(matching.Single().Image.Id);

                }
            }
        }

        private Dictionary<string, int> GenerateCountDictionary(IEnumerable<string> input)
        {
            Dictionary<string, int> countDict = new Dictionary<string, int>();
            foreach (string item in input)
            {
                if (countDict.ContainsKey(item))
                {
                    countDict[item] += 1;
                }
                else
                {
                    countDict.Add(item, 1);
                }
            }
            return countDict;
        }
    }
}
