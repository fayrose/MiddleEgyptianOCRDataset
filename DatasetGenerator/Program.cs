using BitMiracle.Docotic.Pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;


namespace DatasetGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            BitMiracle.Docotic.LicenseManager.AddLicenseData("6EOI3-DXN35-5M8G6-8QGW9-Y18Z5");

            //var inLocation = @"C:\Users\lfr2l\U of T\NML340\VYGUS_Dictionary_2018.pdf";
            //var outPath = @"C:\Users\lfr2l\U of T\CSC420\project\dataset\character_images";
            //var dataPath = @"C:\Users\lfr2l\U of T\CSC420\project\dataset\aug_output";
            //var labelDataPath = @"C:\Users\lfr2l\U of T\CSC420\project\dataset\label_data.json";

            var inLocation = @"/Users/thomashorga/Documents/CSC420/Visual_Vygus/VYGUS_Dictionary_2018.pdf";
            var outPath = @"/Users/thomashorga/Documents/CSC420/Visual_Vygus/output";
            var dataPath = @"/Users/thomashorga/Documents/CSC420/Visual_Vygus/aug_output";
            var labelDataPath = @"/Users/thomashorga/Documents/CSC420/Visual_Vygus/label_data.json";


            //DatasetGenerator dg = new DatasetGenerator(inLocation, outPath);
            //dg.ParsePage(0);

            DictionaryData data = null;
            if (File.Exists("data.json")){
                string jsonString = File.ReadAllText("data.json");
                data = JsonSerializer.Deserialize<DictionaryData>(jsonString);
            }
            else
            {
                DatasetLabelGenerator dlg = new DatasetLabelGenerator(outPath);
                data = dlg.ParseAllFiles();


                CharacterDatasetGenerator cdg = new CharacterDatasetGenerator(inLocation, outPath);
                cdg.GetNamesFromSplitPdfs(data);

                String json = JsonSerializer.Serialize(data);
                File.WriteAllText("data.json", json);
            }


            //DictionaryData test = JsonSerializer.Deserialize<DictionaryData>(jsonString);
            //if (File.Exists("entry" + i.ToString() + ".json"))
            //{
            //    string jsonString = File.ReadAllText("entry" + i.ToString() + ".json");
            //    EntryData test = JsonSerializer.Deserialize<EntryData>(jsonString);
            //}


        }
    }
}
