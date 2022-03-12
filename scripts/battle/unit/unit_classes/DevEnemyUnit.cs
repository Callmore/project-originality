using System;
using Godot;
using ProjectOriginality.Battle.Units.EnemyAttackScripts;

namespace ProjectOriginality.Battle.Units.UnitClasses
{
    public class DevEnemyUnit : Unit
    {
        public override int BaseMaxHealth { get; } = 10;

        public override UnitSkill BasicAttack { get; set; }
        public override UnitSkill BasicDefence { get; set; }
        public override UnitSkill CharacterSkill { get; set; }

        protected override IEnemyAttack GetAIAttackScript() => new BasicEnemyAttack();
    }
}
