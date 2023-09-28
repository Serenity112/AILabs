using AILabs.LabAnts;
using MathLib;

namespace AILabs
{
    public struct AntColonyParameters
    {
        public AntColonyParameters(double PhInfl, double DistInfl, double PhEvapRate, double PhStr, int PathCopyCount)
        {
            this.PhInfl = PhInfl;
            this.DistInfl = DistInfl;
            this.PhEvapRate = PhEvapRate;
            this.PhStr = PhStr;
            this.PathCopyCount = PathCopyCount;
        }

        public static AntColonyParameters DefaultParameters()
        {
            return new AntColonyParameters(0.75, 2.0, 0.27, 1.0, 10);
        }

        public double PhInfl { get; private set; }
        public double DistInfl { get; private set; }
        public double PhEvapRate { get; private set; }
        public double PhStr { get; private set; }
        public int PathCopyCount { get; private set; }
    }

    public struct PathData
    {
        public PathData(List<int> list, double length)
        {
            PathIndexes = list;
            Length = length;
        }

        public List<int> PathIndexes { get; private set; }

        public double Length;

        public override bool Equals(object? obj)
        {
            if (!(obj is PathData))
                return false;

            PathData pathData = (PathData)obj;

            return PathIndexes.SequenceEqual(pathData.PathIndexes);
        }

        public override int GetHashCode()
        {
            return PathIndexes.GetHashCode();
        }
    }

    public class AntColony
    {
        public AntColony(AntColonyParameters parameters, string cityDataPath = @"..\..\..\LabAnts\Input\", string cityFileName = "input.txt")
        {
            InitializeCities(Path.Combine(cityDataPath, cityFileName));

            _phInfl = parameters.PhInfl;
            _distInfl = parameters.DistInfl;
            _phEvapRate = parameters.PhEvapRate;
            _phStr = parameters.PhStr;
            _samePathCount = parameters.PathCopyCount;
        }

        public int StartingCityIndex { get; private set; }
        private int _citiesNum;

        public GraphData DistancesGraph { get; private set; }

        public GraphData PheromonesGraph { get; private set; }

        // Условие окончания полного алгоритма
        private int _samePathCount = 10;
        private int _maxIterations = 200;

        // Контролируемые параметры
        private double _phInfl = 0.75;
        private double _distInfl = 2.0;

        private double _phEvapRate = 0.27;
        private double _phStr = 1.0;

        private const double InitialPhermonoes = 1;

        private void InitializeCities(string filePath)
        {
            DistancesGraph = GraphParser.ReadGraphData(filePath);
            _citiesNum = DistancesGraph.Size;

            PheromonesGraph = new GraphData(new double[_citiesNum, _citiesNum]);
            PheromonesGraph.Fill(InitialPhermonoes);

            StartingCityIndex = 0;
        }

        public PathData ACO_Full()
        {
            int samePathCountdown = _samePathCount;
            int counter = 0;

            PathData prevData = SingleIteration();
            UpdatePheromones(prevData);

            while (samePathCountdown > 0 && counter < _maxIterations)
            {
                PathData newData = SingleIteration();
                UpdatePheromones(newData);

                if (newData.Equals(prevData))
                {
                    samePathCountdown--;
                }

                prevData = newData;
                counter++;
            }

            return prevData;
        }

        public PathData SingleIteration()
        {
            List<int> Path = new List<int>();
            List<int> remainingList = Enumerable.Range(0, _citiesNum).ToList();

            int currentAntIndex = StartingCityIndex;
            remainingList.Remove(StartingCityIndex);
            Path.Add(StartingCityIndex);

            double length = 0;
            // Обход всех вершин
            for (int i = 0; i < _citiesNum - 1; i++)
            {
                List<double> chances = new List<double>();
                foreach (int newVertex in remainingList)
                {
                    double chance = CalculateVertexChance(currentAntIndex, newVertex, remainingList.ToArray());
                    chances.Add(chance);
                }

                WeightedRandom weightedRandom = new WeightedRandom(chances.ToArray());
                int weightedIndex = weightedRandom.GetWeightedChance();
                int newIndex = remainingList[weightedIndex];
                length += DistancesGraph.Get(currentAntIndex, newIndex);

                remainingList.Remove(newIndex);
                Path.Add(newIndex);

                currentAntIndex = newIndex;
            }

            // Прошёл все вершины, возвращаемся в начальную
            length += DistancesGraph.Get(currentAntIndex, StartingCityIndex);
            Path.Add(StartingCityIndex);

            PathData pathData = new PathData(Path, length);
            UpdatePheromones(pathData);

            return pathData;
        }


        private void UpdatePheromones(PathData pathdata)
        {
            List<int> path = pathdata.PathIndexes;
            List<(int, int)> traveledEdges = new List<(int, int)>();

            for (int v = 0; v < path.Count - 1; v++)
            {
                int i = path[v];
                int j = path[v + 1];
                traveledEdges.Add((i, j));
            }

            for (int i = 0; i < _citiesNum; i++)
            {
                for (int j = _citiesNum - 1; j > i; j--)
                {
                    double newPh = (1 - _phEvapRate) * PheromonesGraph.Get(i, j);

                    if (traveledEdges.Contains((i, j)) || traveledEdges.Contains((j, i)))
                    {
                        newPh += _phStr / pathdata.Length;
                    }

                    PheromonesGraph.Set(i, j, newPh);
                    PheromonesGraph.Set(j, i, newPh);
                }
            }
        }

        private double CalculateVertexChance(int i, int j, int[] allowed)
        {
            return VertexChance(i, j) / allowed.Sum(l => VertexChance(i, l));
        }

        private double VertexChance(int i, int j)
        {
            return Math.Pow(PheromonesGraph.Get(i, j), _phInfl) / Math.Pow(DistancesGraph.Get(i, j), _distInfl);
        }
    }
}
