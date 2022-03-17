using Godot;
using System;
using ProjectOriginality;
using ProjectOriginality.Models;
using ProjectOriginality.Enums;
using ProjectOriginality.Inventory;
using ProjectOriginality.Battle.Units;

namespace ProjectOriginality.Inventory.Items
{
    public class ItemPowder : InventoryItem
    {
        // It's not hard drugs i swear
        public override string ItemName { get; } = "Protein POWder";

        public override UnitSkill UseSkill { get; } = new UnitSkill(
            "", "", 0, 0, 0, (Unit unit) => true, new AttackInfo(target: SkillTarget.Board | SkillTarget.Friendly, applyStatuses: new[] { (StatusId.PowderBuff, -1) })
        );

        public override string IconResourcePath { get; } = "res://res/gameicons/powder.png";
    }
}
