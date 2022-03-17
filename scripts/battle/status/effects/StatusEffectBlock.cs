using System;
using Godot;
using ProjectOriginality.Battle.Status;
using ProjectOriginality.Enums;
using ProjectOriginality.Models;

public class StatusEffectBlock : TimedStatusEffect
{
    public override StatusId StatusId { get; } = StatusId.Block;
    public override int DefaultStacks { get; } = 10;

    public override IStatBuff DefenseModifier { get; } = new BuffMultipicitive(0.50);

    public override void _Ready()
    {
        base._Ready();
    }

    public override void OnUnitReceveAttack(object sender, AttackEventArgs e)
    {
        base.OnUnitReceveAttack(sender, e);
    }
}
