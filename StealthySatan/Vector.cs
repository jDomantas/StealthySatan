using Microsoft.Xna.Framework;
using System;

namespace StealthySatan
{
    class Vector
    {
        public double X { get; set; }
        public double Y { get; set; }

        public Vector()
        {
            X = 0;
            Y = 0;
        }

        public Vector(double x, double y)
        {
            X = x;
            Y = y;
        }

        public double LengthSquared => X * X + Y * Y;
        public double Length => Math.Sqrt(X * X + Y * Y);
        public Vector Normalized => this / Length;

        public static Vector operator +(Vector a, Vector b)
        {
            return new Vector(a.X + b.X, a.Y + b.Y);
        }

        public static Vector operator -(Vector a, Vector b)
        {
            return new Vector(a.X - b.X, a.Y - b.Y);
        }

        public static Vector operator *(Vector a, double b)
        {
            return new Vector(a.X * b, a.Y * b);
        }

        public static Vector operator *(double b, Vector a)
        {
            return new Vector(a.X * b, a.Y * b);
        }

        public static Vector operator /(Vector a, double b)
        {
            return new Vector(a.X / b, a.Y / b);
        }

        public double Dot(Vector other) => X * other.X + Y * other.Y;

        public Vector2 ToVector2 => new Vector2((float)X, (float)Y);

        public static Vector Zero => new Vector(0, 0);
        public static Vector UnitX => new Vector(1, 0);
        public static Vector UnitY => new Vector(0, 1);
    }
}
