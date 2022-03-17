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
    public abstract class TimedStatusEffect : StatusEffect
    {
        [Export]
        public double TickTime = 1;
        public double Timer = 0;

        public override void Tick(double delta)
        {
            base.Tick(delta);

            Timer -= delta;
            if (Timer <= 0)
            {
                OnTick();
                Timer += TickTime;
            }
        }

        protected virtual void OnTick()
        {
            Stacks--;
            if (Stacks <= 0)
            {
                RemoveStatus();
            }
        }
    }
}
