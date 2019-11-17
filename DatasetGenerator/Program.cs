
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
            var outFolder = @"C:\Users\lfr2l\U of T\CSC420\project\dataset\character_images";
            var splitPdfPath = @"C:\Users\lfr2l\U of T\CSC420\project\dataset\aug_output";

            DatasetLabelGenerator dlg = new DatasetLabelGenerator(splitPdfPath);
            DictionaryData data = dlg.ParseAllFiles();

            var jsonString = File.ReadAllText("characterMap.json");
            Dictionary<string, string> imageToSignMap = JsonSerializer.Deserialize<Dictionary<string, string>>(jsonString);

            DataAdjuster da = new DataAdjuster(data, inLocation, imageToSignMap);
            DictionaryData fixedData = da.FixData();

            try
            {
                var outStr = JsonSerializer.Serialize<DictionaryData>(fixedData);
                File.WriteAllText("data.json", outStr);
            }
            catch
            {
                Console.WriteLine("Something broke :(");
            }

            CharacterDatasetGenerator cdg = new CharacterDatasetGenerator(inLocation, outFolder);
            cdg.SaveCharacterFilesFromPdf(imageToSignMap);
        }
    }
}
