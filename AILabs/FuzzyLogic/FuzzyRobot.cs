using AILabs.FuzzyLogic.Map;
using MathLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public readonly int RaysCount = 3;
        private double _raysAngle;

        public PointF CentroidGlobalPosition { get; private set; }
        public PointF LeftTopGlobalPosition { get; private set; }

        private double _robotSize;

        private double _curentSpeed = 3;
        private double VisionAngle;

        private Brush _robotBrush = new SolidBrush(Color.Red);
        private Pen _pen = new Pen(Color.Blue, 2);

        private Fuzzification _fuzzification;
        private Defuzzification _defuzzification;

        public FuzzyRobot(double raysAngle,
            double robotSize, int tileSize)
        {
            _raysAngle = raysAngle;
            _robotSize = robotSize;

            int a = (int)(tileSize * 0.2);
            int b = (int)(tileSize * 0.4);
            int c = (int)(tileSize * 1.5);
            int d = (int)(tileSize * 2.0);
            _fuzzification = new Fuzzification(a, b, c, d);

            _defuzzification = new Defuzzification(-30, -20, 0, 20, 30);
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

            VisionAngle = random.NextDouble() * Math.PI * 2;

            return;
        }

        public void Move()
        {
            Vector direction = new Vector(
                        CentroidGlobalPosition,
                        PointByCenter(CentroidGlobalPosition, 1f, (float)VisionAngle));
            CentroidGlobalPosition += (direction * _curentSpeed);
            LeftTopGlobalPosition += (direction * _curentSpeed);
        }

        public void Rotate(double angle)
        {
            VisionAngle += angle;
        }

        public Dictionary<RobotVision, double> RayTraceDirections(SurfaceMap sMap)
        {
            int irsize = (int)_robotSize;

            PointF[] rayTracePoints = new PointF[3]
            {
                PointByCenter(CentroidGlobalPosition, irsize / 2, (float)(VisionAngle - _raysAngle)),
                PointByCenter(CentroidGlobalPosition, irsize / 2, (float)VisionAngle),
                PointByCenter(CentroidGlobalPosition, irsize / 2, (float)(VisionAngle + _raysAngle))
            };

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
            }

            return new Dictionary<RobotVision, double>()
            {
                { RobotVision.Left, res[0]},
                { RobotVision.Straight, res[1]},
                { RobotVision.Right, res[2]},
            };
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
            // Если ((спереди близко) И (слева средне ИЛИ слева далеко)) ИЛИ (везде близко) - поворот влево
            KeyValuePair<RotAction, double> rule1 = new(RotAction.RotLeft,
                FuzzyLogicMath.OR(
                    // Везде близко
                    FuzzyLogicMath.AND(
                        input[RobotVision.Straight][Range.Close],
                        input[RobotVision.Left][Range.Close],
                        input[RobotVision.Right][Range.Close]),
                    // (спереди близко) И (слева средне ИЛИ далеко)
                    FuzzyLogicMath.AND(
                        input[RobotVision.Straight][Range.Close],
                        FuzzyLogicMath.OR(
                            input[RobotVision.Left][Range.Medium],
                            input[RobotVision.Left][Range.Far]))));

            // Если (спереди далеко) ИЛИ (средне) - нет поворота
            KeyValuePair<RotAction, double> rule2 = new(RotAction.RotNone,
                FuzzyLogicMath.OR(
                    input[RobotVision.Straight][Range.Far],
                    input[RobotVision.Straight][Range.Medium]));

            // Если (спереди близко) И (справа средне ИЛИ справа далеко) - поворот вправо
            KeyValuePair<RotAction, double> rule3 = new(RotAction.RotRight,
                FuzzyLogicMath.AND(
                        input[RobotVision.Straight][Range.Close],
                        FuzzyLogicMath.OR(
                            input[RobotVision.Right][Range.Medium],
                            input[RobotVision.Right][Range.Far])));


            // Если поворот вправо и влево равнозачен, то берём в приоритет поворот влево
            /* if (rule2.Value == rule3.Value)
             {
                 rule3 = new(rule3.Key, 0);
             }*/


            return new[]
            {
                rule1,
                rule2,
                rule3
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
                PointF vision = PointByCenter(localCenter, irsize / 2, (float)VisionAngle);
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
