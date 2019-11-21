using BitMiracle.Docotic.Pdf;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace DatasetGenerator
{
    class ImageCopier
    {
        private readonly PdfPage Page;

        public ImageCopier(PdfPage page)
        {
            Page = page;
        }

        public void GenerateLineImage(string outPath, double width, string outfileName)
        {
            using Bitmap bm = new Bitmap((int)Page.Width, (int)Page.Height);
            bm.SetResolution(Page.Canvas.Resolution, Page.Canvas.Resolution);

            using var graphics = Graphics.FromImage(bm);
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            graphics.PageUnit = GraphicsUnit.Point;


            var clipBox = new RectangleF((float)0.0, (float)Page.CropBox.Top, (float)width, (float)Page.CropBox.Height);

            //Page.CropBox = new PdfBox(0, Page.CropBox.Bottom, width,Page.CropBox.Top);
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

            bm.Save(outPath + "/"+ outfileName + ".Tiff", ImageFormat.Tiff);
        }

        private void drawPath(Graphics graphics, PdfPath path)
        {
            if (!path.PaintMode.HasValue) return;

            saveStateAndDraw(graphics, path.ClipRegion, () =>
            {
                concatMatrix(graphics, path.TransformationMatrix);
                using var gdiPath = new GraphicsPath();
                
                toGdiPath(path, gdiPath);
                fillStrokePath(graphics, path, gdiPath);
                
            });
        }

        private void drawImage(Graphics graphics, PdfPaintedImage image)
        {
            PdfSize paintedImageSize = image.Bounds.Size;
            if (Math.Abs(paintedImageSize.Width) < 0.001 ||
                Math.Abs(paintedImageSize.Height) < 0.001) return;

            if (image.Image.IsMask) return;

            using var stream = new MemoryStream();
            image.Image.Save(stream);
            using Bitmap bitmap = (Bitmap)Image.FromStream(stream);
            saveStateAndDraw(graphics, image.ClipRegion, () =>
            {
                graphics.TranslateTransform((float)image.Position.X, (float)image.Position.Y);
                concatMatrix(graphics, image.TransformationMatrix);

                graphics.PixelOffsetMode = PixelOffsetMode.Half;

                PdfSize imageSize = new PdfSize(image.Image.Width, image.Image.Height);
                if (imageSize.Width < bitmap.Width && imageSize.Height < bitmap.Height)
                {
                    InterpolationMode current = graphics.InterpolationMode;
                    graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    graphics.DrawImage(bitmap, 0, 0, (float)imageSize.Width, (float)imageSize.Height);
                    graphics.InterpolationMode = current;
                }
                else
                {
                    graphics.DrawImage(bitmap, 0, 0, bitmap.Width, bitmap.Height);
                }
            });
        }

        private void drawText(Graphics graphics, PdfTextData obj)
        {
            if (obj.RenderingMode == PdfTextRenderingMode.NeitherFillNorStroke ||
                obj.RenderingMode == PdfTextRenderingMode.AddToPath)
                return;

            if (Math.Abs(obj.FontSize) < 0.001) return;
            if (Math.Abs(obj.Bounds.Width) < 0.001 ||
                Math.Abs(obj.Bounds.Height) < 0.001) return;

            saveStateAndDraw(graphics, obj.ClipRegion, () =>
            {
                using Font font = toGdiFont(obj.Font, obj.FontSize);
                using Brush brush = toGdiBrush(obj.Brush);
                    
                graphics.TranslateTransform((float)obj.Position.X, (float)obj.Position.Y);
                concatMatrix(graphics, obj.TransformationMatrix);

                graphics.DrawString(obj.Text, font, brush, PointF.Empty);
            });
        }

        private void saveStateAndDraw(Graphics graphics, PdfClipRegion clipRegion, Action draw)
        {
            var state = graphics.Save();
            try
            {
                setClipRegion(graphics, clipRegion);
                draw();
            }
            finally
            {
                graphics.Restore(state);
            }
        }

        private void setClipRegion(Graphics graphics, PdfClipRegion clipRegion)
        {
            if (clipRegion.IntersectedPaths.Count == 0) return;

            var beforeTransform = graphics.Transform;
            try
            {
                graphics.Transform = new Matrix();
                foreach (PdfPath clipPath in clipRegion.IntersectedPaths)
                {
                    using var gdiPath = new GraphicsPath();
                    toGdiPath(clipPath, gdiPath);
                    gdiPath.Transform(toGdiMatrix(clipPath.TransformationMatrix));

                    gdiPath.FillMode = (FillMode)clipPath.ClipMode.Value;
                    graphics.SetClip(gdiPath, CombineMode.Intersect);
                }
            }
            finally
            {
                graphics.Transform = beforeTransform;
            }
        }

        private void toGdiPath(PdfPath path, GraphicsPath gdiPath)
        {
            foreach (PdfSubpath subpath in path.Subpaths)
            {
                gdiPath.StartFigure();

                foreach (PdfPathSegment segment in subpath.Segments)
                {
                    switch (segment.Type)
                    {
                        case PdfPathSegmentType.Point:
                            break;

                        case PdfPathSegmentType.Line:
                            PdfLineSegment line = (PdfLineSegment)segment;
                            gdiPath.AddLine(line.Start.ToPointF(), line.End.ToPointF());
                            break;

                        case PdfPathSegmentType.Bezier:
                            PdfBezierSegment bezier = (PdfBezierSegment)segment;
                            gdiPath.AddBezier(bezier.Start.ToPointF(),
                                bezier.FirstControl.ToPointF(),
                                bezier.SecondControl.ToPointF(),
                                bezier.End.ToPointF()
                            );
                            break;

                        case PdfPathSegmentType.Rectangle:
                            RectangleF rect = ((PdfRectangleSegment)segment).Bounds.ToRectangleF();
                            gdiPath.AddLines(new PointF[]
                            {
                                rect.Location,
                                new PointF(rect.X + rect.Width, rect.Y),
                                new PointF(rect.X + rect.Width, rect.Y + rect.Height),
                                new PointF(rect.X, rect.Y + rect.Height),
                                rect.Location
                            });
                            break;

                        case PdfPathSegmentType.CloseSubpath:
                            gdiPath.CloseFigure();
                            break;
                    }
                }
            }
        }

        private void fillStrokePath(Graphics gr, PdfPath path, GraphicsPath gdiPath)
        {
            PdfDrawMode paintMode = path.PaintMode.Value;
            if (paintMode == PdfDrawMode.Fill || paintMode == PdfDrawMode.FillAndStroke)
            {
                using var brush = toGdiBrush(path.Brush);
                gdiPath.FillMode = (FillMode)path.FillMode.Value;
                gr.FillPath(brush, gdiPath);
            }

            if (paintMode == PdfDrawMode.Stroke || paintMode == PdfDrawMode.FillAndStroke)
            {
                using var pen = toGdiPen(path.Pen);
                gr.DrawPath(pen, gdiPath);
            }
        }

        private void concatMatrix(Graphics gr, PdfMatrix transformation)
        {
            using Matrix m = toGdiMatrix(transformation);
            using Matrix current = gr.Transform;
            current.Multiply(m, MatrixOrder.Prepend);
            gr.Transform = current;
        }

        private Matrix toGdiMatrix(PdfMatrix matrix)
        {
            return new Matrix(
                (float)matrix.M11,
                (float)matrix.M12,
                (float)matrix.M21,
                (float)matrix.M22,
                (float)matrix.OffsetX,
                (float)matrix.OffsetY
            );
        }

        private Brush toGdiBrush(PdfBrushInfo brush)
        {
            return new SolidBrush(toGdiColor(brush.Color, brush.Opacity));
        }

        private Pen toGdiPen(PdfPenInfo pen)
        {
            return new Pen(toGdiColor(pen.Color, pen.Opacity), (float)pen.Width)
            {
                LineJoin = toGdiLineJoin(pen.LineJoin),
                EndCap = toGdiLineCap(pen.EndCap),
                MiterLimit = (float)pen.MiterLimit
            };
        }

        private Color toGdiColor(PdfColor pdfColor, int opacityPercent)
        {
            if (pdfColor == null)
                return Color.Empty;

            Color color = pdfColor.ToColor();

            int alpha = 255;
            if (opacityPercent < 100 && opacityPercent >= 0)
                alpha = (int)(255.0 * opacityPercent / 100.0);

            return Color.FromArgb(alpha, color.R, color.G, color.B);
        }

        private Font toGdiFont(PdfFont font, double fontSize)
        {
            string fontName = getFontName(font);
            FontStyle fontStyle = getFontStyle(font);

            return new Font(fontName, (float)fontSize, fontStyle);
        }

        private LineCap toGdiLineCap(PdfLineCap lineCap)
        {
            switch (lineCap)
            {
                case PdfLineCap.ButtEnd:
                    return LineCap.Flat;

                case PdfLineCap.Round:
                    return LineCap.Round;

                case PdfLineCap.ProjectingSquare:
                    return LineCap.Square;

                default:
                    throw new InvalidOperationException("Should never happen.");
            }
        }

        private LineJoin toGdiLineJoin(PdfLineJoin lineJoin)
        {
            switch (lineJoin)
            {
                case PdfLineJoin.Miter:
                    return LineJoin.Miter;

                case PdfLineJoin.Round:
                    return LineJoin.Round;

                case PdfLineJoin.Bevel:
                    return LineJoin.Bevel;

                default:
                    throw new InvalidOperationException("We should never be here");
            }
        }

        private string getFontName(PdfFont font)
        {
            // A trick to load a similar font for system. Ideally we should load font from raw bytes. Use PdfFont.Save()
            // method for that.
            string fontName = font.Name;
            if (fontName.Contains("Times"))
                return "Times New Roman";

            if (fontName.Contains("Garamond"))
                return "Garamond";

            if (fontName.Contains("Arial") && !fontName.Contains("Unicode"))
                return "Arial";

            return font.Name;
        }

        private FontStyle getFontStyle(PdfFont font)
        {
            FontStyle fontStyle = FontStyle.Regular;

            if (font.Bold)
                fontStyle |= FontStyle.Bold;

            if (font.Italic)
                fontStyle |= FontStyle.Italic;

            return fontStyle;
        }
    }
}
