using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AILabs.FuzzyLogic.FuzzyRobot;

namespace AILabs.FuzzyLogic
{
    public class Defuzzification
    {
        private enum BorderPosition
        {
            Top,
            MiddleTop,
            Middle,
            MiddleZero,
            Zero
        }

        private double accuracyStep = 1;

        private int a;
        private int b;
        private int c;
        private int left;
        private int right;

        private (double x, double y) t1;
        private (double x, double y) t2;

        private List<Func<double, double>> _trgB = new();

        public Defuzzification(int left, int a, int b, int c, int right)
        {
            this.a = a;
            this.b = b;
            this.c = c;
            this.left = left;
            this.right = right;



            // Первый треугольник
            _trgB.Add((x) => 1 - (a - x) / (a - left));
            _trgB.Add((x) => 1 - (x - a) / (b - a));

            // Второй треугольник
            _trgB.Add((x) => 1 - (b - x) / (b - a));
            _trgB.Add((x) => 1 - (x - b) / (c - b));

            // Третий треугольник
            _trgB.Add((x) => 1 - (c - x) / (c - b));
            _trgB.Add((x) => 1 - (x - c) / (right - c));


            double t1_x = (b + a) / 2;
            t1 = (t1_x, _trgB[1](t1_x));

            double t2_x = (c + b) / 2;
            t2 = (t2_x, _trgB[3](t2_x));
        }

        private bool RoundedEqual(double n1, double n2)
        {
            return Math.Round(n1, 3) == Math.Round(n2, 3);
        }

        private Dictionary<double, double> FormSurface(List<double> borders)
        {
            Dictionary<int, BorderPosition> bordersPositions = new Dictionary<int, BorderPosition>();
            for (int i = 0; i < 3; i++)
            {
                double br = borders[i];

                if (br == 1)
                {
                    bordersPositions[i] = BorderPosition.Top;
                    continue;
                }
                if (br == 1)
                {
                    bordersPositions[i] = BorderPosition.Top;
                    continue;
                }
                if (br == 1)
                {
                    bordersPositions[i] = BorderPosition.Top;
                    continue;
                }
                if (br == 1)
                {
                    bordersPositions[i] = BorderPosition.Top;
                    continue;
                }


            }


            List<Func<double, double>> f_borders = new()
            {
                (x) => borders[0],
                (x) => borders[1],
                (x) => borders[2],
            };


            var surface = new Dictionary<double, double>();

            int triangularState = 0;

            // Начальный угол
            double x = -30;
            // L -> t0

            // 1 треугольник
            // Поднимаемся до 1го бордера
            var fi1 = new FIntersection(_trgB[0], f_borders[0], x);
            while (true)
            {
                var intersection = fi1.GetIntersection();
                surface.Add(x, intersection.y_value);
                if (intersection.intersection)
                {
                    break;
                }

                x += accuracyStep;
                fi1.Iteration(x);
            }

            // Выбор функции для перехода от конца 1го бордера в t1
            var f1 = (borders[0] >= t1.y) ? _trgB[1] : _trgB[2];

            // Идём по 1му бордеру до функции f2
            double temp_f1 = f1(x);
            while (true)
            {
                if (Math.Abs(borders[0] - f1(x)) < accuracyStep)
                {
                    break;
                }

                surface.Add(x, borders[0]);
                x += accuracyStep;
            }

            // Доходим до t1
            while (true)
            {
                double y = f1(x);
                if (Math.Abs(t1.y - y) < accuracyStep)
                {
                    break;
                }

                surface.Add(x, y);
                x += accuracyStep;
            }

            // 2 треугольник1
            var f2 = (borders[1] >= t1.y) ? _trgB[2] : _trgB[1];
            //Идём до 2го бордера
            while (true)
            {
                double y = f2(x);

                if (y >= borders[0])
                {
                    break;
                }

                surface.Add(x, y);
                x += accuracyStep;
            }




            return surface;
        }

        public double Deffuzzify(Dictionary<RotAction, double> input)
        {
            List<double> borders = new List<double>()
            {
                input[RotAction.RotLeft],
                input[RotAction.RotNone],
                input[RotAction.RotRight]
            };

            var surface = FormSurface(borders);

            List<Func<double, double>> _flatBorders = new()
            {
                (x) => (input[RotAction.RotNone]),
                (x) => (x),
                (x) => (x),
            };


            Func<double, double> f = (x) => x - 3;




            return input[RotAction.RotLeft] * (-30) + input[RotAction.RotNone] * 0 + input[RotAction.RotRight] * 30;





            /*if ((input[RotAction.RotNone] > input[RotAction.RotLeft]) && (input[RotAction.RotNone] > input[RotAction.RotRight]))
            {
                return 0;
            }

            if ((input[RotAction.RotLeft] > input[RotAction.RotRight]) && (input[RotAction.RotLeft] > input[RotAction.RotNone]))
            {
                return -30;
            }

            return 30;*/
        }

        //private
    }

    public class FIntersection
    {
        private Func<double, double> _f1;
        private Func<double, double> _f2;

        private double _currentYdiff;
        private double _currentX;
        private bool _intersectionReached = false;

        public FIntersection(Func<double, double> f1, Func<double, double> f2, double initialX)
        {
            _f1 = f1;
            _f2 = f2;
            _currentX = initialX;
            _currentYdiff = Math.Abs(_f1(initialX) - _f2(initialX));
        }

        public void Iteration(double x)
        {
            double diff = Math.Abs(_f1(x) - _f2(x));
            if (diff < _currentYdiff && !_intersectionReached)
            {
                _currentYdiff = diff;
                _currentX = x;
            }
            else
            {
                _intersectionReached = true;
            }
        }

        public (double y_value, bool intersection) GetIntersection()
        {
            return (_f1(_currentX), _intersectionReached);
        }
    }
}
