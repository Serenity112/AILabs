using System;
using System.Collections.Generic;
using System.Drawing;

namespace MathLib
{
    public class Vector
    {
        public double Dx { get; private set; }
        public double Dy { get; private set; }

        public Vector() { }

        public Vector(double x, double y)
        {
            Dx = x;
            Dy = y;
        }

        public Vector(Point start, Point end) : this(end.X - start.X, end.Y - start.Y) { }

        public static Point operator +(Point first, Vector second) => new Point((int)(first.X + second.Dx), (int)(first.Y + second.Dy));

        public static Vector operator -(Vector vector) => new(-vector.Dx, -vector.Dy);

        public static Vector operator *(Vector vector, double multiplier) => new(vector.Dx * multiplier, vector.Dy * multiplier);

        public static Vector operator *(double multiplier, Vector vector) => vector * multiplier;

        public static Vector operator /(Vector vector, double divider) => vector * (1.0 / divider);

        public double Length() => Math.Sqrt(Dx * Dx + Dy * Dy);

        public Vector Normalized()
        {
            double length = Length();
            return this / length;
        }
    }
}
