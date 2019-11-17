using BitMiracle.Docotic.Pdf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DatasetGenerator
{
    class DataAdjuster
    {
        PdfDocument Pdf;
        DictionaryData Data;
        Dictionary<string, string> ImageToSignMap;

        public DataAdjuster(DictionaryData data, string unsplitVygusPath, Dictionary<string, string> charMap)
        {
            Data = data;
            Pdf = new PdfDocument(unsplitVygusPath);
            ImageToSignMap = charMap;

        }

        public DictionaryData FixData()
        {
            MapFullVygusIdsToSplitVygusIds();
            MapVygusIdsToGardinerSigns();
            return Data;
        }

        private void MapVygusIdsToGardinerSigns()
        {
            for (int i = 0; i < Data.Pages.Length; i++)
            {
                MapEntriesOnPage(Data.Pages[i]);
            }
        }

        private void MapEntriesOnPage(PageData pageData)
        {
            foreach (var entry in pageData.EntryData)
            {
                foreach (var image in entry.Images)
                {
                    string sign = ImageToSignMap[image.Id];
                    image.GardinerSign = sign;

                    var blockImagesToChange = entry.GlyphBlocks.SelectMany(x => x.Images).Where(x => x.Id == image.Id);
                    foreach (var bImage in blockImagesToChange)
                    {
                        bImage.GardinerSign = sign;
                    }
                }
            }
        }

        private void MapFullVygusIdsToSplitVygusIds()
        {
            var paintedImagesPerPage = Pdf.Pages.Select(x => x.GetPaintedImages()).ToArray();
            
            for (int i = 0; i < Data.Pages.Length; i++)
            {
                MapPictureIdsForPage(paintedImagesPerPage[i], Data.Pages[i]);
            }
        }
        private void MapPictureIdsForPage(PdfCollection<PdfPaintedImage> vygusPaintedImages, PageData pageData)
        {
            Dictionary<string, string> idMap = new Dictionary<string, string>();
            var imagesByEntry = pageData.EntryData.Select(x => x.Images);
            foreach (var entry in pageData.EntryData)
            {
                var imagesInEntry = entry.Images;

                foreach (var imageWrapper in imagesInEntry)
                {
                    var matching = vygusPaintedImages.Where(x => Math.Abs(x.Position.X - imageWrapper.X) <= 0.5
                                                                && Math.Abs(x.Position.Y - imageWrapper.Y) <= 0.5)
                                                        .Single().Image.Id;

                    var glyphBlocksToAmend = entry.GlyphBlocks.SelectMany(x => x.Images).Where(x => x.Id == imageWrapper.Id);
                    foreach (var block in glyphBlocksToAmend)
                    {
                        block.Id = matching;
                    }
                    imageWrapper.AddId(matching);
                }

            }
        }

    }
}
