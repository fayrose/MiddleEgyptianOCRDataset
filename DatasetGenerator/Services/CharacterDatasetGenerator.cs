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

        
        public Dictionary<string, string> GetNamesFromSplitPdfs(DictionaryData datasetData)
        {
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
                            // Return candidate with gardinerCounts[bestCandidate] closest to gardinersWithId.Length
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
            }
            return null;
        }

        
    }
}
