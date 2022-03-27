using Godot;
using System;
using System.Collections.Generic;
using ProjectOriginality.Battle.Units;
using ProjectOriginality.Models;
using ProjectOriginality.Enums;

namespace ProjectOriginality.Party.Classes
{
    public class TestMember : PartyMember
    {
        public override int MaxHealth { get; protected set; } = 20;
        public override UnitSkill DefaultAttackSkill { get; } = new UnitSkill(
            name: "Test attack",
            description: "Do a thing.",
            manaCost: 0,
            windup: 0,
            cooldown: 5.0,
            usable: UnitCheckFunctions.AliveCheck,
            activate: new AttackInfo(damage: 5)
        );
        public override UnitSkill DefaultDefendSkill { get; } = new UnitSkill(
            name: "Test defend",
            description: "Block a thing.",
            manaCost: 0,
            windup: 0,
            cooldown: 5.0,
            usable: UnitCheckFunctions.AliveCheck,
            activate: new AttackInfo(target: SkillTarget.Self, applyStatuses: new[] { (StatusId.Block, 10) })
        );
        public override UnitSkill DefaultCharacterSkill { get; } = new UnitSkill(
            name: "Test cool skill",
            description: "This is a cool skill.",
            manaCost: 10,
            windup: 2,
            cooldown: 6,
            usable: UnitCheckFunctions.AliveCheck,
            activate: new AttackInfo(damage: 1, applyStatuses: new[] { (StatusId.Weak, -1) })
        );
        public override List<UnitSkill> LearnableSkills { get; } = new List<UnitSkill>();
        public override PackedScene UnitObject { get; } = GD.Load<PackedScene>("res://objects/battle_unit/units/dev.tscn");
    }
}
