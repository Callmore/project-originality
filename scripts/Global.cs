using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectOriginality.Party;
using Godot;

namespace ProjectOriginality
{
    public static class Global
    {
        // TODO: Replace with own RNG system for consistant results between seeds.
        public static RandomNumberGenerator Rng { get; } = new RandomNumberGenerator();

        public static PackedScene[,] NextBattleEnemyArrangement
        {
            get => _nextBattleEnemyArrangement;
            set
            {
                if (value.GetLength(0) != 2 || value.GetLength(1) != 3)
                {
                    throw new ArgumentException("Array size should be [2,3]");
                }
                _nextBattleEnemyArrangement = value;
            }
        }
        private static PackedScene[,] _nextBattleEnemyArrangement = null;

        public static void Assert(bool test)
        {
            if (!test)
            {
                throw new Exception("Exception failed.");
            }
        }

        public static void Assert(bool test, string message)
        {
            if (!test)
            {
                throw new Exception(message);
            }
        }

    }
}
