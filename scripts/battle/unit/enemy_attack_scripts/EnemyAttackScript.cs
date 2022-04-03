using System;
using Godot;
using ProjectOriginality.Resources;

namespace ProjectOriginality.Battle.Units.EnemyAttackScripts
{
    public abstract class EnemyAttackScript : Reference
    {
        /// <summary>
        /// Get the next attack that should happen.
        /// </summary>
        /// <returns></returns>
        public abstract UnitSkill Next();

        /// <summary>
        /// Advance the script state to get a new attack.
        /// </summary>
        public abstract void Advance();
    }
}
