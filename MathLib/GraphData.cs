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
    }
}
