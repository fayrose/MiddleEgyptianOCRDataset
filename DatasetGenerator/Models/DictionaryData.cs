using System.Collections.Immutable;
using System.Runtime.Serialization;

namespace DatasetGenerator
{
    [DataContract]
    public class DictionaryData
    {
        [DataMember]
        public ImmutableArray<PageData> Pages { get; internal set; }
    }
}
