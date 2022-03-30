using Godot;
using System;
using ProjectOriginality;
using ProjectOriginality.Party;
using ProjectOriginality.Party.Classes;
using ProjectOriginality.Battle;
using ProjectOriginality.Models;
using System.Linq;
using ProjectOriginality.Resources;

namespace ProjectOriginality.Nodes
{
    public class BattleSelectMenu : Control
    {
        [Export(PropertyHint.File)]
        private string _battleScenePath = "";
        private PackedScene _battleScene;

        [Export]
        private NodePath _partyDisplayContainerNode = "";
        private Node _partyDisplayContainer = null;

        private static bool _initialisedTeam = false;

        // Member position swapping variables
        private bool _isSwappingMembers = false;
        private int _swapTargetIndex = 0;

        public override void _Ready()
        {
            base._Ready();

            if (!_initialisedTeam)
            {
                PlayerStatus.AddPartyMember((PartyMember)GD.Load<PartyMember>("res://resources/party/sword.tres").Duplicate());
                _initialisedTeam = true;
            }

            _battleScene = GD.Load<PackedScene>(_battleScenePath);
            _partyDisplayContainer = GetNode<Node>(_partyDisplayContainerNode);

            // Connnect signals.
            // This is a dumb hack to make it loop with an index...
            Godot.Collections.Array children = _partyDisplayContainer.GetChildren();
            for (int i = 0; i < children.Count; i++)
            {
                PartyDisplayController node = (PartyDisplayController)children[i];
                node.Connect(nameof(PartyDisplayController.MemberPressed), this, nameof(OnPartyMemberButtonPushed), new Godot.Collections.Array { i % 2, i / 2 });
            }

            UpdatePartyDisplay();
        }

        public void OnButtonPush(int buttonID)
        {
            GD.Print($"Got {buttonID}");

            switch (buttonID)
            {
                case 0:
                    BeginBattle(new[,] {
                        {null,null,null},
                        {GD.Load<UnitResource>("res://resources/units/enemy/sword_enemy.tres"),GD.Load<UnitResource>("res://resources/units/enemy/sword_enemy.tres"),GD.Load<UnitResource>("res://resources/units/enemy/sword_enemy.tres")}
                    });
                    break;
                case 1:
                    break;
                case 2:
                    break;
                case 3:
                    break;
                default:
                    throw new InvalidOperationException();
            }
        }

        private void BeginBattle(UnitResource[,] enemyArrangement)
        {
            if (enemyArrangement.GetLength(0) != BattleController.LineCount || enemyArrangement.GetLength(1) != BattleController.LaneCount)
            {
                throw new ArgumentException("Enemy arrangement should be [2,3]");
            }

            Global.NextBattleEnemyArrangement = enemyArrangement;
            GetTree().ChangeSceneTo(_battleScene);
        }

        private void UpdatePartyDisplay()
        {
            // Reset all member display nodes back to default.
            foreach (PartyDisplayController node in _partyDisplayContainer.GetChildren())
            {
                node.ClearDisplay();
            }

            foreach (PartyMember member in PlayerStatus.PlayerParty)
            {
                string loc = $"PartyDisplay{member.BattleLocation.Line}{member.BattleLocation.Lane}";
                PartyDisplayController controller = _partyDisplayContainer.GetNode<PartyDisplayController>(loc);
                controller.UpdateDisplay(member);
            }
        }

        public void OnPartyMemberButtonPushed(int line, int lane)
        {
            BattleLoc targetPoint = new BattleLoc(line, lane);

            GD.Print($"Clicked {targetPoint}");

            // Find member that was clicked on
            int clickedMember = -1;
            for (int i = 0; i < PlayerStatus.PlayerParty.Count; i++)
            {
                if (PlayerStatus.PlayerParty[i].BattleLocation == targetPoint)
                {
                    clickedMember = i;
                    break;
                }
            }

            if (clickedMember == -1 && !_isSwappingMembers)
            {
                // Clicking on an empty slot causes nothing to happen if they aren't trying to move a member.
                return;
            }

            if (!_isSwappingMembers)
            {
                _swapTargetIndex = clickedMember;
                _isSwappingMembers = true;

                GD.Print("Swapping started.");
            }
            else
            {

                if (clickedMember == -1)
                {
                    PlayerStatus.PlayerParty[_swapTargetIndex].BattleLocation = targetPoint;
                }
                else
                {
                    BattleLoc oldPosition = PlayerStatus.PlayerParty[_swapTargetIndex].BattleLocation;
                    PlayerStatus.PlayerParty[_swapTargetIndex].BattleLocation = PlayerStatus.PlayerParty[clickedMember].BattleLocation;
                    PlayerStatus.PlayerParty[clickedMember].BattleLocation = oldPosition;
                }
                _isSwappingMembers = false;

                GD.Print("Swapping finished.");
            }

            UpdatePartyDisplay();
        }
    }
}
