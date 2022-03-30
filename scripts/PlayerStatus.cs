using Godot;
using System;
using System.Collections.Generic;
using ProjectOriginality.Party;
using ProjectOriginality.Inventory;
using ProjectOriginality.Inventory.Items;
using System.Collections.ObjectModel;
using ProjectOriginality.Models;
using System.Linq;

namespace ProjectOriginality
{
    public static class PlayerStatus
    {
        public const int MaxInventorySize = 8;

        private static List<PartyMember> _playerParty = new List<PartyMember>();
        public static ReadOnlyCollection<PartyMember> PlayerParty { get; } = new ReadOnlyCollection<PartyMember>(_playerParty);
        public static int Money { get; set; }

        public static List<InventoryItem> Inventory { get; } = new List<InventoryItem>()
        {
            new ItemHeal(),
            new ItemHeal(),
            new ItemHeal(),
            new ItemPowder(),
        };

        private static readonly BattleLoc[] _preferedSpotOrder = new BattleLoc[] {
            new BattleLoc(1, 1),
            new BattleLoc(1, 2),
            new BattleLoc(1, 0),
            new BattleLoc(0, 1),
            new BattleLoc(0, 2),
            new BattleLoc(0, 0),
        };

        public static void SortInventory()
        {
            Inventory.Sort();
        }

        /// <summary>
        /// Add a new party member to the player's party.
        /// </summary>
        /// <param name="member">Initialised party member to add.</param>
        public static void AddPartyMember(PartyMember member)
        {
            BattleLoc partyLocation = FindFreeSpot();
            _playerParty.Add(member);
            member.BattleLocation = partyLocation;
        }

        public static void AddPartyMember(PartyMemberResource resource)
        {
            AddPartyMember(PartyMember.FromResource(resource));
        }

        /// <summary>
        /// Find a free slot in the party member list.
        /// </summary>
        /// <returns>A free point.</returns>
        /// <exception cref="InvalidOperationException"></exception>
        private static BattleLoc FindFreeSpot()
        {
            foreach (BattleLoc point in _preferedSpotOrder)
            {
                if (!_playerParty.Any(member => member.BattleLocation == point))
                {
                    return point;
                }
            }
            throw new InvalidOperationException("No avalible slots to find.");
        }
    }
}
