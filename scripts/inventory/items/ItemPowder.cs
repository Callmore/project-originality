using Godot;
using System;
using ProjectOriginality;
using ProjectOriginality.Models;
using ProjectOriginality.Enums;
using ProjectOriginality.Inventory;
using ProjectOriginality.Battle.Units;
using ProjectOriginality.Resources;

namespace ProjectOriginality.Inventory.Items
{
    public class ItemPowder : InventoryItem
    {
        // It's not hard drugs i swear
        public override string ItemName { get; } = "Protein POWder";

        public override UnitSkill UseSkill { get; } = new UnitSkill(
            usable: SkillUsability.TargetAlive,
            target: SkillTarget.Board | SkillTarget.Friendly,
            applyStatuses: new StatusStack[] { new StatusStack(StatusId.PowderBuff) }
        );

        public override string IconResourcePath { get; } = "res://res/gameicons/powder.png";
    }
}
