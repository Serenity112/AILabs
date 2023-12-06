using AILabs.FuzzyLogic.Map;
using MathLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AILabs.FuzzyLogic.FuzzyRobot;

namespace AILabs.FuzzyLogic
{
    public class FuzzyRobot
    {
        public enum RotAction
        {
            RotNone,
            RotLeft,
            RotRight,
        }

        public enum RobotVision
        {
            Straight,
            Left,
            Right,
        }

        private double _raysAngle;

        public PointF CentroidGlobalPosition { get; private set; }
        public PointF LeftTopGlobalPosition { get; private set; }

        private double _robotSize;

        private double _curentSpeed = 3;
        private double _visionAngle;

        private Brush _robotBrush = new SolidBrush(Color.Red);
        private Pen _pen = new Pen(Color.Blue, 2);

        private Fuzzification _fuzzification;
        private Defuzzification _defuzzification;

        public FuzzyRobot(double raysAngle,
            double robotSize, int tileSize, double speed)
        {
            _raysAngle = raysAngle;
            _robotSize = robotSize;
            _curentSpeed = speed;

            int a = (int)(_robotSize * 0.45);
            int b = (int)(_robotSize * 0.6);
            int c = (int)(_robotSize * 1.5);
            int d = (int)(_robotSize * 2.0);

            _fuzzification = new Fuzzification(a, b, c, d);

            double tAngle = 15;
            _defuzzification = new Defuzzification(-tAngle, -tAngle / 2, 0, tAngle / 2, tAngle);
        }

        public double GetCurrentVisionAngle()
        {
            return _visionAngle;
        }


        public void SetRandomStartPosition(SurfaceMap sMap)
        {
            Random random = new Random();

            List<IMapTile> avaliableTiles = new List<IMapTile>();

            foreach (var tile in sMap.GetNextTile())
            {
                if (!tile.CollidesWithPoint(tile.LeftTop))
                {
                    avaliableTiles.Add(tile);
                }
            }

            IMapTile startingTile = avaliableTiles[random.Next(avaliableTiles.Count)];

            CentroidGlobalPosition = new PointF()
            {
                X = startingTile.LeftTop.X + startingTile.Width / 2,
                Y = startingTile.LeftTop.Y + startingTile.Height / 2
            };

            LeftTopGlobalPosition = new PointF()
            {
                X = CentroidGlobalPosition.X - (float)_robotSize / 2,
                Y = CentroidGlobalPosition.Y - (float)_robotSize / 2,
            };

            _visionAngle = random.NextDouble() * Math.PI * 2;

            return;
        }

        public void Move()
        {
            Vector direction = new Vector(
                        CentroidGlobalPosition,
                        PointByCenter(CentroidGlobalPosition, 1f, (float)_visionAngle));
            CentroidGlobalPosition += (direction * _curentSpeed);
            LeftTopGlobalPosition += (direction * _curentSpeed);
        }

        public void Rotate(double degreesAngle)
        {
            double radians = degreesAngle * Math.PI / 180;
            _visionAngle += radians;
        }

        public (PointF[] Points, Dictionary<RobotVision, double> Distances) RayTraceDirections(SurfaceMap sMap)
        {
            int irsize = (int)_robotSize;

            PointF[] rayTracePoints = new PointF[3]
            {
                PointByCenter(CentroidGlobalPosition, irsize / 2, (float)(_visionAngle - _raysAngle)),
                PointByCenter(CentroidGlobalPosition, irsize / 2, (float)_visionAngle),
                PointByCenter(CentroidGlobalPosition, irsize / 2, (float)(_visionAngle + _raysAngle))
            };

            PointF[] points = new PointF[3];
            double[] res = new double[3];

            double rayTraceStep = 0.5f;
            for (int p = 0; p < 3; p++)
            {
                PointF newStepPoint = new PointF(rayTracePoints[p].X, rayTracePoints[p].Y);
                Vector ray = new Vector(CentroidGlobalPosition, newStepPoint).Normalized() * rayTraceStep;

                bool exit = false;
                while (!exit)
                {
                    newStepPoint = newStepPoint + ray;

                    if (!sMap.IfPointInsideMap(newStepPoint))
                    {
                        break;
                    }

                    foreach (var tile in sMap.GetNextTile())
                    {
                        if (tile.CollidesWithPoint(newStepPoint))
                        {
                            exit = true;
                            break;
                        }
                    }
                }

                res[p] = new Vector(rayTracePoints[p], newStepPoint).Length();
                points[p] = newStepPoint;
            }

            return (points, new Dictionary<RobotVision, double>()
            {
                { RobotVision.Left, res[0]},
                { RobotVision.Straight, res[1]},
                { RobotVision.Right, res[2]},
            });
        }

        public Dictionary<RobotVision, Dictionary<Range, double>> Fuzzify(Dictionary<RobotVision, double> distances)
        {
            return new Dictionary<RobotVision, Dictionary<Range, double>>(){
                { RobotVision.Left, _fuzzification.Fuzzify(distances[RobotVision.Left]) },
                { RobotVision.Straight, _fuzzification.Fuzzify(distances[RobotVision.Straight]) },
                { RobotVision.Right, _fuzzification.Fuzzify(distances[RobotVision.Right]) },
            };
        }

        public double GetDeffusedValue(Dictionary<RotAction, double> input)
        {
            return _defuzzification.Deffuzzify(input);
        }

        public Dictionary<RotAction, double> FuzzyRules(Dictionary<RobotVision, Dictionary<Range, double>> input)
        {
            // Спереди
            double strClose = input[RobotVision.Straight][Range.Close];
            double strMedium = input[RobotVision.Straight][Range.Medium];
            double strFar = input[RobotVision.Straight][Range.Far];

            // Слева
            double leftClose = input[RobotVision.Left][Range.Close];
            double leftMedium = input[RobotVision.Left][Range.Medium];
            double leftFar = input[RobotVision.Left][Range.Far];

            // Справа близко  
            double rightClose = input[RobotVision.Right][Range.Close];
            double rightMedium = input[RobotVision.Right][Range.Medium];
            double rightFar = input[RobotVision.Right][Range.Far];

            // Везде близко
            double allClose = FuzzyLogicMath.AND(strClose, rightClose, leftClose);


            // --------------------------------------------------
            // Поворот 0й
            // Спереди далеко или средне
            double r_0_1 = FuzzyLogicMath.OR(strMedium, strFar);

            KeyValuePair<RotAction, double> r_0 = new(RotAction.RotNone, r_0_1);


            // --------------------------------------------------
            // Поворот влево
            // Спереди близко И (слева средне или слева далеко)
            double r_1_1 = FuzzyLogicMath.AND(strClose, FuzzyLogicMath.OR(leftMedium, leftFar));

            // ИЛИ всё близко ИЛИ спарва близко
            KeyValuePair<RotAction, double> r_1 = new(RotAction.RotLeft, FuzzyLogicMath.OR(r_1_1, allClose, rightClose));

            // --------------------------------------------------
            // Поворот вправо

            // ((Спереди близко И (справа средне или справа далеко)) ИЛИ слева близко)
            double r_2_1 = FuzzyLogicMath.OR(FuzzyLogicMath.AND(strClose, FuzzyLogicMath.OR(rightMedium, rightFar)), leftClose);

            // Не всё близко
            double r_2_2 = FuzzyLogicMath.NOT(allClose);

            KeyValuePair<RotAction, double> r_2 = new(RotAction.RotRight, FuzzyLogicMath.AND(r_2_1, r_2_2));


            return new[]
            {
                r_1,
                r_0,
                r_2
            }.ToDictionary(rule => rule.Key, rule => rule.Value);
        }


        // Отрисовка
        public Bitmap DrawRobot()
        {
            int irsize = (int)_robotSize;
            Bitmap bitmap = new Bitmap(irsize, irsize);
            bitmap.MakeTransparent();

            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                PointF zero = new PointF(0, 0);
                RectangleF border = new RectangleF(zero, new SizeF(irsize, irsize));
                graphics.FillEllipse(_robotBrush, border);

                PointF localCenter = new PointF(irsize / 2, irsize / 2);
                PointF vision = PointByCenter(localCenter, irsize / 2, (float)_visionAngle);
                graphics.DrawLine(_pen, localCenter, vision);
            }
            return bitmap;
        }

        private PointF PointByCenter(PointF center, float radius, float angle)
        {
            float x = center.X + radius * (float)Math.Cos(angle);
            float y = center.Y + radius * (float)Math.Sin(angle);
            return new PointF(x, y);
        }
    }
}
