using System;
using Godot;
using ProjectOriginality.Models;
using ProjectOriginality.Resources;

namespace ProjectOriginality.Battle.Units.EnemyAttackScripts
{
    public class BasicEnemyAttack : EnemyAttackScript
    {
        private UnitSkill[] _skillList = new UnitSkill[] {
            new UnitSkill(name: "Hit them lamo", windup:1, recoveryTime: 5, damage:1),
            new UnitSkill(name:"Hit them again", windup:1,recoveryTime: 5, damage: 2),
            new UnitSkill(name:"One more time",windup: 2,recoveryTime: 6, damage: 3),
        };

        private int _currentSkill = 0;

        public override UnitSkill Next()
        {
            return _skillList[_currentSkill];
        }

        public override void Advance()
        {
            _currentSkill = (_currentSkill + 1) % _skillList.Length;
        }
    }
}
