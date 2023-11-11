using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathLib;

namespace AILabs.Genetic
{
    public abstract class Chromosome
    {
        protected RectangleF _rect;

        protected Random _seed;

        public Chromosome(Random seed, RectangleF rect)
        {
            _rect = rect;
            _seed = seed;
        }

        public abstract Vector GetRealCoordiantes();

        public abstract void Mutate(double chance);

        public abstract (Chromosome child1, Chromosome child2) CrossoverWith(Chromosome partner);
    }

    public class BinaryChromosome : Chromosome
    {
        public uint X { get; private set; }

        public uint Y { get; private set; }

        public BinaryChromosome(uint x, uint y, Random seed, RectangleF rect) : base(seed, rect)
        {
            X = x;
            Y = y;
        }

        public BinaryChromosome(Random seed, RectangleF rect) : base(seed, rect)
        {
            X = GrayCode((uint)(uint.MaxValue * seed.NextDouble()));
            Y = GrayCode((uint)(uint.MaxValue * seed.NextDouble()));
        }

        public override Vector GetRealCoordiantes()
        {
            return new Vector(
                (double)DecodeGrayCode(X) / uint.MaxValue * _rect.Width + _rect.Left,
                (double)DecodeGrayCode(Y) / uint.MaxValue * _rect.Height + _rect.Top);
        }

        public override void Mutate(double chance)
        {
            for (int i = 0; i < 32; i++)
            {
                if (_seed.NextDouble() < chance)
                {
                    X ^= (1U << i);
                }

                if (_seed.NextDouble() < chance)
                {
                    Y ^= (1U << i);
                }
            }
        }

        public override (Chromosome child1, Chromosome child2) CrossoverWith(Chromosome partner)
        {
            BinaryChromosome other = (BinaryChromosome)partner;

            int gene = _seed.Next(2);

            // Разрыв в X
            if (gene == 0)
            {
                uint mask = (uint)((~0) << _seed.Next(32));

                uint newX1 = (other.X & mask) | (X & ~mask);
                uint newX2 = (X & mask) | (other.X & ~mask);

                uint newY1 = Y;
                uint newY2 = other.Y;

                return (
                    new BinaryChromosome(newX1, newY1, _seed, _rect),
                    new BinaryChromosome(newX2, newY2, _seed, _rect));
            }
            else
            // Разрыв в Y            
            {
                uint newX1 = other.X;
                uint newX2 = X;

                uint mask = (uint)((~0) << _seed.Next(32));

                uint newY1 = (other.Y & mask) | (Y & ~mask);
                uint newY2 = (Y & mask) | (other.Y & ~mask);

                return (
                    new BinaryChromosome(newX1, newY1, _seed, _rect),
                    new BinaryChromosome(newX2, newY2, _seed, _rect));
            }
        }

        public static uint GrayCode(uint n)
        {
            return n ^ (n >> 1);
        }

        public static uint DecodeGrayCode(uint grayCode)
        {
            uint n = 0;
            while (grayCode != 0)
            {
                n ^= grayCode;
                grayCode >>= 1;
            }
            return n;
        }
    }

    public class RealChromosome : Chromosome
    {
        public double X { get; private set; }

        public double Y { get; private set; }

        public RealChromosome(Random seed, RectangleF rect) : base(seed, rect)
        {
            (double X, double Y) left_bot = (rect.X, rect.Y);
            (double X, double Y) right_top = (rect.X + rect.Width, rect.Y + rect.Height);

            X = ((right_top.X - left_bot.X) * seed.NextDouble()) + left_bot.X;
            Y = ((right_top.Y - left_bot.Y) * seed.NextDouble()) + left_bot.Y;
        }

        public RealChromosome(double x, double y, Random seed, RectangleF rect) : base(seed, rect)
        {
            X = x;
            Y = y;
        }

        public override Vector GetRealCoordiantes()
        {
            return new Vector(X, Y);
        }

        public override void Mutate(double chance)
        {
            double shift = 0.5;

            if (_seed.NextDouble() < chance)
            {
                X += (_seed.NextDouble() * 2 * shift) - shift;
            }

            if (_seed.NextDouble() < chance)
            {
                Y += (_seed.NextDouble() * 2 * shift) - shift;
            }
        }

        public override (Chromosome child1, Chromosome child2) CrossoverWith(Chromosome partner)
        {
            double l = 0.3;

            RealChromosome other = (RealChromosome)partner;

            int rGene = _seed.Next(3);

            if (rGene == 0)
            {
                return (
                    new RealChromosome(X, Y, _seed, _rect),
                    new RealChromosome(other.X, other.Y, _seed, _rect));
            }
            else if (rGene == 1)
            {
                double newX1 = l * X + (1 - l) * other.X;
                double newX2 = l * other.X + (1 - l) * X;

                double newY1 = Y;
                double newY2 = other.Y;

                return (
                    new RealChromosome(newX1, newY1, _seed, _rect),
                    new RealChromosome(newX2, newY2, _seed, _rect));
            }
            else
            {
                double newX1 = l * X + (1 - l) * other.X;
                double newX2 = l * other.X + (1 - l) * X;

                double newY1 = l * Y + (1 - l) * other.Y;
                double newY2 = l * other.Y + (1 - l) * Y;

                return (
                    new RealChromosome(newX1, newY1, _seed, _rect),
                    new RealChromosome(newX2, newY2, _seed, _rect));
            }
        }
    }
}
