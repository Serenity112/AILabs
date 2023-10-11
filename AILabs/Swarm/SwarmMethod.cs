using MathLib;

namespace AILabs.Swarm
{
    public struct ParticleData
    {
        public ParticleData(Vector currentPoint, Vector speed, Vector bestExtremum)
        {
            CurrentPoint = currentPoint;
            Speed = speed;
            BestExtr = bestExtremum;
        }

        public Vector CurrentPoint;
        public Vector Speed;
        public Vector BestExtr;
    }

    public class SwarmMethod
    {
        private List<ParticleData> _particlesData;

        private double _speed = 1;
        private Vector _glExtr = new Vector(0, 0);

        private PointF _lowerBorder = new PointF(-10.0F, -10.0F);
        private PointF _topBorder = new PointF(10.0F, 10.0F);

        private double _minVelocity = -1;
        private double _maxVelocity = 1;

        private Func<double, double, double> _f;

        private Random _random;

        public SwarmMethod(Func<double, double, double> func, int particlesCount)
        {
            _random = new Random(Guid.NewGuid().GetHashCode());
            _f = func;
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

                if (_f(x, y) < _f(_glExtr.Dx, _glExtr.Dy))
                {
                    _glExtr = new Vector(x, y);
                }
            }
        }

        public (List<ParticleData> data, Vector extremum) FindMinimum()
        {
            Vector _prevExtremum = _glExtr.Copy();

            int counter = 10;

            while (counter > 0)
            {
                var iteration = SingleIteration();

                if (Math.Abs(_f(_prevExtremum.Dx, _prevExtremum.Dy) - _f(_glExtr.Dx, _glExtr.Dy)) < 1)
                {
                    counter--;
                }
                else
                {
                    counter = 10;
                }
            }

            return (_particlesData, _glExtr);
        }

        public (List<ParticleData> data, Vector extremum) SingleIteration()
        {
            for (int i = 0; i < _particlesData.Count; i++)
            {
                ParticleData data = _particlesData[i];

                Vector newSpeed = 0.9 * data.Speed +
                    _random.NextDouble() * (Vector.ComponentSubstract(data.BestExtr, data.CurrentPoint)) +
                    _random.NextDouble() * (Vector.ComponentSubstract(_glExtr, data.CurrentPoint));

                Vector newPoint = data.CurrentPoint + _speed * newSpeed;

                if (_f(newPoint.Dx, newPoint.Dy) < _f(data.BestExtr.Dx, data.BestExtr.Dy))
                {
                    Vector newExtremum = new Vector(newPoint.Dx, newPoint.Dy);
                    data.BestExtr = newExtremum;

                    if (_f(data.BestExtr.Dx, data.BestExtr.Dy) < _f(_glExtr.Dx, _glExtr.Dy))
                    {
                        _glExtr = data.BestExtr.Copy();
                    }
                }

                data.CurrentPoint = newPoint;
                data.Speed = newSpeed;

                _particlesData[i] = data;
            }


            return (_particlesData, _glExtr);
        }
    }
}
