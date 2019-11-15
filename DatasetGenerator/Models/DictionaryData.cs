using System.Collections.Immutable;
using System.Runtime.Serialization;

namespace DatasetGenerator
{
    public class DictionaryData
    {
        public ImmutableArray<PageData> Pages { get; set; }
    }
}
