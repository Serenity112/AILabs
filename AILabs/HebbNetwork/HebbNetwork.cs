using MathLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AILabs.HebbNetwork
{
    public class HebbNetwork
    {
        private List<NumericVector> _wMatrix;
        private int _vectorsCount;
        private int _vectorLen;

        private List<NumericVector> _outputVectors;

        private int _maxGenerations = 1000;
        private TextBox textbox;
        public HebbNetwork(List<NumericVector> input, TextBox textbox)
        {
            this.textbox = textbox;
            _wMatrix = new List<NumericVector>();
            _outputVectors = new List<NumericVector>();

            _vectorsCount = input.Count;
            _vectorLen = input[0].size;

            Initialization(input);
        }

        private void Initialization(List<NumericVector> input)
        {
            // Инициализация структур
            UniqueCodeGenerator codegen = new UniqueCodeGenerator(_vectorsCount);
            for (int j = 0; j < _vectorsCount; j++)
            {
                NumericVector newCode = codegen.GetNext();
                textbox.Text += newCode.ToString();
                _outputVectors.Add(newCode);

                // +1 т.к. хранится как w_0, так и w_1..w_n
                NumericVector v = new NumericVector(_vectorLen + 1, 0);
                _wMatrix.Add(v);
            }

            // Обучение
            bool training_ended = false;
            int counter = 0;
            while (!training_ended && counter < _maxGenerations)
            {
                // Прогон каждой пары на входе
                for (int j = 0; j < _vectorsCount; j++)
                {
                    NumericVector trainingVector = input[j].Copy();
                    NumericVector t_result = CalculateWeightReaction(trainingVector);

                    if (t_result.Equals(_outputVectors[j]))
                    {
                        continue;
                    }

                    // Корректировка весов матрицы W  
                    CorrectWeights(trainingVector.Copy().Prepend(1), j);
                }

                // Проверка обучения
                training_ended = true;
                for (int j = 0; j < _vectorsCount; j++)
                {
                    NumericVector trainingVector = input[j].Copy();
                    NumericVector t_result = CalculateWeightReaction(trainingVector);

                    if (!t_result.Equals(_outputVectors[j]))
                    {
                        training_ended = false;
                    }
                }

                counter++;
            }

            textbox.Text += counter;
        }

        private void CorrectWeights(NumericVector input, int vectorNumber)
        {
            for (int j = 0; j < _wMatrix.Count; j++)
            {
                for (int i = 0; i < input.size; i++)
                {

                    _wMatrix[j][i] = _wMatrix[j][i] + input[i] * _outputVectors[vectorNumber][j];
                }
            }
        }

        // Получить реакцию размерности m по входному вектору размерности n
        private NumericVector CalculateWeightReaction(NumericVector input)
        {
            NumericVector reult = new NumericVector(_vectorsCount, 0);

            for (int j = 0; j < _vectorsCount; j++)
            {
                double sum = 0;

                // w0
                sum += _wMatrix[j][0];

                // w1 - w_max
                for (int i = 0; i < _vectorLen; i++)
                {
                    sum += _wMatrix[j][i + 1] * input[i];
                }

                sum = (sum >= 0) ? 1 : -1;

                reult[j] = sum;
            }

            return reult;
        }

        public int GetVectorGroup(NumericVector vectToRecognize)
        {
            NumericVector result = CalculateWeightReaction(vectToRecognize);

            int found_index = -1;
            bool copy_found = false;

            for (int i = 0; i < _vectorsCount; i++)
            {
                if (result.Equals(_outputVectors[i]))
                {
                    if (found_index != -1)
                    {
                        copy_found = true;
                    }

                    found_index = i;
                }
            }

            if (copy_found)
            {
                throw new Exception("Неоднозначность");
            }

            if (found_index == -1)
            {
                throw new Exception("не найдено " + result.ToString());
            }

            return found_index;
        }
    }

    public class UniqueCodeGenerator
    {
        private int currentNumber;
        private int length;

        public UniqueCodeGenerator(int length)
        {
            this.length = length;
            currentNumber = 0;
        }

        public NumericVector GetNext()
        {
            NumericVector vector = new NumericVector();
            for (int i = 0; i < length; i++)
            {
                vector.Append(((currentNumber >> i) & 1) == 0 ? -1 : 1);
            }
            currentNumber++;
            return vector;
        }
    }
}
