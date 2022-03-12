using System;
using System.Collections.Generic;
using Godot;
using ProjectOriginality.Enums;
using ProjectOriginality.Models;
using ProjectOriginality.Battle.Units;

namespace ProjectOriginality.Battle.Status
{
    public struct AttackEventArgs
    {
        public AttackInfo AttackInfo;
    }

    public class StatusEffectController : Node
    {
        [Signal]
        public delegate void OnStatusAdded(StatusEffect effect);

        public event EventHandler OnPause;
        public event EventHandler OnUnpause;

        public event EventHandler<AttackEventArgs> OnUnitReceveAttack;

        public static Dictionary<StatusId, PackedScene> StatusIdToResource = new Dictionary<StatusId, PackedScene>()
        {
            [StatusId.Block] = GD.Load<PackedScene>("res://objects/status_effect/effects/block.tscn"),
            [StatusId.Weak] = GD.Load<PackedScene>("res://objects/status_effect/effects/weak.tscn")
        };

        public override void _Process(float delta)
        {
            base._Process(delta);

            foreach (StatusEffect status in GetChildren())
            {
                if (status.ShouldBeRemoved())
                {
                    RemoveStatusEffect(status);
                }
            }
        }

        private void AddStatusEffect(PackedScene statusPacked, int stacks)
        {
            StatusEffect statusObject = statusPacked.Instance<StatusEffect>();
            statusObject.OwningUnit = GetParent<Unit>();
            if (stacks != -1)
            {
                statusObject.InitStacks(stacks);
            }
            AddChild(statusObject);

            OnPause += statusObject.Pause;
            OnUnpause += statusObject.Unpause;

            OnUnitReceveAttack += statusObject.OnUnitReceveAttack;

            EmitSignal(nameof(OnStatusAdded), statusObject);
        }

        private void RemoveStatusEffect(StatusEffect status)
        {
            OnPause -= status.Pause;
            OnUnpause -= status.Unpause;

            OnUnitReceveAttack -= status.OnUnitReceveAttack;

            status.QueueFree();
        }

        public void ApplyStatus(StatusId id, int stacks = -1)
        {
            GD.Print($"APPLYING STATUS {id}");
            if (!StatusIdToResource.ContainsKey(id))
            {
                throw new ArgumentException($"Unknown status ID {nameof(id)}");
            }

            IncreaseStatusEffect(id, stacks);
        }

        /// <summary>
        /// Returns if the controller has status effect listed.
        /// </summary>
        /// <param name="id">Status effect to check for.</param>
        /// <returns></returns>
        public bool CheckStatusEffect(StatusId id)
        {
            return GetStatusEffectOrNull(id) != null;
        }

        private void IncreaseStatusEffect(StatusId id, int stacks)
        {
            StatusEffect status = GetStatusEffectOrNull(id);
            if (status != null)
            {
                status.AddStacks(stacks);
            }
            else
            {
                AddStatusEffect(StatusIdToResource[id], stacks);
            }
        }

        private StatusEffect GetStatusEffectOrNull(StatusId id)
        {
            foreach (StatusEffect status in GetChildren())
            {
                if (status.StatusId == id)
                {
                    return status;
                }
            }
            return null;
        }

        public void PauseStatuses()
        {
            OnPause?.Invoke(this, null);
        }

        public void UnpauseStatuses()
        {
            OnUnpause?.Invoke(this, null);
        }

        public AttackInfo CallOnUnitReceveAttack(AttackInfo orignal)
        {
            GD.Print("STATUS EFFECTING");
            AttackEventArgs eventArgs = new AttackEventArgs() { AttackInfo = orignal };
            OnUnitReceveAttack?.Invoke(this, eventArgs);
            return eventArgs.AttackInfo;
        }
    }
}
