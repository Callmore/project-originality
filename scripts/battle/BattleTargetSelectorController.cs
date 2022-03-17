using Godot;
using System;
using ProjectOriginality;

namespace ProjectOriginality.Battle
{
    public class BattleTargetSelectorController : Control
    {
        [Signal]
        public delegate void Canceled();

        [Signal]
        public delegate void PressedLocation(int x, int y);

        public void OnCancelPressed()
        {
            EmitSignal(nameof(Canceled));
            QueueFree();
        }

        public void OnLocationPressed(int x, int y)
        {
            EmitSignal(nameof(PressedLocation), x, y);
            QueueFree();
        }
    }
}
