using AILabs.DrawingUtils;
using MathLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AILabs.Swarm
{
    public class SwarmMethod
    {
        public struct ParticleData
        {
            public ParticleData(Vector currentPoint, Vector speed, Vector bestExtremum)
            {
                CurrentPoint = currentPoint;
                Speed = speed;
                BestExtremum = bestExtremum;
            }

            public Vector CurrentPoint;
            public Vector Speed;
            public Vector BestExtremum;
        }

        private List<ParticleData> _particlesData;

        private double _speed = 1;
        private Vector _globalExtremum = new Vector(10.0, 10.0);

        private PointF _lowerBorder = new PointF(-10.0F, -10.0F);
        private PointF _topBorder = new PointF(10.0F, 10.0F);

        private double _minVelocity = -0.1;
        private double _maxVelocity = 0.1;

        private Func<double, double, double> _function;

        private Random _random;

        public SwarmMethod(Func<double, double, double> func, int particlesCount)
        {
            _random = new Random(Guid.NewGuid().GetHashCode());
            _function = func;
            _particlesData = new List<ParticleData>();
            RandomInitialization(particlesCount);
        }

        private void RandomInitialization(int particlesCount)
        {
            for (int i = 0; i < particlesCount; i++)
            {
                double x = _random.NextDouble() * (_topBorder.X - _lowerBorder.X) + _lowerBorder.X;
                double y = _random.NextDouble() * (_topBorder.Y - _lowerBorder.Y) + _lowerBorder.Y;
                Vector initialPoint = new Vector(x, y);

                double xv = _random.NextDouble() * (_maxVelocity - _minVelocity) + _minVelocity;
                double yv = _random.NextDouble() * (_maxVelocity - _minVelocity) + _minVelocity;
                Vector vector = new Vector(xv, yv);

                _particlesData.Add(new ParticleData(initialPoint, vector, initialPoint));

                if (_function(x, y) < _function(_globalExtremum.Dx, _globalExtremum.Dy))
                {
                    _globalExtremum = new Vector(x, y);
                }
            }
        }

        public (List<ParticleData> data, Vector extremum) FindMinimum()
        {
            Vector _prevExtremum = _globalExtremum.Copy();

            int counter = 10;

            while (counter > 0)
            {
                var iteration = SingleIteration();

                if (Math.Abs(_function(_prevExtremum.Dx, _prevExtremum.Dy) - _function(_globalExtremum.Dx, _globalExtremum.Dy)) < 0.01)
                {
                    counter--;
                }
                else
                {
                    counter = 10;
                }
            }

            return (_particlesData, _globalExtremum);
        }

        public (List<ParticleData> data, Vector extremum) SingleIteration()
        {
            for (int i = 0; i < _particlesData.Count; i++)
            {
                ParticleData data = _particlesData[i];

                Vector newSpeed = data.CurrentPoint +
                    _random.NextDouble() * (data.BestExtremum - data.CurrentPoint) +
                    _random.NextDouble() * (_globalExtremum - data.CurrentPoint);

                Vector newPoint = data.CurrentPoint + _speed * newSpeed;

                if (_function(newPoint.Dx, newPoint.Dy) < _function(data.BestExtremum.Dx, data.BestExtremum.Dy))
                {
                    Vector newExtremum = new Vector(newPoint.Dx, newPoint.Dy);
                    data.BestExtremum = newExtremum;

                    if (_function(data.BestExtremum.Dx, data.BestExtremum.Dy) < _function(_globalExtremum.Dx, _globalExtremum.Dy))
                    {
                        _globalExtremum = data.BestExtremum;
                    }
                }
            }

            return (_particlesData, _globalExtremum);
        }
    }
}
