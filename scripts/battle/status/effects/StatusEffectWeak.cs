using System;
using Godot;
using ProjectOriginality.Battle.Status;
using ProjectOriginality.Enums;
using ProjectOriginality.Models;

public class StatusEffectWeak : TimedStatusEffect
{
    public override StatusId StatusId { get; } = StatusId.Weak;
    public override int DefaultStacks { get; } = 10;

    public override IStatBuff DefenseModifier { get; } = new BuffAdditive(0.25);

    public override void _Ready()
    {
        base._Ready();
    }

    public override void OnUnitReceveAttack(object sender, AttackEventArgs e)
    {
        base.OnUnitReceveAttack(sender, e);
    }
}
