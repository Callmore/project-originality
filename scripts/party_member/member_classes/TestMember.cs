using Godot;
using System;
using System.Collections.Generic;
using ProjectOriginality.Battle.Units;

namespace ProjectOriginality.Party.Classes
{
    public class TestMember : PartyMember
    {
        public override int MaxHealth { get; protected set; } = 5;
        public override List<UnitSkill> LearnableSkills { get; } = new List<UnitSkill>();
        public override PackedScene UnitObject { get; } = GD.Load<PackedScene>("res://objects/battle_unit/units/dev.tscn");
    }
}
