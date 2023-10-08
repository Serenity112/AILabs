using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MathLib;

namespace AILabs.HammingNetwork
{
    public class ImageUtils
    {
        public static NumericVector ConvertImageToBinaryVector(Bitmap image)
        {
            NumericVector vector = new NumericVector();

            int width = image.Width;
            int height = image.Height;

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    Color pixelColor = image.GetPixel(i, j);
                    int brightness = (int)((pixelColor.R + pixelColor.G + pixelColor.B) / 3.0);
                    int pixelValue = (brightness >= 128) ? -1 : 1;
                    vector.Append(pixelValue);
                }
            }

            return vector;
        }

        public static Bitmap EnlargeImage(Bitmap originalImage, int scaleFactor)
        {
            int originalWidth = originalImage.Width;
            int originalHeight = originalImage.Height;

            int newWidth = originalWidth * scaleFactor;
            int newHeight = originalHeight * scaleFactor;

            Bitmap enlargedPixelArtImage = new Bitmap(newWidth, newHeight);

            for (int x = 0; x < originalWidth; x++)
            {
                for (int y = 0; y < originalHeight; y++)
                {
                    Color pixelColor = originalImage.GetPixel(x, y);

                    for (int i = 0; i < scaleFactor; i++)
                    {
                        for (int j = 0; j < scaleFactor; j++)
                        {
                            int newX = x * scaleFactor + i;
                            int newY = y * scaleFactor + j;


                            enlargedPixelArtImage.SetPixel(newX, newY, pixelColor);
                        }
                    }
                }
            }

            return enlargedPixelArtImage;
        }
    }
}
