using BitMiracle.Docotic.Pdf;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace DatasetGenerator
{
    class ImageCopier
    {
        private readonly PdfPage Page;
        public List<LineCoordinates> LineLocations;

        public ImageCopier(PdfPage page, List<LineCoordinates> lineLocations)
        {
            Page = page;
            LineLocations = lineLocations;
        }

        private void GenerateLineImage(LineCoordinates coords, string outPath)
        {
            using (Bitmap bm = new Bitmap((int)Page.Width, (int)Page.Height))
            {
                bm.SetResolution(Page.Canvas.Resolution, Page.Canvas.Resolution);

                using (var graphics = Graphics.FromImage(bm))
                {
                    graphics.SmoothingMode = SmoothingMode.HighQuality;
                    graphics.PageUnit = GraphicsUnit.Point;

                    var clipBox = new RectangleF((float)0.0, (float)coords.LineTop, (float)Page.Width, (float)coords.LineHeight);

                    //Page.CropBox = new PdfBox(0, coords.LineBottom, Page.Width, coords.LineTop);
                    graphics.SetClip(clipBox, CombineMode.Intersect);
                    
                    foreach (PdfPageObject obj in Page.GetObjects())
                    {
                        switch (obj.Type)
                        {
                            case PdfPageObjectType.Text:
                                drawText(graphics, (PdfTextData)obj);
                                break;
                            case PdfPageObjectType.Image:
                                drawImage(graphics, (PdfPaintedImage)obj);
                                break;
                            case PdfPageObjectType.Path:
                                drawPath(graphics, (PdfPath)obj);
                                break;
                            default:
                                break;
                        }
                    }
                }

                bm.Save(outPath, ImageFormat.Png);
            }
        }

        private void drawPath(Graphics graphics, PdfPath obj)
        {
            throw new NotImplementedException();
        }

        private void drawImage(Graphics graphics, PdfPaintedImage obj)
        {
            throw new NotImplementedException();
        }

        private void drawText(Graphics graphics, PdfTextData obj)
        {
            throw new NotImplementedException();
        }
    }
}
