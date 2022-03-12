using Godot;
using System;
using System.Collections.Generic;
using ProjectOriginality.Party;
using ProjectOriginality.Inventory;

namespace ProjectOriginality
{
    public static class PlayerStatus
    {
        public const int MaxInventorySize = 8;

        public static List<PartyMember> PlayerParty { get; } = new List<PartyMember>();
        public static int Money { get; set; }
        public static List<InventoryItem> Inventory { get; } = new List<InventoryItem>();
    }
}
