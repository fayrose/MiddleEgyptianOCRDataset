using BitMiracle.Docotic.Pdf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace DatasetGenerator
{
    class PdfObjectCopier
    {
        readonly PdfPage Page;
        readonly int PageIndex;
        readonly PdfDocument SourceDocument;
        readonly List<LineCoordinates> LineInformation;
        readonly string OutputDestination;

        public PdfObjectCopier(PdfDocument pdf, int pageIdx, List<LineCoordinates> lines, string outDir)
        {
            PageIndex = pageIdx;
            SourceDocument = pdf;
            Page = pdf.GetPage(pageIdx);
            LineInformation = lines;
            OutputDestination = outDir;
        }

        public void GenerateCroppedPage()
        {
            for (int i = 0; i < LineInformation.Count; i++)
            {
                string outputName = "page" + PageIndex.ToString() + "_" + i.ToString() + ".pdf";
                GenerateCroppedLine(LineInformation[i], outputName);
            }
        }

        private void GenerateCroppedLine(LineCoordinates line, string outPath)
        {
            using (PdfDocument copyDoc = SourceDocument.CopyPages(new PdfPage[] { Page }))
            {
                PdfPage sourcePage = copyDoc.Pages[0];
                PdfPage copyPage = copyDoc.AddPage();

                copyPage.Rotation = sourcePage.Rotation;
                copyPage.MediaBox = sourcePage.MediaBox;

                copyPage.CropBox = new PdfBox(0, line.LineBottom, Page.Width, line.LineTop);

                PdfCanvas target = copyPage.Canvas;

                foreach (PdfPageObject obj in sourcePage.GetObjects())
                {
                    target.SaveState();
                    setClipRegion(target, obj.ClipRegion);

                    switch (obj.Type)
                    {
                        case PdfPageObjectType.Path:
                            PathHandler(obj, target);
                            break;
                        case PdfPageObjectType.Image:
                            ImageHandler(obj, target);
                            break;
                        case PdfPageObjectType.Text:
                            TextHandler(obj, target, copyDoc);
                            break;
                    }

                    target.RestoreState();
                }
                copyDoc.RemovePage(0);
                copyDoc.Save(Path.Combine(OutputDestination, outPath));
            }
        }

        private void PathHandler(PdfPageObject obj, PdfCanvas target)
        {
            var path = (PdfPath)obj;
            target.Transform(path.TransformationMatrix);

            if (path.PaintMode == PdfDrawMode.Fill || path.PaintMode == PdfDrawMode.FillAndStroke)
                setBrush(target.Brush, path.Brush);
            if (path.PaintMode == PdfDrawMode.Stroke || path.PaintMode == PdfDrawMode.FillAndStroke)
                setPen(target.Pen, path.Pen);
            appendPath(target, path);
            drawPath(target, path);
        }

        private void ImageHandler(PdfPageObject obj, PdfCanvas target)
        {
            var image = (PdfPaintedImage) obj;
            target.TranslateTransform(image.Position.X, image.Position.Y);
            target.Transform(image.TransformationMatrix);

            setBrush(target.Brush, image.Brush);
            target.DrawImage(image.Image, 0, 0, 0);
        }

        private void TextHandler(PdfPageObject obj, PdfCanvas target, PdfDocument copy)
        {
            var text = (PdfTextData)obj;
            drawText(target, text, copy);
        }

        private void setClipRegion(PdfCanvas canvas, PdfClipRegion clipRegion)
        {
            if (clipRegion.IntersectedPaths.Count == 0) return;

            PdfMatrix beforeTransform = canvas.TransformationMatrix;

            try
            {
                foreach (var path in clipRegion.IntersectedPaths)
                {
                    canvas.ResetTransform();
                    canvas.Transform(path.TransformationMatrix);
                    appendPath(canvas, path);
                    canvas.SetClip(path.ClipMode.Value);
                }
            }
            finally
            {
                canvas.ResetTransform();
                canvas.Transform(beforeTransform);
            }
        }

        private void setBrush(PdfBrush dest, PdfBrushInfo source)
        {
            PdfColor colour = source.Color;
            if (colour != null)
            {
                dest.Color = colour;
            }

            var pattern = source.Pattern;
            if (pattern != null)
            {
                dest.Pattern = pattern;
            }

            dest.Opacity = source.Opacity;
        }
        
        private void setPen(PdfPen dest, PdfPenInfo source)
        {
            PdfColor colour = source.Color;
            if (colour != null)
            {
                dest.Color = colour;
            }

            var pattern = source.Pattern;
            if (pattern != null)
            {
                dest.Pattern = pattern;
            }

            dest.DashPattern = source.DashPattern;
            dest.EndCap = dest.EndCap;
            dest.LineJoin = source.LineJoin;
            dest.MiterLimit = source.MiterLimit;
            dest.Opacity = source.Opacity;
            dest.Width = source.Width;
        }

        private void appendPath(PdfCanvas target, PdfPath path)
        {
            foreach (var subpath in path.Subpaths)
            {
                foreach (var segment in subpath.Segments)
                {
                    switch (segment.Type)
                    {
                        case PdfPathSegmentType.Point:
                            target.CurrentPosition = ((PdfPointSegment)segment).Value;
                            break;

                        case PdfPathSegmentType.Line:
                            var line = (PdfLineSegment) segment;
                            target.CurrentPosition = line.Start;
                            target.AppendLineTo(line.End);
                            break;

                        case PdfPathSegmentType.Bezier:
                            var bezier = (PdfBezierSegment)segment;
                            target.CurrentPosition = bezier.Start;
                            target.AppendCurveTo(bezier.FirstControl, bezier.SecondControl, bezier.End);
                            break;

                        case PdfPathSegmentType.Rectangle:
                            target.AppendRectangle(((PdfRectangleSegment)segment).Bounds);
                            break;

                        case PdfPathSegmentType.CloseSubpath:
                            target.ClosePath();
                            break;
                    }
                }
            }
        }

        private void drawPath(PdfCanvas target, PdfPath path)
        {
            switch (path.PaintMode)
            {
                case PdfDrawMode.Fill:
                    target.FillPath(path.FillMode.Value);
                    break;

                case PdfDrawMode.FillAndStroke:
                    target.FillAndStrokePath(path.FillMode.Value);
                    break;

                case PdfDrawMode.Stroke:
                    target.StrokePath();
                    break;

                default:
                    target.ResetPath();
                    break;
            }
        }

        private void drawText(PdfCanvas target, PdfTextData text, PdfDocument pdf)
        {
            target.TextRenderingMode = text.RenderingMode;
            setBrush(target.Brush, text.Brush);
            setPen(target.Pen, text.Pen);

            target.TextPosition = PdfPoint.Empty;
            target.FontSize = text.FontSize;
            target.Font = text.Font;
            target.TranslateTransform(text.Position.X, text.Position.Y);
            target.Transform(text.TransformationMatrix);

            target.DrawString(text.GetCharacterCodes());
        }
    }
}
