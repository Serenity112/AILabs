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

    public struct SwarmMethodData
    {
        public SwarmMethodData(List<ParticleData> particles, double extremumValue, Vector extremumCoords)
        {
            Particles = particles;
            ExtremumValue = extremumValue;
            ExtremumCoords = extremumCoords;
        }

        public List<ParticleData> Particles;
        public double ExtremumValue;
        public Vector ExtremumCoords;
    }

    public class SwarmMethod
    {
        private List<ParticleData> _particlesData;

        private Vector _glExtr = new Vector(100, 100);

        private Func<double, double, double> _f;

        private Random _random;

        private RectangleF _border;

        public SwarmMethod(Func<double, double, double> func, int particlesCount, RectangleF bounds)
        {
            _random = new Random(Guid.NewGuid().GetHashCode());
            _f = func;
            _particlesData = new List<ParticleData>();
            _border = bounds;

            RandomInitialization(particlesCount);
        }

        private void RandomInitialization(int particlesCount)
        {
            double _minVelocity = -1;
            double _maxVelocity = 1;

            for (int i = 0; i < particlesCount; i++)
            {
                double x = _random.NextDouble() * _border.Width + _border.Left;
                double y = _random.NextDouble() * _border.Height + _border.Top;
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

        public SwarmMethodData SingleIteration()
        {
            double _speed = 1;

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

            return new SwarmMethodData(_particlesData, _f(_glExtr.Dx, _glExtr.Dy), _glExtr);
        }
    }
}
