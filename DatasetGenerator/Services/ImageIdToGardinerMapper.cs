using BitMiracle.Docotic.Pdf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DatasetGenerator.Services
{
    class ImageIdToGardinerMapper
    {
        PdfDocument Pdf;

        public ImageIdToGardinerMapper(PdfDocument pdf)
        {
            Pdf = pdf;
        }

        public Dictionary<string, string> GetNamesFromSplitPdfs(DictionaryData datasetData)
        {
            MapFullVygusIdsToSplitVygusIds(datasetData);

            // Get count of each image ID
            var entries = datasetData.Pages.SelectMany(x => x.EntryData);
            var imageIds = entries.SelectMany(x => x.Images.Select(y => y.Id));
            var entryIdCounts = CountDictionary.Generate(imageIds);

            // Get count of each gardiner ID
            var gardinerIds = entries.SelectMany(x => x.GardinerSigns);
            var gardinerCounts = CountDictionary.Generate(gardinerIds);

            Dictionary<string, string> ImageIdToGardinerValue = new Dictionary<string, string>();
            foreach (var id in entryIdCounts.Keys)
            {
                Console.WriteLine(ImageIdToGardinerValue.Count);
                var gardiner = GetNameFromEntries(entries, id, entryIdCounts, gardinerCounts);
                ImageIdToGardinerValue.Add(id, gardiner);
            }
            return ImageIdToGardinerValue;
        }

        private void MapFullVygusIdsToSplitVygusIds(DictionaryData data)
        {
            var paintedImagesPerPage = Pdf.Pages.Select(x => x.GetPaintedImages()).ToArray();
            var fullVygusIds = paintedImagesPerPage.SelectMany(x => x) // Flattens before getting Ids
                                                   .Select(x => x.Image.Id);
            var fullVygusCounts = CountDictionary.Generate(fullVygusIds);

            for (int i = 0; i < data.Pages.Length; i++)
            {
                MapPictureIdsForPage(paintedImagesPerPage[i], data.Pages[i]);
            }
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
                    return closeCandidates.Where(x => x.Value == maxVal).Single().Key;
                }
                else
                {
                    var entriesWithId = entries.Where(x => x.Images.Select(y => y.Id).Contains(id));
                    var gardinersWithId = entriesWithId.SelectMany(x => x.GardinerSigns).ToArray();
                    if (gardinersWithId.Length > 1)
                    {
                        var countDict = CountDictionary.Generate(gardinersWithId);
                        var maxVal = countDict.Values.Max();
                        var bestCandidate = countDict.Where(x => x.Value == maxVal).Select(x => x.Key).ToArray();
                        if (bestCandidate.Length == 1)
                        {
                            return bestCandidate.Single();
                        }
                        else
                        {
                            var bestCandidateOccurrances = bestCandidate.Select(x => gardinerCounts[x]);
                            var candidateOccurrancesDiffFromIdOccurances = bestCandidateOccurrances.Select(x => Math.Abs(entriesWithId.Count() - x));
                            var lowestError = candidateOccurrancesDiffFromIdOccurances.Min();
                            var entriesWithLowestError = bestCandidate.Where(x => Math.Abs(entriesWithId.Count() - gardinerCounts[x]) == lowestError);

                            if (entriesWithLowestError.Count() == 1)
                            {
                                return entriesWithLowestError.Single();
                            }
                            else
                            {
                                if (entriesWithId.Count() == 1)
                                {
                                    var entr = entriesWithId.Single();
                                    if (entriesWithLowestError.Contains(entr.GardinerSigns[entr.GardinerSigns.Length - 1])
                                     && entr.GlyphBlocks[entr.GlyphBlocks.Length - 1].Size == 1)
                                    {
                                        var lastBlock = entr.GlyphBlocks[entr.GlyphBlocks.Length - 1];
                                        var image = entr.Images.Where(x => x.Id == id).Single();
                                        if (entr.Images.Last().Id == image.Id)
                                        {
                                            return entr.GardinerSigns[entr.GardinerSigns.Length - 1];
                                        }
                                        else
                                        {
                                            return entriesWithLowestError.Where(x => x != entr.GardinerSigns[entr.GardinerSigns.Length - 1]).Single();
                                        }
                                    }
                                    else if (entr.GlyphBlocks.Where(x => x.Size == 1).Count() == entr.GlyphBlocks.Length)
                                    {
                                        for (int i = 0; i < entr.GlyphBlocks.Length; i++)
                                        {
                                            if (entr.GlyphBlocks[i].Images[0].Id == id)
                                            {
                                                return entr.GardinerSigns[i];
                                            }
                                            else if (entr.Images[i].Id == id)
                                            {
                                                return entr.GardinerSigns[i];
                                            }
                                        }
                                    }
                                }
                            }
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
            }
            else
            {
                var entriesWithId = entries.Where(x => x.Images.Select(y => y.Id).Contains(id));
                var gardinersWithId = entriesWithId.SelectMany(x => x.GardinerSigns).ToArray();
                if (gardinersWithId.Length > 1)
                {
                    var countDict = CountDictionary.Generate(gardinersWithId);
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
            Dictionary<string, string> idMap = new Dictionary<string, string>();
            var imagesByEntry = pageData.EntryData.Select(x => x.Images);
            foreach (var entryImages in imagesByEntry)
            {
                foreach (var imageWrapper in entryImages)
                {
                    var matching = vygusPaintedImages.Where(x => Math.Abs(x.Position.X - imageWrapper.X) <= 0.5
                                                              && Math.Abs(x.Position.Y - imageWrapper.Y) <= 0.5)
                                                     .Single().Image.Id;
                    idMap.Add(imageWrapper.Id, matching);
                    imageWrapper.AddId(matching);

                }
            }
            foreach (var entry in pageData.EntryData)
            {
                foreach (var block in entry.GlyphBlocks)
                {
                    foreach (var image in block.Images)
                    {
                        image.AddId(idMap[image.Id]);
                    }
                }
            }
        }
    }
}
