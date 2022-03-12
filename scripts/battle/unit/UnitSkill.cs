using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectOriginality.Models;

namespace ProjectOriginality.Battle.Units
{
    public readonly struct UnitSkill
    {
        public string Name { get; }
        public string Description { get; }
        public int ManaCost { get; }
        public double Windup { get; }
        public double RecoveryTime { get; }
        public Func<Unit, bool> Usable { get; }
        public AttackInfo Activate { get; }
        public readonly bool Valid;

        /// <summary>
        /// Define a unit skill for a playable character.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="manaCost"></param>
        /// <param name="windup"></param>
        /// <param name="cooldown"></param>
        /// <param name="usable"></param>
        /// <param name="activate"></param>
        public UnitSkill(string name, string description, int manaCost, double windup, double cooldown, Func<Unit, bool> usable, AttackInfo activate)
        {
            Name = name;
            Description = description;
            ManaCost = manaCost;
            Windup = windup;
            RecoveryTime = cooldown;
            Usable = usable;
            Activate = activate;

            Valid = true;
        }

        /// <summary>
        /// Define a unit skill for an enemy.
        /// </summary>
        public UnitSkill(string name, double windup, double cooldown, AttackInfo activate)
        {
            Name = name;
            Description = "";
            ManaCost = 0;
            Windup = windup;
            RecoveryTime = cooldown;
            Usable = IsUnitAlive;
            Activate = activate;

            Valid = true;
        }

        private static bool IsUnitAlive(Unit unit)
        {
            return unit.Health > 0;
        }
    }
}
