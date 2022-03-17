using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Godot;
using ProjectOriginality.Battle.Units;
using ProjectOriginality.Enums;
using ProjectOriginality.Models;

namespace ProjectOriginality.Battle.Status
{
    public abstract class StatusEffect : Node
    {
        [Signal]
        public delegate void Removed();
        [Signal]
        public delegate void StackCountModified(int newAmount);

        [Export]
        public readonly string EffectName = "";
        [Export(PropertyHint.MultilineText)]
        public readonly string EffectDescription = "";
        [Export]
        public readonly Texture Icon;

        public int Stacks
        {
            get => _stacks;
            set
            {
                _stacks = value;
                EmitSignal(nameof(StackCountModified), value);
            }
        }
        private int _stacks;


        public virtual int DefaultStacks { get; } = 1;
        public Unit OwningUnit { get; set; }
        public abstract StatusId StatusId { get; }

        // Stat changes
        public virtual IStatBuff AttackModifier { get; } = new BuffAdditive();
        public virtual IStatBuff DefenseModifier { get; } = new BuffAdditive();

        public override void _Ready()
        {
            base._Ready();

            InitStacks(DefaultStacks);
        }

        public virtual void OnUnitReceveAttack(object sender, AttackEventArgs e) { }

        public virtual void OnUnitDealDamage(object sender, AttackEventArgs e) { }

        public bool Paused = false;

        public virtual void Tick(double delta) { }

        public virtual bool ShouldBeRemoved()
        {
            return Stacks <= 0;
        }

        public virtual void AddStacks(int amount)
        {
            GD.Print($"{amount}, {DefaultStacks}");
            if (amount < 0)
            {
                Stacks += DefaultStacks;
            }
            else
            {
                Stacks += amount;
            }
        }

        public virtual void InitStacks(int amount)
        {
            Stacks = amount;
        }

        public virtual void RemoveStatus()
        {
            EmitSignal(nameof(Removed));
            QueueFree();
        }
    }
}
