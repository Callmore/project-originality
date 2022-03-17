using System;
using Godot;
using ProjectOriginality.Battle.Status;
using ProjectOriginality.Enums;
using ProjectOriginality.Models;

public class StatusEffectPowderBuff : TimedStatusEffect
{
    public override StatusId StatusId { get; } = StatusId.PowderBuff;
    public override int DefaultStacks { get; } = 10;

    public override IStatBuff AttackModifier { get; } = new BuffAdditive(0.5);

    public override void _Ready()
    {
        base._Ready();
    }

    public override void OnUnitReceveAttack(object sender, AttackEventArgs e)
    {
        base.OnUnitReceveAttack(sender, e);
    }
}
