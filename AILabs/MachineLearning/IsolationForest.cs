using MathLib;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace AILabs.MachineLearning
{
    public class DataPoint
    {
        private double[] _dimentions;

        public int Dimentions => _dimentions.Length;

        public double this[int i]
        {
            get { return _dimentions[i]; }
            set { _dimentions[i] = value; }
        }

        public DataPoint(double[] data)
        {
            _dimentions = data;
        }

        public override bool Equals(object? obj)
        {
            var item = obj as DataPoint;

            if (item == null)
            {
                return false;
            }

            return _dimentions == item._dimentions;
        }

        public override int GetHashCode()
        {
            return _dimentions.GetHashCode();
        }

        public override string ToString()
        {
            return $"[{string.Join(", ", _dimentions.Select(x => Math.Round(x, 5).ToString(CultureInfo.InvariantCulture)))}]";
        }
    }

    internal class TreeNode
    {
        public int SplitDimentionIndex { get; set; }
        public double SplitValue { get; set; }
        public TreeNode LeftChild { get; set; }
        public TreeNode RightChild { get; set; }
        public int Size { get; set; } = 0;
        public bool External { get; set; } = false;
    }

    internal class DecisionTree
    {
        public DecisionTree()
        {
            Root = null;
        }

        public TreeNode Root { get; set; }
    }

    public class IsolationForest
    {
        private int _treesNum;

        private List<DecisionTree> _trees;

        private List<DataPoint> _trainData;

        private int _dimentions;

        private int _treeHeightLimit;
        private int _subSampleSize;

        private Random _seed;

        public IsolationForest(int numTrees, int dimentions, int subSampleSize)
        {
            _seed = new Random();

            _dimentions = dimentions;
            _subSampleSize = subSampleSize;
            _treeHeightLimit = (int)Math.Round(Math.Log2(subSampleSize));
            _treesNum = numTrees;

            _trainData = new List<DataPoint>();
            _trees = new List<DecisionTree>();
        }

        public void Train(List<DataPoint> inputSample)
        {
            _trainData = inputSample;

            // Создание и тренировка деревьев
            for (int t = 0; t < _treesNum; t++)
            {
                DecisionTree tree = new DecisionTree();

                List<DataPoint> inputSampleCopy = new List<DataPoint>(inputSample);
                List<DataPoint> subSample = new List<DataPoint>();

                for (int i = 0; i < _subSampleSize; i++)
                {
                    int randElem = _seed.Next(inputSampleCopy.Count);
                    subSample.Add(inputSampleCopy[randElem]);
                    inputSampleCopy.RemoveAt(randElem);
                }

                tree.Root = BuildTree(subSample);
                _trees.Add(tree);
            }
        }

        public Dictionary<DataPoint, double> GetAnomalyScores()
        {
            var anomalyScores = new Dictionary<DataPoint, double>();
            double c = CalculateC(_trainData.Count);

            foreach (DataPoint p in _trainData)
            {
                double averagePathLength = _trees.Select(tree => PathLength(tree.Root, p, 0)).Average();
                double anomalyScore = Math.Pow(2, -averagePathLength / c);
                anomalyScores.Add(p, Math.Round(anomalyScore, 15));
            }

            return anomalyScores;
        }

        // Рекурсивное построение дерева
        private TreeNode BuildTree(List<DataPoint> data)
        {
            // Рекурсивное построение дерева с ограничением глубины
            return BuildTreeRecursive(data, depth: 0);
        }

        private TreeNode BuildTreeRecursive(List<DataPoint> data, int depth)
        {
            if (depth >= _treeHeightLimit || data.Count <= 1 /*|| (data.Distinct().Count() == 1)*/)
            {
                return new TreeNode
                {
                    Size = data.Count,
                    External = true
                };
            }

            (double min, double max)[] _dimRanges = new (double min, double max)[_dimentions];
            for (int dim = 0; dim < _dimentions; dim++)
            {
                double min = data.Min(p => p[dim]);
                double max = data.Max(p => p[dim]);
                _dimRanges[dim] = (min, max);
            }

            // Выбор случайного измерения
            int rDim = _seed.Next(_dimentions);
            // Выбор границы разделения
            double dimThreshold = _seed.NextDouble() * (_dimRanges[rDim].max - _dimRanges[rDim].min) + _dimRanges[rDim].min;

            // Разделение данных
            List<DataPoint> leftData = new List<DataPoint>();
            List<DataPoint> rightData = new List<DataPoint>();

            foreach (DataPoint dataPoint in data)
            {
                if (dataPoint[rDim] < dimThreshold)
                {
                    leftData.Add(dataPoint);
                }
                else
                {
                    rightData.Add(dataPoint);
                }
            }

            // Рекурсивное построение левого и правого поддерева
            TreeNode leftChild = BuildTreeRecursive(leftData, depth + 1);
            TreeNode rightChild = BuildTreeRecursive(rightData, depth + 1);

            // Возвращаем узел с информацией о разделении
            return new TreeNode
            {
                SplitDimentionIndex = rDim,
                SplitValue = dimThreshold,
                LeftChild = leftChild,
                RightChild = rightChild,
                Size = 0,
                External = false
            };
        }

        // Рекурсивный расчет длины пути в дереве
        private double PathLength(TreeNode node, DataPoint dataPoint, int currentDepth)
        {
            if (node.External)
            {
                return currentDepth + CalculateC(node.Size);
            }

            int dimentionIndex = node.SplitDimentionIndex;
            double dimValue = dataPoint[dimentionIndex];

            if (dimValue < node.SplitValue)
            {
                return PathLength(node.LeftChild, dataPoint, currentDepth + 1);
            }
            else
            {
                return PathLength(node.RightChild, dataPoint, currentDepth + 1);
            }
        }

        private double CalculateC(double n)
        {
            if (n > 2)
            {
                return 2.0 * (Math.Log(n - 1) + 0.5772156649) - (2.0 * (n - 1)) / n;
            }

            if (n == 2)
            {
                return 1.0;
            }

            return 0.0;
        }
    }
}
