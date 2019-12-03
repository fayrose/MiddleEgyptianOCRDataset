
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using DatasetGenerator.Services;

namespace DatasetGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            BitMiracle.Docotic.LicenseManager.AddLicenseData("7GO0J-1HDYO-297Q3-OJDM0-PE4WW");

            var inLocation = @"/Users/thomashorga/Documents/CSC420/Visual_Vygus/VYGUS_Dictionary_2018.pdf";
            var outFolder = @"/Users/thomashorga/Documents/CSC420/Visual_Vygus/entry_images";
            var dataFolder = @"/Users/thomashorga/Documents/CSC420/Visual_Vygus/aug_output";

            var jsonString = File.ReadAllText("data.json");
            DictionaryData data = JsonSerializer.Deserialize<DictionaryData>(jsonString);

            EntryFormatter.FormatDictionary(data);

            File.WriteAllText("data2.json", JsonSerializer.Serialize<DictionaryData>(data));

            //jsonString = File.ReadAllText("characterMap.json");
            //Dictionary<string, string> charMap = JsonSerializer.Deserialize<Dictionary<string, string>>(jsonString);

            //DataAdjuster da = new DataAdjuster(data, inLocation, charMap);
            //data = da.FixData();

            //File.WriteAllText("data.json", JsonSerializer.Serialize<DictionaryData>(data));
            //CharacterDatasetGenerator cdg = new CharacterDatasetGenerator(inLocation, outFolder);
            //cdg.SaveCharacterFilesFromPdf(imageToSignMap);


            //DatasetImageGenerator dig = new DatasetImageGenerator(dataFolder, outFolder, data);
            //dig.GetImagesFromFolder();
        }
    }
}
