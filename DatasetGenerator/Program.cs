using BitMiracle.Docotic.Pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DatasetGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            BitMiracle.Docotic.LicenseManager.AddLicenseData("6EOI3-DXN35-5M8G6-8QGW9-Y18Z5");

            var inLocation = @"C:\Users\lfr2l\U of T\NML340\VYGUS_Dictionary_2018.pdf";
            var outPath = @"C:\Users\lfr2l\U of T\CSC420\project\dataset\character_images";
            var dataPath = @"C:\Users\lfr2l\U of T\CSC420\project\dataset\aug_output";
            CharacterDatasetGenerator cdg = new CharacterDatasetGenerator(inLocation, outPath);
            cdg.GetNamesFromSplitPdfs(dataPath);
        }
    }
}
