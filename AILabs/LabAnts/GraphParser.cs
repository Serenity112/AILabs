using MathLib;

namespace AILabs.LabAnts
{
    public class GraphParser
    {
        public static GraphData ReadGraphData(string path)
        {
            string[] lines = File.ReadAllLines(path);
            int graphSize = lines.Length;
            double[,] data = new double[graphSize, graphSize];

            for (int i = 0; i < graphSize; i++)
            {
                string line = lines[i];
                string[] substr = line.Split(' ');

                for (int j = 0; j < graphSize; j++)
                {
                    string s = substr[j];
                    data[i, j] = double.Parse(s);
                }
            }

            return new GraphData(data);
        }
    }
}
