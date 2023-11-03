using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AILabs.DrawingUtils
{
    public enum DrawingMode
    {
        Contour,
        Gradient
    }

    public class FunctionPlotDrawer
    {
        private static Color _minColor = Color.FromArgb(102, 0, 204);
        private static Color _maxColor = Color.FromArgb(255, 0, 0);
        private static int _contourCount = 10;

        public static Bitmap ContourPlotter(PictureBox pictureBox, Func<double, double, double> func,
            (double x, double y) left_bottom, (double x, double y) right_top, DrawingMode drawingMode)
        {
            (double x, double y) center = ((right_top.x + left_bottom.x) / 2, (right_top.y + left_bottom.y) / 2);

            int width = pictureBox.Width;
            int height = pictureBox.Height;

            Bitmap bitmap = new Bitmap(width, height);

            double minVal = func(center.x, center.y);
            double maxVal = minVal;

            double x_interval = right_top.x - left_bottom.x;
            double y_interval = right_top.y - left_bottom.y;

            // Поиск минимума и максимума значения функции
            double searchStep = 0.01;
            for (double x_i = left_bottom.x; x_i < right_top.x; x_i += searchStep)
            {
                for (double y_j = left_bottom.y; y_j < right_top.y; y_j += searchStep)
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

            // Отрисовка
            double drawStep = 1.0;
            for (double i = 0; i < width; i += drawStep)
            {
                for (double j = 0; j < height; j += drawStep)
                {
                    double f_x = (i / width) * x_interval + left_bottom.x;
                    double f_y = (j / height) * y_interval + left_bottom.y;
                    double value = func(f_x, f_y);
                    double ratio = (value - minVal) / (maxVal - minVal);

                    switch (drawingMode)
                    {
                        case DrawingMode.Contour:
                            double step = Math.Round(1.0 / _contourCount, 2);
                            for (double k = step; k <= 1; k += step)
                            {
                                if (ratio <= k)
                                {
                                    ratio = k;
                                    break;
                                }
                            }
                            break;
                        case DrawingMode.Gradient:
                            break;
                    }

                    Color newColor = InterpolateColor(ratio);
                    bitmap.SetPixel((int)(i), (int)(j), newColor);
                }
            }

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
