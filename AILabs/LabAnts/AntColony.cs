using AILabs.LabAnts;
using MathLib;

namespace AILabs
{
    public struct PathData
    {
        public PathData(List<int> list, double length)
        {
            PathIndexes = list;
            Length = length;
        }

        public List<int> PathIndexes { get; private set; }

        public double Length;
    }

    public class AntColony
    {
        public AntColony(string cityDataPath = @"..\..\..\LabAnts\Input\", string cityFileName = "input.txt")
        {
            InitializeCities(Path.Combine(cityDataPath, cityFileName));
        }

        public int StartingCityIndex { get; private set; }
        private int _citiesNum;

        public GraphData _distGraph { get; private set; }

        public GraphData _phGraph { get; private set; }

        private int _maxSteps = 100;

        // Контролируемые параметры
        public double PhInfl = 0.75;
        public double DistInfl = 2.0;

        public double PhEvapRate = 0.27;
        public double PhStr = 1.0;

        private const double InitialPhermonoes = 1;

        private void InitializeCities(string filePath)
        {
            _distGraph = GraphParser.ReadGraphData(filePath);
            _citiesNum = _distGraph.Size;

            _phGraph = new GraphData(new double[_citiesNum, _citiesNum]);
            _phGraph.Fill(InitialPhermonoes);

            StartingCityIndex = 0;
        }

        public void SearchPath()
        {
            for (int step = 0; step < _maxSteps; step++)
            {

            }
        }

        public PathData ACO_Start()
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
                length += _distGraph.Get(currentAntIndex, newIndex);

                remainingList.Remove(newIndex);
                Path.Add(newIndex);

                currentAntIndex = newIndex;
            }

            // Прошёл все вершины, возвращаемся в начальную
            length += _distGraph.Get(currentAntIndex, StartingCityIndex);
            Path.Add(StartingCityIndex);

            return new PathData(Path, length);
        }


        public void UpdatePheromones(PathData pathdata)
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
                    double newPh = (1 - PhEvapRate) * _phGraph.Get(i, j);

                    if (traveledEdges.Contains((i, j)) || traveledEdges.Contains((j, i)))
                    {
                        newPh += PhStr / pathdata.Length;
                    }

                    _phGraph.Set(i, j, newPh);
                    _phGraph.Set(j, i, newPh);
                }
            }
        }

        private double CalculateVertexChance(int i, int j, int[] allowed)
        {
            return VertexChance(i, j) / allowed.Sum(l => VertexChance(i, l));
        }

        private double VertexChance(int i, int j)
        {
            return Math.Pow(_phGraph.Get(i, j), PhInfl) / Math.Pow(_distGraph.Get(i, j), DistInfl);
        }

    }
}
