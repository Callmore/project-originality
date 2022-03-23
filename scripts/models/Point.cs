using System;

namespace ProjectOriginality.Models
{
    public struct Point
    {
        public int X;
        public int Y;

        public Point(int _x, int _y)
        {
            X = _x;
            Y = _y;
        }

        public static bool operator ==(Point pointA, Point pointB) => pointA.X == pointB.X && pointA.Y == pointB.Y;
        public static bool operator !=(Point pointA, Point pointB) => pointA.X != pointB.X || pointA.Y != pointB.Y;

        public override bool Equals(object obj)
        {
            return obj is Point && this == (Point)obj;
        }

        public override int GetHashCode()
        {
            return Tuple.Create(X, Y).GetHashCode();
        }
    }
}
