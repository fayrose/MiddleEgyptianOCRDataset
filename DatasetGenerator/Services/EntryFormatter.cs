using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;

namespace DatasetGenerator.Services
{
    class EntryFormatter
    {
        public static void FormatDictionary(DictionaryData data)
        {
            foreach (var page in data.Pages)
            {
                foreach (var entry in page.EntryData)
                {
                    FormatEntry(entry);
                }
            }
        }

        public static void FormatEntry(EntryData entry)
        {
            int offset = 0;
            var gardiner = entry.GardinerSigns.ToArray();
            List<string> formattedBlocks = new List<string>();

            // Split Gardiners according to gardiner blocks
            for (int i = 0; i < entry.GlyphBlocks.Length; i++)
            {
                var block = entry.GlyphBlocks[i];
                var formattedBlock = GlyphBlockFormatter.Format(block, gardiner[offset..block.Size]);
                formattedBlocks.Add('(' + formattedBlock + ')');
                offset += block.Size;
            }

            var manuelDeCodage = String.Join('-', formattedBlocks);

            ValidateEntry(manuelDeCodage, entry.GardinerSigns);
            entry.ManuelDeCodage = manuelDeCodage;
        }

        private static void ValidateEntry(string MdC, ImmutableArray<string> entryGardiner)
        {
            // Ensures that the outputted formatting and the list of gardiner signs have the 
            // same order, implying correct behavior.
            var strippedMdC = MdC.Split(new char[] { '*', ':', '-', '(', ')' });
            Debug.Equals(strippedMdC.Length, entryGardiner.Length);
            for (int i = 0; i < entryGardiner.Length; i++)
            {
                Debug.Equals(entryGardiner[i], MdC[i]);
            }
        }
    }
}
