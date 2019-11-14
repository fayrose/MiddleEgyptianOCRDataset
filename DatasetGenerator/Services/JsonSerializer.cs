using System.IO;
using System.Runtime.Serialization.Json;

namespace DatasetGenerator.Services
{
    public static class JsonSerializer
    {
        public static void SerializeDictionaryDataToJson(DictionaryData data, string fileLocation)
        {
            using var stream = new MemoryStream();
            var ser = new DataContractJsonSerializer(typeof(DictionaryData));
            ser.WriteObject(stream, data);

            using FileStream streamWriter = new FileStream(fileLocation, FileMode.OpenOrCreate);
            stream.CopyTo(streamWriter);
        }
    }
}
