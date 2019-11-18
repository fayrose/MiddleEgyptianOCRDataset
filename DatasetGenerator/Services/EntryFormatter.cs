using System;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;

namespace DatasetGenerator.Services
{
    class EntryFormatter
    {
        public static void FormatEntry(EntryData entry)
        {
            var formattedBlocks = entry.GlyphBlocks.Select(x => GlyphBlockFormatter.Format(x));
            var parenthesizedFormattedBlocks = formattedBlocks.Select(x => "(" + x + ")");
            var manuelDeCodage = String.Join('-', parenthesizedFormattedBlocks);

            ValidateEntry(manuelDeCodage, entry.GardinerSigns);
            entry.ManuelDeCodage = manuelDeCodage;
        }

        private static void ValidateEntry(string MdC, ImmutableArray<string> entryGardiner)
        {
            var strippedMdC = MdC.Split(new char[] { '*', ':', '-', '(', ')' });
            Debug.Equals(strippedMdC.Length, entryGardiner.Length);
            for (int i = 0; i < entryGardiner.Length; i++)
            {
                Debug.Equals(entryGardiner[i], MdC[i]);
            }
        }
    }
}
