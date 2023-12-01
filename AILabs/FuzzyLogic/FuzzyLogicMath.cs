using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AILabs.FuzzyLogic
{
    public class FuzzyLogicMath
    {
        public static double AND(params double[] fvals)
        {
            double res = fvals[0];

            for (int i = 0; i < fvals.Length; i++)
            {
                res = Math.Min(res, fvals[i]);
            }

            return res;
        }

        public static double OR(params double[] fvals)
        {
            double res = fvals[0];

            for (int i = 0; i < fvals.Length; i++)
            {
                res = Math.Max(res, fvals[i]);
            }

            return res;
        }

        public static double NOT(double fval)
        {
            return 1 - fval;
        }
    }
}
