using Godot;
using System;
using ProjectOriginality;
using ProjectOriginality.Party;
using ProjectOriginality.Party.Classes;
using ProjectOriginality.Battle;

namespace ProjectOriginality.Nodes
{
    public class BattleSelectMenu : Control
    {
        [Export]
        private PackedScene _battleScene = null;

        private bool _initialisedTeam = false;

        public override void _Ready()
        {
            base._Ready();

            _initialisedTeam = true;
            PlayerStatus.AddPartyMember(new TestMember());
        }

        public void OnButtonPush(int buttonID)
        {
            GD.Print($"Got {buttonID}");

            switch (buttonID)
            {
                case 0:
                    BeginBattle(new[,] {
                        {null,null,null},
                        {GD.Load<PackedScene>("res://objects/battle_unit/units/dev_enemy.tscn"),GD.Load<PackedScene>("res://objects/battle_unit/units/dev_enemy.tscn"),GD.Load<PackedScene>("res://objects/battle_unit/units/dev_enemy.tscn")}
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

        private void BeginBattle(PackedScene[,] enemyArrangement)
        {
            if (enemyArrangement.GetLength(0) != BattleController.LineCount || enemyArrangement.GetLength(1) != BattleController.LaneCount)
            {
                throw new ArgumentException("Enemy arrangement should be [2,3]");
            }

            Global.NextBattleEnemyArrangement = enemyArrangement;
            GetTree().ChangeSceneTo(_battleScene);
        }
    }
}
