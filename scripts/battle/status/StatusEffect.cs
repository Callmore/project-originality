using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Godot;
using ProjectOriginality.Battle.Units;
using ProjectOriginality.Enums;

namespace ProjectOriginality.Battle.Status
{
    public abstract class StatusEffect : Node
    {
        [Export]
        public readonly string EffectName = "";
        [Export(PropertyHint.MultilineText)]
        public readonly string EffectDescription = "";
        [Export]
        public readonly Texture Icon;

        public int Stacks { get; set; }
        public virtual int DefaultStacks { get; } = 1;
        public Unit OwningUnit { get; set; }
        public abstract StatusId StatusId { get; }

        public override void _Ready()
        {
            base._Ready();
        }

        public virtual void OnUnitReceveAttack(object sender, AttackEventArgs e) { }

        public virtual void OnUnitDealDamage(object sender, AttackEventArgs e) { }

        public virtual void Pause(object sender, EventArgs e) { }
        public virtual void Unpause(object sender, EventArgs e) { }

        public virtual bool ShouldBeRemoved()
        {
            return Stacks <= 0;
        }

        public virtual void AddStacks(int amount)
        {
            Stacks += amount;
        }

        public virtual void InitStacks(int amount)
        {
            Stacks = amount;
        }
    }
}
