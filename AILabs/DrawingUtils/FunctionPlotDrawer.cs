using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AILabs.DrawingUtils
{
    public class FunctionPlotDrawer
    {
        private static int interval = 10;

        private static Color _minColor = Color.FromArgb(40, 27, 149);
        private static Color _maxColor = Color.FromArgb(255, 102, 102);

        public static Bitmap DrawBitmap(PictureBox pictureBox, Func<double, double, double> func, (double x, double y) minimumCoords)
        {
            int width = pictureBox.Width;
            int height = pictureBox.Height;

            Bitmap bitmap = new Bitmap(width, height);



            double minimum = func(minimumCoords.x, minimumCoords.y);
            (double x, double y) maximumCoords = minimumCoords;
            double maximum = minimum;

            double searchStep = 0.01;
            for (double i = minimumCoords.x - interval; i < minimumCoords.x + interval; i += searchStep)
            {
                for (double j = minimumCoords.y - interval; j < minimumCoords.y + interval; j += searchStep)
                {
                    if (func(i, j) > maximum)
                    {
                        maximum = func(i, j);
                        maximumCoords = (i, j);
                    }
                }
            }

            double valuesInterval = Math.Abs(maximum - minimum);
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    /* double f_x = 2 * interval 
                     double f_y =*/
                    //(func(f_x, f_y) + Math.Abs(minimum)) / valuesInterval
                    Color newColor = InterpolateColor((i + j) / 1024.0);
                    bitmap.SetPixel(i, j, newColor);
                }
            }

            //Function func = new Function((double x, double y) => x + y);


            return bitmap;
        }

        private static Color InterpolateColor(double ratio)
        {
            int interpolatedR = (int)(_minColor.R + (_maxColor.R - _minColor.R) * ratio);
            int interpolatedG = (int)(_minColor.G + (_maxColor.G - _minColor.G) * ratio);
            int interpolatedB = (int)(_minColor.B + (_maxColor.B - _minColor.B) * ratio);

            return Color.FromArgb(interpolatedR, interpolatedG, interpolatedB);
        }
    }
}
