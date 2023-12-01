using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AILabs.MachineLearning
{
    public class MLParser
    {
        public static List<(double[] dimentionalData, bool isAnomaly)> ParseFile(string filePath)
        {
            List<(double[], bool)> resultList = new();

            string[] lines = File.ReadAllLines(filePath);

            // Парсим каждую строку
            foreach (string line in lines)
            {
                string[] numberStrings = line.Split(',');

                double[] numbers = new double[8];

                for (int i = 0; i < 8; i++)
                {
                    numbers[i] = double.Parse(numberStrings[i], CultureInfo.InvariantCulture);
                }

                bool res = int.Parse(numberStrings[8]) == 1;

                // Добавляем пару значений в список
                resultList.Add((numbers, res));
            }

            return resultList;
        }
    }
}
 