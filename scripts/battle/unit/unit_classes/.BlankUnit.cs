using System;
using Godot;
using ProjectOriginality.Battle.Units.EnemyAttackScripts;

namespace ProjectOriginality.Battle.Units.UnitClasses
{
    public class BlankUnit : Unit
    {
        public override int BaseMaxHealth { get; } = 5;

        public override UnitSkill BasicAttack { get; set; } = new UnitSkill();
        public override UnitSkill BasicDefence { get; set; } = new UnitSkill();
        public override UnitSkill CharacterSkill { get; set; } = new UnitSkill();

        protected override IEnemyAttack GetAIAttackScript() => null;
    }
}
