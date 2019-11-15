using System.Runtime.Serialization;

namespace DatasetGenerator
{
    public class PageData
    {
        public EntryData[] EntryData { get; set; }
        public string FileLocation { get; set; }
        public int PageNumber { get; set; }

        public PageData()
        {

        }
    }
}
