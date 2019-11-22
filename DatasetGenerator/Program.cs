
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace DatasetGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            BitMiracle.Docotic.LicenseManager.AddLicenseData("6EOI3-DXN35-5M8G6-8QGW9-Y18Z5");

            var inLocation = @"C:\Users\lfr2l\U of T\NML340\VYGUS_Dictionary_2018.pdf";
            var outFolder = @"C:\Users\lfr2l\U of T\CSC420\project\dataset\entry_images";
            var dataFolder = @"C:\Users\lfr2l\U of T\CSC420\project\dataset\aug_output";

            var jsonString = File.ReadAllText("data.json");
            DictionaryData data = JsonSerializer.Deserialize<DictionaryData>(jsonString);

            //jsonString = File.ReadAllText("characterMap.json");
            //Dictionary<string, string> charMap = JsonSerializer.Deserialize<Dictionary<string, string>>(jsonString);

            //DataAdjuster da = new DataAdjuster(data, inLocation, charMap);
            //data = da.FixData();

            //File.WriteAllText("data.json", JsonSerializer.Serialize<DictionaryData>(data));
            //CharacterDatasetGenerator cdg = new CharacterDatasetGenerator(inLocation, outFolder);
            //cdg.SaveCharacterFilesFromPdf(imageToSignMap);
            

            DatasetImageGenerator dig = new DatasetImageGenerator(dataFolder, outFolder, data);
            dig.GetImagesFromFolder();
        }
    }
}
