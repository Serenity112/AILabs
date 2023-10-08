using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathLib;

namespace AILabs.HammingNetwork
{
    public class ActivationFunctions
    {
        public static NumericVector Z_ActivationFunction(NumericVector vector, double k1, double U_n)
        {
            NumericVector activatedVector = new NumericVector();

            for (int i = 0; i < vector.size; i++)
            {
                double zNeuron = vector[i];
                double actZ = 0;

                if (zNeuron >= 0 && zNeuron <= U_n)
                {
                    actZ = zNeuron * k1;
                }

                if (zNeuron > U_n)
                {
                    actZ = zNeuron;
                }

                activatedVector.Append(actZ);
            }

            return activatedVector;
        }

        public static NumericVector Y_ActivationFunction(NumericVector vector)
        {
            NumericVector activatedVector = new NumericVector();

            for (int i = 0; i < vector.size; i++)
            {
                double yNeuron = vector[i];

                if (yNeuron > 0)
                {
                    activatedVector.Append(1.0);
                }
                else
                {
                    activatedVector.Append(0.0);
                }
            }

            return activatedVector;
        }

        public static NumericVector A_ActivationFunction(NumericVector vector)
        {
            NumericVector activatedVector = new NumericVector();

            for (int i = 0; i < vector.size; i++)
            {
                double aNeuron = vector[i];

                if (aNeuron > 0)
                {
                    activatedVector.Append(aNeuron);
                }
                else
                {
                    activatedVector.Append(0);
                }
            }

            return activatedVector;
        }
    }
}
