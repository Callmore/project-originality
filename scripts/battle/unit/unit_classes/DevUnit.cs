using System;
using Godot;
using ProjectOriginality.Battle.Units.EnemyAttackScripts;
using ProjectOriginality.Models;
using ProjectOriginality.Enums;

namespace ProjectOriginality.Battle.Units.UnitClasses
{
    public class DevUnit : Unit
    {
        public override int BaseMaxHealth { get; } = 20;

        public override UnitSkill BasicAttack { get; set; } = new UnitSkill(
            name: "Test attack",
            description: "Do a thing.",
            manaCost: 0,
            windup: 0,
            cooldown: 5.0,
            usable: UnitCheckFunctions.AliveCheck,
            activate: new AttackInfo(damage: 5)
        );
        public override UnitSkill BasicDefence { get; set; } = new UnitSkill(
            name: "Test defend",
            description: "Block a thing.",
            manaCost: 0,
            windup: 0,
            cooldown: 5.0,
            usable: UnitCheckFunctions.AliveCheck,
            activate: new AttackInfo(target: SkillTarget.Self, applyStatuses: new[] { (StatusId.Block, 10) })
        );
        public override UnitSkill CharacterSkill { get; set; } = new UnitSkill(
            name: "Test cool skill",
            description: "This is a cool skill.",
            manaCost: 10,
            windup: 2,
            cooldown: 6,
            usable: UnitCheckFunctions.AliveCheck,
            activate: new AttackInfo(damage: 10, applyStatuses: new[] { (StatusId.Weak, -1) })
        );

        protected override IEnemyAttack GetAIAttackScript() => null;
    }
}
