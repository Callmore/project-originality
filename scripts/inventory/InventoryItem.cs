using Godot;
using System;
using ProjectOriginality.Battle.Units;

namespace ProjectOriginality.Inventory
{
    public abstract class InventoryItem
    {
        public abstract UnitSkill UseSkill { get; }
    }
}
