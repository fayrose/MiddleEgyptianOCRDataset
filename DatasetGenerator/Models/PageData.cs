using System.Runtime.Serialization;

namespace DatasetGenerator
{
    [DataContract]
    public class PageData
    {
        [DataMember]
        public EntryData[] EntryData { get; internal set; }
        [DataMember]
        public string FileLocation { get; internal set; }
        [DataMember]
        public int PageNumber { get; internal set; }
    }
}
