using System;
using System.Globalization;

namespace MathLib
{
    public class NumericVector
    {
        private List<double> _data = new List<double>();

        public NumericVector() { }

        public NumericVector(List<double> data)
        {
            _data = new List<double>(data);
        }

        public NumericVector(params double[] bits)
        {
            foreach (var bit in bits)
            {
                _data.Add(bit);
            }
        }

        public static NumericVector operator *(NumericVector NumericVector, double multiplier)
        {
            NumericVector newvect = new NumericVector();
            NumericVector._data.ForEach(x => { newvect.Append(x * multiplier); });
            return newvect;
        }

        public static NumericVector operator *(double multiplier, NumericVector NumericVector) => NumericVector * multiplier;

        public NumericVector Copy()
        {
            return new NumericVector(_data);
        }

        public int size => _data.Count;

        public double this[int i]
        {
            get
            {
                return _data[i];
            }
            set
            {
                Set(i, value);
            }
        }

        public double Get(int index)
        {
            if (index >= _data.Count)
            {
                throw new ArgumentException("Index out of array");
            }

            return _data[index];
        }

        public void Set(int index, double number)
        {
            if (index >= _data.Count)
            {
                throw new ArgumentException("Index out of array");
            }

            _data[index] = number;
        }

        public void Append(double number)
        {
            _data.Add(number);
        }

        public void RemoveAt(int index)
        {
            if (index >= _data.Count)
            {
                throw new ArgumentException("Index out of array");
            }

            _data.RemoveAt(index);
        }

        public override bool Equals(object obj)
        {
            var item = obj as NumericVector;

            if (item == null)
            {
                return false;
            }

            bool equals = true;
            for (int i = 0; i < _data.Count; i++)
            {
                if (Math.Round(this._data[i], 10) != Math.Round(item._data[i], 10))
                {
                    equals = false;
                    break;
                }
            }

            return equals;
        }

        public override int GetHashCode()
        {
            return this._data.GetHashCode();
        }

        public override string ToString()
        {
            return $"[{string.Join(", ", _data.Select(p => p.ToString(CultureInfo.InvariantCulture)).ToArray())}]";
        }
    }
}
