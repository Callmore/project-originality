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
    public class ItemHeal : InventoryItem
    {
        public override string ItemName { get; } = "Heal";

        public override UnitSkill UseSkill { get; } = new UnitSkill(
                name: "", description: "", usable: SkillUsability.TargetMissingHealth, heal: 10, target: SkillTarget.Board | SkillTarget.Friendly
            );

        public override string IconResourcePath { get; } = "res://res/gameicons/potion-ball.png";
    }
}
