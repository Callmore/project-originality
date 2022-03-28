using System;

namespace ProjectOriginality.Models
{
    public struct BattleLoc
    {
        public int Line;
        public int Lane;

        public BattleLoc(int line, int lane)
        {
            Line = line;
            Lane = lane;
        }

        public override bool Equals(object obj)
        {
            return obj is BattleLoc point &&
                   Line == point.Line &&
                   Lane == point.Lane;
        }

        public override int GetHashCode()
        {
            int hashCode = 1861411795;
            hashCode = hashCode * -1521134295 + Line.GetHashCode();
            hashCode = hashCode * -1521134295 + Lane.GetHashCode();
            return hashCode;
        }

        public static bool operator ==(BattleLoc pointA, BattleLoc pointB) => pointA.Line == pointB.Line && pointA.Lane == pointB.Lane;
        public static bool operator !=(BattleLoc pointA, BattleLoc pointB) => pointA.Line != pointB.Line || pointA.Lane != pointB.Lane;

        public override string ToString()
        {
            return $"{nameof(BattleLoc)}({Line}, {Lane})";
        }
    }
}
