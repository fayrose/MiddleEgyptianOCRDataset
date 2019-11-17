using System;
using System.Linq;

namespace DatasetGenerator.Services
{
    class EntryFormatter
    {
        public static string FormatEntry(EntryData entry)
        {
            var formattedBlocks = entry.GlyphBlocks.Select(x => GlyphBlockFormatter.FormatGlyphBlock(x));
            var parenthesizedFormattedBlocks = formattedBlocks.Select(x => "(" + x + ")");
            return String.Join('-', parenthesizedFormattedBlocks);
        }
    }
}
