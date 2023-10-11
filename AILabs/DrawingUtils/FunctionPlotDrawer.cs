using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AILabs.DrawingUtils
{
    public class FunctionPlotDrawer
    {
        private static int contourCount = 8;

        private static Color _minColor = Color.FromArgb(102, 0, 204);
        private static Color _maxColor = Color.FromArgb(255, 0, 0);

        public static Bitmap ContourPlotter(PictureBox pictureBox, Func<double, double, double> func, (double x, double y) center, double interval)
        {
            int width = pictureBox.Width;
            int height = pictureBox.Height;

            Bitmap bitmap = new Bitmap(width, height);

            double minVal = func(0, 0);
            double maxVal = minVal;

            double searchStep = 0.01;
            for (double x_i = center.x - interval; x_i < center.x + interval; x_i += searchStep)
            {
                for (double y_j = center.y - interval; y_j < center.y + interval; y_j += searchStep)
                {
                    double value = func(x_i, y_j);

                    if (value > maxVal)
                    {
                        maxVal = value;
                    }

                    if (value < minVal)
                    {
                        minVal = value;
                    }
                }
            }

            double drawStep = 1.0; 
            for (double i = 0; i < width; i += drawStep)
            {
                for (double j = 0; j < height; j += drawStep)
                {
                    double f_x = (i / width) * (2 * interval) + (-interval);
                    double f_y = (j / height) * (2 * interval) + (-interval);
                    double value = func(f_x, f_y);
                    double ratio = (value - minVal) / (maxVal - minVal);
                    
                    if (ratio >= 0)
                    {
                        Color newColor = InterpolateColor(ratio);
                        bitmap.SetPixel((int)i, (int)j, newColor);
                        //textBox.Text += "ratio: " + ratio;
                    }
                    else
                    {

                    }



                }
            }

            return bitmap;
        }

        private static Color InterpolateColor(double ratio)
        {
            int interpolatedR = (int)(_minColor.R + (_maxColor.R - _minColor.R) * ratio);
            int interpolatedG = (int)(_minColor.G + (_maxColor.G - _minColor.G) * ratio);
            int interpolatedB = (int)(_minColor.B + (_maxColor.B - _minColor.B) * ratio);
            double r = ratio;
            return Color.FromArgb(interpolatedR, interpolatedG, interpolatedB);
        }
    }
}
