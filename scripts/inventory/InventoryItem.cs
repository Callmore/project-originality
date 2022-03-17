using Godot;
using System;
using ProjectOriginality.Battle.Units;

namespace ProjectOriginality.Inventory
{
    public abstract class InventoryItem : IComparable
    {
        public abstract string ItemName { get; }
        public abstract UnitSkill UseSkill { get; }

        public abstract string IconResourcePath { get; }

        public int CompareTo(object other)
        {
            if (other == null)
                return 1;

            InventoryItem item = other as InventoryItem;
            if (item != null)
            {
                return ItemName.CompareTo(item.ItemName);
            }
            throw new ArgumentException("Other is not an item.");
        }
    }
}
