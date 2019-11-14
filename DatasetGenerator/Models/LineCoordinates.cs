using System.Runtime.Serialization;

namespace DatasetGenerator
{
    [DataContract]
    public class LineCoordinates
    {
        [DataMember]
        public double LineBottom { get; internal set; }
        [DataMember]
        public double LineTop { get; internal set; }
        [DataMember]
        public double LineHeight { get; internal set; }
        [DataMember]
        public double TextHeight { get; internal set; }
    }
}
