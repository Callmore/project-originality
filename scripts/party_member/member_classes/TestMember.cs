using Godot;
using System;
using System.Collections.Generic;
using ProjectOriginality.Battle.Units;
using ProjectOriginality.Models;
using ProjectOriginality.Enums;
using ProjectOriginality.Resources;

namespace ProjectOriginality.Party.Classes
{
    public class TestMember : PartyMemberodl
    {
        public override int MaxHealth { get; protected set; } = 20;
        public override UnitSkill DefaultAttackSkill { get; } = new UnitSkill(
            name: "Test attack",
            description: "Do a thing.",
            manaCost: 0,
            windup: 0,
            recoveryTime: 5.0,
            usable: SkillUsability.TargetAlive,
            damage: 5
        );
        public override UnitSkill DefaultDefendSkill { get; } = new UnitSkill(
            name: "Test defend",
            description: "Block a thing.",
            manaCost: 0,
            windup: 0,
            recoveryTime: 5.0,
            usable: SkillUsability.TargetAlive,
            target: SkillTarget.Self,
            applyStatuses: new[] { new StatusStack(StatusId.Block, 10) }
        );
        public override UnitSkill DefaultCharacterSkill { get; } = new UnitSkill(
            name: "Test cool skill",
            description: "This is a cool skill.",
            manaCost: 10,
            windup: 2,
            recoveryTime: 6,
            usable: SkillUsability.TargetAlive,
            damage: 1,
            applyStatuses: new[] { new StatusStack(StatusId.Weak) }
        );
        public override List<UnitSkill> LearnableSkills { get; } = new List<UnitSkill>();
        public override PackedScene UnitObject { get; } = GD.Load<PackedScene>("res://objects/battle_unit/units/dev.tscn");
        public override Texture UnitTexture { get; } = GD.Load<Texture>("res://res/unit.png");
    }
}
