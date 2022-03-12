using System;
using Godot;
using ProjectOriginality.Models;

namespace ProjectOriginality.Battle.Units.EnemyAttackScripts
{
    public class BasicEnemyAttack : IEnemyAttack
    {
        private UnitSkill[] _skillList = new UnitSkill[] {
            new UnitSkill("Hit them lamo", 1, 5, new AttackInfo(1)),
            new UnitSkill("Hit them again", 1, 5, new AttackInfo(2)),
            new UnitSkill("One more time", 2, 6, new AttackInfo(3)),
        };

        private int _currentSkill = 0;

        public UnitSkill Next()
        {
            return _skillList[_currentSkill];
        }

        public void Advance()
        {
            _currentSkill = (_currentSkill + 1) % _skillList.Length;
        }
    }
}
