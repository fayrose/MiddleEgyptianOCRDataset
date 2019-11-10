using System;
using System.Collections.Generic;
using System.Text;

namespace DatasetGenerator
{
    public class PageData
    {
        public EntryData[] EntryData { get; internal set; }
        public string FileLocation { get; internal set; }
        public int PageNumber { get; internal set; }
    }
}
