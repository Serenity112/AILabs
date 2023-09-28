namespace MathLib
{
    public class WeightedRandom
    {
        private double _ratioSum;

        private double[] _chances;

        public WeightedRandom(double[] chances)
        {
            _chances = chances;
            _ratioSum = chances.Sum();
        }

        public int GetWeightedChance()
        {
            Random random = new Random(Guid.NewGuid().GetHashCode());
            double numericValue = random.NextDouble() * _ratioSum;

            for (int i = 0; i < _chances.Length; i++)
            {
                double chance = _chances[i];
                numericValue -= chance;

                if (numericValue > 0)
                {
                    continue;
                }
                else
                {
                    return i;
                }
            }

            return _chances.Length - 1;
        }
    }
}
