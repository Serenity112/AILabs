using MathLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AILabs.SimulatedAnnealing
{
    public struct AnnealingParameters
    {
        public AnnealingParameters(double InitialTemperature, double FinalTemperature, double TemperatureStep)
        {
            this.InitialTemperature = InitialTemperature;
            this.FinalTemperature = FinalTemperature;
            this.TemperatureStep = TemperatureStep;
        }

        public static AnnealingParameters DefaultParameters()
        {
            return new AnnealingParameters(90, 0.1, 0.01);
        }

        public double InitialTemperature { get; private set; }
        public double FinalTemperature { get; private set; }
        public double TemperatureStep { get; private set; }
    }

    public class SimulatedAnnealing
    {
        private double _currentTemperature;
        private double _finalTemperature;
        private double _step;

        private GraphData _graphData;

        private PathData _solution;

        private Random _randomSeed = new Random();

        public SimulatedAnnealing(GraphData graphData, AnnealingParameters parameters)
        {
            _graphData = graphData;

            _currentTemperature = parameters.InitialTemperature;
            _finalTemperature = parameters.FinalTemperature;
            _step = parameters.TemperatureStep;

            _solution = ShuffledSolution();
        }

        public GraphData GetGraph()
        {
            return _graphData;
        }

        public PathData FullSearch()
        {
            while (_currentTemperature > _finalTemperature)
            {
                SingleIteration();
            }

            return _solution;
        }

        public PathData SingleIteration()
        {
            PathData newSolution = SwapSolution();

            double diff = _solution.Length - newSolution.Length;

            if (diff > 0 || (_randomSeed.NextDouble() < Math.Exp(diff / _currentTemperature)))
            {
                _solution = newSolution;
            }

            _currentTemperature -= _step;

            return _solution;
        }

        private PathData ShuffledSolution()
        {
            int size = _graphData.Size;

            List<int> newSolution = new List<int>();
            for (int i = 1; i < size; i++)
            {
                newSolution.Add(i);
            }

            for (int i = 0; i < newSolution.Count; i++)
            {
                int randIndex = _randomSeed.Next(0, newSolution.Count);
                int buffer = newSolution[randIndex];
                newSolution[randIndex] = newSolution[i];
                newSolution[i] = buffer;
            }

            newSolution.Insert(0, 0);
            newSolution.Add(0);

            return _graphData.GetPath(newSolution);
        }

        private PathData SwapSolution()
        {
            int size = _graphData.Size;

            List<int> newSolution = new List<int>();
            for (int i = 1; i < size; i++)
            {
                newSolution.Add(_solution[i]);
            }

            int randIndex1 = _randomSeed.Next(0, newSolution.Count);
            int randIndex2 = randIndex1;
            while (randIndex1 == randIndex2)
            {
                randIndex2 = _randomSeed.Next(0, newSolution.Count);
            }
            int buff = newSolution[randIndex1];
            newSolution[randIndex1] = newSolution[randIndex2];
            newSolution[randIndex2] = buff;

            newSolution.Insert(0, 0);
            newSolution.Add(0);

            return _graphData.GetPath(newSolution);
        }

        public double GetTemperature() => Math.Round(_currentTemperature, 3);
    }
}
