using MathLib;

namespace AILabs.HammingNetwork
{
    public class HammingNetwork
    {
        private List<NumericVector> _weightMatrix;

        private double _zOffset;
        private double _k1;
        private double _Un;
        private double _epsilon;

        public HammingNetwork(List<NumericVector> vectors)
        {
            _weightMatrix = new List<NumericVector>();

            vectors.ForEach(x => _weightMatrix.Add(x * 0.5));

            _zOffset = (double)vectors[0].size / 2;
            _k1 = 0.1;
            _Un = 1 / _k1;
            _epsilon = 1.0 / vectors.Count;
        }

        public int GetVectorGroup(NumericVector newVect)
        {
            int zCount = _weightMatrix.Count;
            int sCount = newVect.size;

            NumericVector Uinput = new NumericVector();
            for (int i = 0; i < zCount; i++)
            {
                double sum = 0;

                for (int j = 0; j < sCount; j++)
                {
                    sum += _weightMatrix[i][j] * newVect[j];
                }

                Uinput.Append(_zOffset + sum);
            }

            NumericVector MaxnetPrev = ActivationFunctions.Z_ActivationFunction(Uinput, _k1, _Un);
            NumericVector MaxnetNew = MaxnetPrev.Copy();
            int iteration = 1;

            while (true)
            {
                MaxnetNew = IterMaxnet(MaxnetPrev, iteration);

                if (MaxnetNew.Equals(MaxnetPrev))
                {
                    break;
                }

                MaxnetPrev = MaxnetNew.Copy();
                iteration++;
            }

            NumericVector MaxnetNewActivated = ActivationFunctions.Y_ActivationFunction(MaxnetNew);

            (int index, int count) trueOutput = GetTrueOutput(MaxnetNewActivated);

            if (trueOutput.count > 1)
            {
                throw new Exception("Изображению соответствует несколько исходных");
            }

            if (trueOutput.index == -1)
            {
                throw new Exception("Изображению не найдено исходных");
            }

            return trueOutput.index;
        }

        private NumericVector IterMaxnet(NumericVector vPrev, int iteration)
        {
            if (iteration == 0)
            {
                return vPrev;
            }
            else
            {
                NumericVector newVect = new NumericVector();
                double len = vPrev.size;

                for (int i = 0; i < len; i++)
                {
                    double sum = 0;

                    for (int j = 0; j < len; j++)
                    {
                        if (i != j)
                        {
                            sum += vPrev[j];
                        }
                    }

                    newVect.Append(vPrev[i] - _epsilon * sum);
                }

                return ActivationFunctions.A_ActivationFunction(newVect);
            }
        }

        private (int index, int count) GetTrueOutput(NumericVector vector)
        {
            int index = -1;
            int count = 0;

            for (int i = 0; i < vector.size; i++)
            {
                if (vector[i] == 1.0)
                {
                    index = i;
                    count++;
                }
            }

            return (index, count);
        }
    }
}
