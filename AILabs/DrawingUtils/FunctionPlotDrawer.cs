using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AILabs.DrawingUtils
{
    public class Function
    {
        public Function((double x, double y) parameters)
        {

        }
    }

    public class FunctionPlotDrawer
    {
        private double Func(double x, double y)
        {
            return x * x + 3 * y * y + 2 * x * y;
        }

        public FunctionPlotDrawer(PictureBox pictureBox)
        {
            //Function func = new Function((double x, double y) => x + y);
        }
    }
}
