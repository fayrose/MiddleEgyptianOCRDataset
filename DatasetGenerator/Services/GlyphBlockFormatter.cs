using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace DatasetGenerator
{
    class GlyphBlockFormatter
    {
        public static string Format(GlyphBlock block, string[] gardinerSignsInBlockInOrder)
        {
            switch (block.Size) 
            {
                case 1:
                    return gardinerSignsInBlockInOrder[0];
                case 2:
                    return TwoGlyphCase(block, gardinerSignsInBlockInOrder);
                case 3:
                    return ThreeGlyphCase(block, gardinerSignsInBlockInOrder);
                case 4:
                    return FourGlyphCase(block, gardinerSignsInBlockInOrder);

                default:
                    throw new ArgumentException("Block has invalid number of glyphs.");
            }
        }

        private static string FourGlyphCase(GlyphBlock block, string[] gardiners)
        {
            double i = Math.Max(block.Images[0].X + block.Images[0].Width, block.Images[1].X + block.Images[1].Width);
            List<ImageData> imagesYSort = block.Images.OrderBy(image => image.Y).ToList();

            if (i > block.Images[2].X || i > block.Images[3].X)
            {
                double yi = Math.Max(imagesYSort[0].X + imagesYSort[0].Height, imagesYSort[1].X + imagesYSort[1].Height);
                if (imagesYSort[0].Y + imagesYSort[0].Width > imagesYSort[1].Y && yi < imagesYSort[2].Y && yi < imagesYSort[3].Y && imagesYSort[2].Y + imagesYSort[2].Height < imagesYSort[3].Y) {
                    return String.Format("({0}-{3}):{1}:{2}", block.Images[0],
                        block.Images[1],
                        block.Images[2],
                        block.Images[3]);
                }
                else
                {
                    Console.WriteLine(gardiners);
                    Debug.Assert(true == true, "UNSUPPORTED 4 BLOCK GLYPH ");
                }


            }
            else
            {
                return String.Format("({0}-{1}):({2}-{3})", gardiners[0],
                                gardiners[1],
                                gardiners[2],
                                gardiners[3]);
            }


            /*
             * If images in block have similar y-values and different x-values:
             *     - Return gardiner IDs joined by '*'
             *     - First is gardiners[0], second is gardiners[1]
             * If images in block have similar x-values and different y-values:
             *     - Return gardiner IDs joined by ':'
             *     - First is gardiners[0], second is gardiners[1]
             */
            throw new NotImplementedException();
        }

        private static string TwoGlyphCase(GlyphBlock block, string[] gardiners)
        {
            ImageData firstImage = block.Images[0];
            ImageData secondImage = block.Images[1];

            if (Math.Abs( firstImage.Y - secondImage.Y) < 0.5)
            {
                return gardiners[0] + "-" + gardiners[1];
            }
            else
            {
                return gardiners[0] + ":" + gardiners[1];
            }

            /*
             * If images in block have similar y-values and different x-values:
             *     - Return gardiner IDs joined by '*'
             *     - First is gardiners[0], second is gardiners[1]
             * If images in block have similar x-values and different y-values:
             *     - Return gardiner IDs joined by ':'
             *     - First is gardiners[0], second is gardiners[1]
             */
            throw new NotImplementedException();
        }

        private static string ThreeGlyphCase(GlyphBlock block, string[] gardiners)
        {
            ImageData firstImage = block.Images[0];
            ImageData secondImage = block.Images[1];
            ImageData thirdImage = block.Images[2];

            //imageList.OrderBy(image => image.Bounds.X).ToList();

            List<ImageData> imagesYSort = block.Images.OrderBy(image => image.Y).ToList();
            //Three Glyphs all above each other
            if (imagesYSort[0].Y + imagesYSort[0].Height < imagesYSort[1].Y && imagesYSort[1].Y + imagesYSort[1].Height < imagesYSort[2].Y)
            {
                return String.Format("{0}:{1}:{2}", gardiners[0], gardiners[1], gardiners[2]);

            }

            //First Image beside second image
            if (Math.Abs(firstImage.Y - secondImage.Y) < 0.5)
            {
                return String.Format("{0}-({1}:{2})", gardiners[0], gardiners[1], gardiners[2]);
            }
            //second image below first image
            else if (firstImage.Y < secondImage.Y)
            {
                //Third Image beside second image, second Image below first image
                if (Math.Abs(secondImage.Y - thirdImage.Y) < 0.5)
                {
                    //first image above ONLY second image
                    if(firstImage.X+firstImage.Width < thirdImage.X)
                    {
                        return String.Format("({0}:{1})-{2}", gardiners[0], gardiners[1], gardiners[2]);

                    }
                    //First Image stretched and above second and third Image
                    else
                    {
                        return String.Format("{0}:({1}-{2})", gardiners[0], gardiners[1], gardiners[2]);

                    }
                }
                //Third Image is above the second Image.
                else
                {
                    //First and third Image beside each other, Second Image below ONLY First Image
                    if (secondImage.X + secondImage.Width < thirdImage.X)
                    {
                        return String.Format("({0}:{1})-{2}", gardiners[0], gardiners[1], gardiners[2]);

                    }
                    //Second Image is below first and third Image. First and third Image are beside each other
                    else
                    {
                        return String.Format("({0}-{2}):{1}", firstImage.GardinerSign, secondImage.GardinerSign, thirdImage.GardinerSign);
                    }
                }
            }
            //First Image is below second Image
            else if (firstImage.Y > secondImage.Y)
            {
                //Third Image beside second image, second Image below first image
                if (Math.Abs(firstImage.Y - thirdImage.Y) < 0.5)
                {
                    //first image above ONLY second image
                    if (secondImage.X + secondImage.Width < thirdImage.X)
                    {
                        return String.Format("({1}:{0})-{2}", firstImage.GardinerSign, secondImage.GardinerSign, thirdImage.GardinerSign);

                    }
                    //First Image stretched and above second and third Image
                    else
                    {
                        return String.Format("{1}:({0}-{2})", firstImage.GardinerSign, secondImage.GardinerSign, thirdImage.GardinerSign);

                    }
                }
                //Third Image is above the second Image.
                else
                {
                    //First and third Image beside each other, Second Image below ONLY First Image
                    if (firstImage.X + firstImage.Width < thirdImage.X)
                    {
                        return String.Format("({1}:{0})-{2}", firstImage.GardinerSign, secondImage.GardinerSign, thirdImage.GardinerSign);

                    }
                    //Second Image is below first and third Image. First and third Image are beside each other
                    else
                    {
                        return String.Format("({1}-{2}):{0}", firstImage.GardinerSign, secondImage.GardinerSign, thirdImage.GardinerSign);

                    }
                }
            }
            /*
             * If 2 of the 3 images have similar x-values and different y-values
             *      - Take 2 with similar x-values and String.Join with ':', then add parentheses around
             *      - If outlier image's gardiner == gardiners[0], return string 0-(1:2) formatted with gardiner indexes 0..2
             *      - Else if outlier image's gardiner == gardiners[-1], return string (0:1)-2 formatted with gardiner indexes 0..2
             *      
             * If 2 of the 3 images have similar y-values and different x-values:
             *      - Take 2 with similar y-values and String.Join with '-', then add parentheses around
             *      - If outlier image's gardiner == gardiners[0], return string 0:(1-2) formatted with gardiner indexes 0..2
             *      - Else if outlier image's gardiner == gardiners[-1], return string (0-1):2 formatted with gardiner indexes 0..2
             */
            throw new NotImplementedException();
        }
    }
}
