using System;
using Godot;

namespace ProjectOriginality.Battle.Units.EnemyAttackScripts
{
    public interface IEnemyAttack
    {
        /// <summary>
        /// Get the next attack that should happen.
        /// </summary>
        /// <returns></returns>
        UnitSkill Next();

        /// <summary>
        /// Advance the script state to get a new attack.
        /// </summary>
        void Advance();
    }
}
