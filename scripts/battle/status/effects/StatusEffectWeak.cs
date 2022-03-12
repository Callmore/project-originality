using System;
using Godot;
using ProjectOriginality.Battle.Status;
using ProjectOriginality.Enums;

public class StatusEffectWeak : StatusEffect
{
    public override StatusId StatusId { get; } = StatusId.Weak;
    public override int DefaultStacks { get; } = 10;

    private Timer _decrementTimer;

    public override void _Ready()
    {
        base._Ready();

        _decrementTimer = GetNode<Timer>("DecrementTimer");
    }

    public override void OnUnitReceveAttack(object sender, AttackEventArgs e)
    {
        base.OnUnitReceveAttack(sender, e);
    }

    public override void Pause(object sender, EventArgs e)
    {
        base.Pause(sender, e);

        _decrementTimer.Paused = true;
    }

    public override void Unpause(object sender, EventArgs e)
    {
        base.Unpause(sender, e);

        _decrementTimer.Paused = false;
    }
}
