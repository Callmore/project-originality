using System;
using System.Collections.Generic;
using Godot;
using ProjectOriginality.Enums;
using ProjectOriginality.Models;
using ProjectOriginality.Battle.Units;
using System.Linq;
using ProjectOriginality.Resources;

namespace ProjectOriginality.Battle.Status
{
    public struct AttackEventArgs
    {
        public UnitSkill Skill;
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
            [StatusId.Weak] = GD.Load<PackedScene>("res://objects/status_effect/effects/weak.tscn"),
            [StatusId.PowderBuff] = GD.Load<PackedScene>("res://objects/status_effect/effects/powder_buff.tscn"),
        };

        public void Tick(float delta)
        {
            foreach (StatusEffect status in GetChildren())
            {
                status.Tick(delta);
                if (status.ShouldBeRemoved())
                {
                    //RemoveStatusEffect(status);
                }
            }
        }

        private void AddStatusEffect(PackedScene statusPacked, int stacks)
        {
            StatusEffect statusObject = statusPacked.Instance<StatusEffect>();
            AddChild(statusObject);

            statusObject.OwningUnit = GetParent<Unit>();
            GD.Print(stacks);
            if (stacks != -1)
            {
                statusObject.InitStacks(stacks);
            }

            OnUnitReceveAttack += statusObject.OnUnitReceveAttack;

            EmitSignal(nameof(OnStatusAdded), statusObject);
        }

        private void RemoveStatusEffect(StatusEffect status)
        {
            status.QueueFree();
        }

        public void ApplyStatus(StatusId id, int stacks = -1)
        {
            GD.Print($"APPLYING STATUS {id}");
            if (!StatusIdToResource.ContainsKey(id))
            {
                throw new ArgumentException($"Unknown status ID {id}");
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
            GD.Print("Increasing");
            StatusEffect status = GetStatusEffectOrNull(id);
            GD.Print(status);
            if (status != null)
            {
                status.AddStacks(stacks);
            }
            else
            {
                GD.Print("ADDING STATUS!!!");
                AddStatusEffect(StatusIdToResource[id], stacks);
            }
        }

        public StatusEffect GetStatusEffectOrNull(StatusId id)
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

        public HashSet<(StatusId, int)> GetAllStatusesAndStacks()
        {
            return GetChildren().Cast<StatusEffect>().Select(status => (status.StatusId, status.Stacks)).ToHashSet();
        }

        public void PauseStatuses()
        {
            OnPause?.Invoke(this, null);
        }

        public void UnpauseStatuses()
        {
            OnUnpause?.Invoke(this, null);
        }
    }
}
