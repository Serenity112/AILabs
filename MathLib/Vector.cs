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

        public static PointF operator +(PointF first, Vector second) => new PointF((float)(first.X + second.Dx), (float)(first.Y + second.Dy));

        public static Vector operator -(Vector vector) => new Vector(-vector.Dx, -vector.Dy);

        public static Vector operator *(Vector vector, double multiplier) => new Vector(vector.Dx * multiplier, vector.Dy * multiplier);

        public static Vector operator *(double multiplier, Vector vector) => vector * multiplier;

        public static Vector operator -(Vector vector1, Vector vector2) => vector1 + (-vector2);

        public static Vector operator +(Vector vector1, Vector vector2) => new Vector(vector1.Dx + vector2.Dx, vector1.Dy + vector2.Dy);

        public static Vector operator /(Vector vector, double divider) => vector * (1.0 / divider);

        public double Length() => Math.Sqrt(Dx * Dx + Dy * Dy);

        public Vector Normalized()
        {
            double length = Length();
            return this / length;
        }

        public Vector Copy() => new Vector(Dx, Dy);

        public override string ToString()
        {
            return $"[{Dx}, {Dy}]";
        }
    }
}
