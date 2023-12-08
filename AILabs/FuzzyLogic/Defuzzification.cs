using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AILabs.FuzzyLogic.FuzzyRobot;

namespace AILabs.FuzzyLogic
{
    public class FuzzyTriangle
    {
        private Func<double, double> leftEdge;
        private Func<double, double> rightEdge;
        private double a;
        private double b;
        private double c;

        public FuzzyTriangle(double a, double b, double c)
        {
            this.a = a;
            this.b = b;
            this.c = c;

            leftEdge = (x) => 1 - (b - x) / (b - a);
            rightEdge = (x) => 1 - (x - b) / (c - b);
        }

        public double GetValue(double x, double topBorder)
        {
            if (x >= a && x <= b)
            {
                double y = leftEdge(x);
                return y < topBorder ? y : topBorder;
            }

            if (x >= b && x <= c)
            {
                double y = rightEdge(x);
                return y < topBorder ? y : topBorder;
            }

            return 0;
        }
    }

    public class Defuzzification
    {
        private double _accuracyStep = 1;

        private List<FuzzyTriangle> _triangles = new List<FuzzyTriangle>();

        public Defuzzification(double left, double a, double b, double c, double right)
        {
            _triangles.Add(new FuzzyTriangle(left, a, b));
            _triangles.Add(new FuzzyTriangle(a, b, c));
            _triangles.Add(new FuzzyTriangle(b, c, right));
        }

        public double Deffuzzify(Dictionary<RotAction, double> input)
        {
            List<double> borders = new List<double>()
            {
                input[RotAction.RotLeft],
                input[RotAction.RotNone],
                input[RotAction.RotRight]
            };

            double sum1 = 0;
            double sum2 = 0;

            for (double x = -30; x < 30; x += _accuracyStep)
            {
                double tr0 = _triangles[0].GetValue(x, borders[0]);
                double tr1 = _triangles[1].GetValue(x, borders[1]);
                double tr2 = _triangles[2].GetValue(x, borders[2]);

                double value = Math.Max(tr0, Math.Max(tr1, tr2));

                sum1 += x * value;
                sum2 += value;
            }

            double gravityCenter = sum1 / sum2;

            return gravityCenter;
        }
    }
}
