
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
            var dataFolder = @"C:\Users\lfr2l\U of T\CSC420\project\dataset\aug_output";

            //var jsonString = File.ReadAllText("characterMap.json");
            //Dictionary<string, string> imageToSignMap = JsonSerializer.Deserialize<Dictionary<string, string>>(jsonString);

            DatasetLabelGenerator dlg = new DatasetLabelGenerator(dataFolder);
            var data = dlg.ParseAllFiles();

            ImageIdToGardinerMapper mapper = new ImageIdToGardinerMapper(inLocation);
            var imageToSignMap = mapper.GetNamesFromSplitPdfs(data);

            File.WriteAllText("characterMap.json", JsonSerializer.Serialize<Dictionary<string, string>>(imageToSignMap));
            CharacterDatasetGenerator cdg = new CharacterDatasetGenerator(inLocation, outFolder);
            cdg.SaveCharacterFilesFromPdf(imageToSignMap);
        }
    }
}
