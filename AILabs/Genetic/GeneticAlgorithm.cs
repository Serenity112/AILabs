using MathLib;

namespace AILabs.Genetic
{
    public enum EncodingMode
    {
        Integer,
        Real,
    }

    public struct GeneticAlgorithmData
    {
        public GeneticAlgorithmData(int populationSize, double selectionCutoffThreshold,
            EncodingMode encodingMode, double crossoverChance,
            double mutationChance)
        {
            PopulationSize = populationSize;
            SelectionCutoffThreshold = selectionCutoffThreshold;
            Encoding = EncodingMode.Integer;
            CrossoverChance = crossoverChance;
            MutationChance = mutationChance;
        }

        public static GeneticAlgorithmData DefaultData() => new GeneticAlgorithmData(100, 0.7,
            EncodingMode.Integer, 0.6,
            0.01);

        public int PopulationSize;

        public double SelectionCutoffThreshold;

        public EncodingMode Encoding;

        public double CrossoverChance;

        public double MutationChance;

        public double GenerationGap;
    }

    public struct RectangleBorder
    {
        public (double x1, double y1) Point1 { get; private set; }

        public (double x2, double y2) Point2 { get; private set; }

        public RectangleBorder((double x1, double y1) point1, (double x2, double y2) point2)
        {
            Point1 = point1;
            Point2 = point2;
        }
    }

    public class GeneticAlgorithm
    {
        private int _populationMaxSize;

        private Random _seed;

        RectangleF _border;

        private List<Chromosome> _populationData = new List<Chromosome>();

        private GeneticAlgorithmData _gaData;

        private Func<double, double, double> _function;

        private EncodingMode _enconingMode;

        public GeneticAlgorithm(Func<double, double, double> func,
            GeneticAlgorithmData gaData,
            RectangleF border)
        {
            _function = func;
            _gaData = gaData;
            _populationMaxSize = gaData.PopulationSize;
            _seed = new Random(Guid.NewGuid().GetHashCode());
            _enconingMode = gaData.Encoding;
            _border = border;
            RandomInitialization(_populationMaxSize);
        }

        private void RandomInitialization(int size)
        {
            switch (_enconingMode)
            {
                case EncodingMode.Integer:
                    for (int i = 0; i < size; i++)
                    {
                        IntChromosome chr = new IntChromosome(_seed, _border);
                        _populationData.Add(chr);
                    }
                    break;
                case EncodingMode.Real:
                    for (int i = 0; i < size; i++)
                    {
                        _populationData.Add(new RealChromosome(_seed, _border));
                    }
                    break;
            }
        }

        public struct GeneticAlgResult
        {
            public GeneticAlgResult(List<Vector> vectors, double extremumValue, Vector extremumCoords)
            {
                Vectors = vectors;
                ExtremumValue = extremumValue;
                ExtremumCoords = extremumCoords;
            }

            public List<Vector> Vectors;
            public double ExtremumValue;
            public Vector ExtremumCoords;
        }

        public GeneticAlgResult SingleIteration()
        {
            // Кроссовер
            List<Chromosome> newGeneration = new List<Chromosome>();
            int childTargetCount = (int)((1 - _gaData.SelectionCutoffThreshold) * _populationMaxSize);

            int childCount = 0;
            while (true)
            {
                if (_seed.NextDouble() < _gaData.CrossoverChance)
                {
                    var newChr = CrossingOver();

                    int diff = childTargetCount - childCount;

                    if (diff == 0)
                    {
                        break;
                    }
                    else if (diff == 1)
                    {
                        newGeneration.Add(newChr.child1);
                        break;
                    }
                    else
                    {
                        newGeneration.Add(newChr.child1);
                        newGeneration.Add(newChr.child2);
                        childCount += 2;
                    }
                }
            }

            // Селекция
            Selection();

            // Обновление поколения
            _populationData.AddRange(newGeneration);

            // Мутация
            Mutate();

            double extremum = FuncVal(_populationData[0]);
            Vector extremumCoords = _populationData[0].GetRealCoordiantes();
            List<Vector> vectors = new List<Vector>();

            foreach (var chromosome in _populationData)
            {
                Vector coords = chromosome.GetRealCoordiantes();
                vectors.Add(coords);
                double value = FuncVal(chromosome);
                if (value < extremum)
                {
                    extremum = value;
                    extremumCoords = coords;
                }
            }

            return new GeneticAlgResult(vectors, extremum, extremumCoords);
        }

        private void Selection()
        {
            int cutoffTarget = _populationMaxSize - (int)(_gaData.SelectionCutoffThreshold * _populationMaxSize);

            SortPopulation();

            for (int i = 0; i < cutoffTarget; i++)
            {
                _populationData.RemoveAt(_populationData.Count - 1);
            }
        }

        private (Chromosome child1, Chromosome child2) CrossingOver()
        {
            int parent1 = _seed.Next(_populationMaxSize);
            int parent2 = parent1;
            while (parent1 == parent2)
            {
                parent2 = _seed.Next(_populationMaxSize);
            }

            return _populationData[parent1].CrossoverWith(_populationData[parent2]);
        }

        private void Mutate()
        {
            foreach (Chromosome child in _populationData)
            {
                child.Mutate(_gaData.MutationChance);
            }
        }

        private void SortPopulation()
        {
            for (int i = 0; i < _populationMaxSize; i++)
            {
                for (int j = 1; j < _populationMaxSize - i; j++)
                {
                    double val_j1 = FuncVal(_populationData[j - 1]);
                    double val_j2 = FuncVal(_populationData[j]);

                    if (val_j1 > val_j2)
                    {
                        Chromosome temp = _populationData[j - 1];
                        _populationData[j - 1] = _populationData[j];
                        _populationData[j] = temp;
                    }
                }
            }
        }

        private double FuncVal(Chromosome chr)
        {
            Vector val = chr.GetRealCoordiantes();
            return _function(val.Dx, val.Dy);
        }

    }
}
