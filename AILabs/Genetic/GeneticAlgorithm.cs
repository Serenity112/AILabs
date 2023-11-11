using MathLib;
using System.Collections.Generic;

namespace AILabs.Genetic
{
    public enum EncodingMode
    {
        Binary,
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
            Encoding = encodingMode;
            CrossoverChance = crossoverChance;
            MutationChance = mutationChance;
        }

        public static GeneticAlgorithmData DefaultData() => new GeneticAlgorithmData(100, 0.7,
            EncodingMode.Binary, 0.6,
            0.01);

        public int PopulationSize;

        public double SelectionCutoffThreshold;

        public EncodingMode Encoding;

        public double CrossoverChance;

        public double MutationChance;

        public double GenerationGap;
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
                case EncodingMode.Binary:
                    for (int i = 0; i < size; i++)
                    {
                        BinaryChromosome chr = new BinaryChromosome(_seed, _border);
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
            int childTargetCount = (int)((1 - _gaData.SelectionCutoffThreshold) * _populationData.Count);

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


            // Мутация
            newGeneration = Mutate(newGeneration);

            // Селекция
            Selection();

            // Обновление поколения
            _populationData.AddRange(newGeneration);

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
            int cutoffTarget = (int)((1 - _gaData.SelectionCutoffThreshold) * _populationData.Count);

            SortPopulation();

            for (int i = 0; i < cutoffTarget; i++)
            {
                _populationData.RemoveAt(_populationData.Count - 1);
            }
        }

        private (Chromosome child1, Chromosome child2) CrossingOver()
        {
            int parent1 = _seed.Next(_populationData.Count);
            int parent2 = parent1;
            while (parent1 == parent2)
            {
                parent2 = _seed.Next(_populationData.Count);
            }

            return _populationData[parent1].CrossoverWith(_populationData[parent2]);
        }

        private List<Chromosome> Mutate(List<Chromosome> mutants)
        {
            List<Chromosome> m = new List<Chromosome>();
            foreach (Chromosome chr in mutants)
            {
                chr.Mutate(_gaData.MutationChance);
                m.Add(chr);
            }
            return m;
        }

        private void SortPopulation()
        {
            for (int i = 0; i < _populationData.Count; i++)
            {
                for (int j = 1; j < _populationData.Count - i; j++)
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
