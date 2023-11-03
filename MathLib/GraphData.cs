using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathLib
{
    public struct GraphData
    {
        public GraphData(double[,] data)
        {
            _matrix = data;
        }

        private double[,] _matrix;

        public int Size
        {
            get
            {
                return _matrix.GetLength(0);
            }
            private set
            {
                Size = value;
            }
        }

        public double Get(int i, int j)
        {
            return _matrix[i, j];
        }

        public void Set(int i, int j, double value)
        {
            _matrix[i, j] = value;
        }

        public void Fill(double value)
        {
            for (int i = 0; i < Size; i++)
            {
                for (int j = 0; j < Size; j++)
                {
                    _matrix[i, j] = value;
                }
            }
        }

        public PathData GetPath(List<int> vertexes)
        {
            double length = 0;
            int currVertex = vertexes[0];

            for (int i = 1; i < vertexes.Count; i++)
            {
                int newVertex = vertexes[i];
                length += _matrix[currVertex, newVertex];
                currVertex = newVertex;
            }

            return new PathData(vertexes, length);
        }
    }

    public struct PathData
    {
        public PathData(List<int> list, double length)
        {
            PathIndexes = list;
            Length = length;
        }

        public List<int> PathIndexes { get; private set; }

        public double Length { get; private set; }

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

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < PathIndexes.Count; i++)
            {
                stringBuilder.Append(Convert.ToChar(PathIndexes[i] + 65).ToString());
                if (i != PathIndexes.Count - 1)
                {
                    stringBuilder.Append(" -> ");
                }
            }
            return stringBuilder.ToString();
        }
    }
}
