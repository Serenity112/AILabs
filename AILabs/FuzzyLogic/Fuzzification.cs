using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AILabs.FuzzyLogic
{
    public enum Range
    {
        Close,
        Medium,
        Far,
    }

    public class Fuzzification
    {
        private int a;
        private int b;
        private int c;
        private int d;

        public Fuzzification(int a, int b, int c, int d)
        {
            this.a = a;
            this.b = b;
            this.c = c;
            this.d = d;
        }

        public Dictionary<Range, double> Fuzzify(double distance)
        {
            return new Dictionary<Range, double>()
            {
                 { Range.Close, FuncClose(distance) },
                 { Range.Medium, FuncMedium(distance) },
                 { Range.Far, FuncFar(distance) },
            };
        }

        // Левая трапеция
        private double FuncClose(double x)
        {
            if (x < a)
            {
                return 1;
            }

            if (x > a && x < b)
            {
                return 1 - (x - a) / (b - a);
            }

            return 0;
        }

        // центральная трапеция
        private double FuncMedium(double x)
        {
            if (a <= x && x <= b)
            {
                return 1 - (b - x) / (b - a);
            }

            if (b <= x && x <= c)
            {
                return 1;
            }

            if (c <= x && x <= d)
            {
                return 1 - (x - c) / (d - c);
            }

            if (x < a || x > d)
            {
                return 0;
            }

            return 0;
        }

        // Правая трапеция
        private double FuncFar(double x)
        {
            if (x < c)
            {
                return 0;
            }

            if (x >= c && x <= d)
            {
                return 1 - (d - x) / (d - c);
            }

            return 1;
        }
    }
}
