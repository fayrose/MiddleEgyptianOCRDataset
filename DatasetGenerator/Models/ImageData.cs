using System;
using BitMiracle.Docotic.Pdf;

namespace DatasetGenerator
{
    public class ImageData
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public string Id { get; set; }
        public string GardinerSign { get; set; }

        public ImageData()
        {
            //Neeeded for deserialization
        }

        public ImageData(PdfPaintedImage image)
        {
            X = image.Bounds.X;
            Y = image.Bounds.Y;
            Width = image.Bounds.Width;
            Height = image.Bounds.Height;
            Id = image.Image.Id;
        }

        public void AddId(String id)
        {
            Id = id;
        }
    }
}
