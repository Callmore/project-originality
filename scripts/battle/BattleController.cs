using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProjectOriginality.Battle.Units;
using ProjectOriginality.Battle.Status;
using ProjectOriginality.Enums;
using ProjectOriginality.Models;
using ProjectOriginality.Party;
using ProjectOriginality.Resources;
using System.Text;

namespace ProjectOriginality.Battle
{
    public class BattleController : Node
    {
        [Signal]
        public delegate void ControllerWindupFinished();
        [Signal]
        public delegate void ControllerRecoveredFinished();

        public bool BattleOver { get; private set; } = false;

        private Unit[,] _playerBoard = new Unit[LineCount, LaneCount];
        private Node2D[,] _playerBoardLocators = new Node2D[LineCount, LaneCount];
        private Unit[,] _enemyBoard = new Unit[LineCount, LaneCount];
        private Node2D[,] _enemyBoardLocators = new Node2D[LineCount, LaneCount];

        // Timer finished sets
        public HashSet<Unit> WindupTimerFinished = new HashSet<Unit>();
        public HashSet<Unit> RecoveryTimerFinished = new HashSet<Unit>();
        public Queue<Unit> UnitRecoveryQueue = new Queue<Unit>();

        // Timer pausing attrbutes
        public bool TimerPaused { get; private set; } = false;
        public bool InventoryOpen { get; private set; } = false;

        private static readonly PackedScene _objLocator = GD.Load<PackedScene>("res://objects/battle_locator/battle_locator.tscn");
        private static readonly PackedScene _objSkillMenu = GD.Load<PackedScene>("res://objects/battle_skill_menu/battle_skill_menu.tscn");
        private static readonly PackedScene _objUnitBattleUI = GD.Load<PackedScene>("res://objects/battle_unit_ui/battle_unit_ui.tscn");

        private static readonly PackedScene _scnBattleSelect = GD.Load<PackedScene>("res://scenes/battle_select.tscn");

        private const string GroupLocatorPlayer = "player_battle_locator";
        private const string GroupLocatorEnemy = "enemy_battle_locator";

        private const string GroupUnit = "unit";

        public const int LineCount = 2;
        public const int LaneCount = 3;

        private Unit SpawnUnit(int x, int y, bool enemy, UnitResource unitRes, int level = 1)
        {
            Unit unit = Unit.FromResource(unitRes);
            if (enemy)
            {
                _enemyBoardLocators[x, y].AddChild(unit);
                _enemyBoard[x, y] = unit;
                unit.Enemy = true;
            }
            else
            {
                _playerBoardLocators[x, y].AddChild(unit);
                _playerBoard[x, y] = unit;
            }
            unit.Controller = this;
            unit.OnWindupTimerFinish = OnUnitDoSkill;
            unit.OnRecoveryTimerFinish = OnUnitRecover;
            unit.OnDieFunc = OnUnitDie;
            unit.AddToGroup(GroupUnit);

            // Spawn a unit UI and add it to the hud layer.
            UnitUI unitUI = _objUnitBattleUI.Instance<UnitUI>();
            GetNode<CanvasLayer>("HudLayer").AddChild(unitUI);
            Transform2D transform = unit.GetGlobalTransformWithCanvas();
            unitUI.RectPosition = transform.origin;
            unitUI.InitHealthBar(unit.MaxHealth, unit.MaxHealth);

            unit.Connect(nameof(Unit.HealthModified), unitUI, nameof(UnitUI.OnHealthChanged));
            unit.Connect(nameof(Unit.MaxHealthModified), unitUI, nameof(UnitUI.OnMaxHealthChanged));
            unit.StatusEffectController.Connect(nameof(StatusEffectController.OnStatusAdded), unitUI, nameof(UnitUI.OnStatusAdded));

            return unit;
        }

        private Unit SpawnPartyMember(PartyMember member)
        {
            Unit unit = SpawnUnit(member.BattleLocation.Line, member.BattleLocation.Lane, false, member.UnitRes);
            unit.UpdateStatsFromPartyMember(member);

            return unit;
        }

        private void SpawnBattlePoints()
        {
            for (BoardSide side = BoardSide.Player; side <= BoardSide.Enemy; side++)
            {
                for (int x = 0; x < LineCount; x++)
                {
                    for (int y = 0; y < LaneCount; y++)
                    {
                        Node2D locator = _objLocator.Instance<Node2D>();
                        locator.AddToGroup(side == BoardSide.Player ? GroupLocatorPlayer : GroupLocatorEnemy);
                        int sideOffset = side == BoardSide.Enemy ? y * 32 : y * -32;
                        locator.Position = new Vector2(192 + x * 128 + (int)side * 512 + sideOffset, 128 + y * 128);
                        AddChild(locator);
                        switch (side)
                        {
                            case BoardSide.Player:
                                _playerBoardLocators[x, y] = locator;
                                break;
                            case BoardSide.Enemy:
                                _enemyBoardLocators[1 - x, y] = locator;
                                break;
                        }
                    }
                }
            }
        }

        private void SpawnPlayerUnits()
        {
            // TODO: Get the player's units.
            //Unit unit = SpawnUnit(1, 1, false, GD.Load<PackedScene>("res://objects/battle_unit/units/dev.tscn"));

            foreach (PartyMember member in PlayerStatus.PlayerParty)
            {
                SpawnPartyMember(member);
            }
        }

        private void SpawnEnemyUnits()
        {
            // TODO: Make encounters and have units positioned in specific locations

            for (int x = 0; x < LineCount; x++)
            {
                for (int y = 0; y < LaneCount; y++)
                {
                    if (Global.NextBattleEnemyArrangement[x, y] != null)
                    {
                        SpawnUnit(x, y, true, Global.NextBattleEnemyArrangement[x, y]); //;
                    }
                }
            }
        }

        private void SpawnAllUnits()
        {
            SpawnPlayerUnits();
            SpawnEnemyUnits();
        }

        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
        {
            SpawnBattlePoints();
            SpawnAllUnits();
        }

        public bool IsTimerPaused()
        {
            return TimerPaused || InventoryOpen;
        }

        public async void CheckForTimerFinishedAsync()
        {
            if (IsTimerPaused())
            {
                return;
            }

            if (WindupTimerFinished.Count > 0)
            {
                TimerPaused = true;
                foreach (Unit unit in WindupTimerFinished.OrderBy(u => u.AttackWindupTimer))
                {
                    await unit.AttackWindupTimerFinished();
                }
                WindupTimerFinished.Clear();
            }

            if (RecoveryTimerFinished.Count > 0)
            {
                TimerPaused = true;
                foreach (Unit unit in RecoveryTimerFinished.OrderBy(u => u.AttackRecoveryTimer).ThenBy(u => u.Enemy))
                {
                    UnitRecoveryQueue.Enqueue(unit);
                }
                RecoveryTimerFinished.Clear();
            }
            TimerPaused = false;
        }

        // Called every frame. 'delta' is the elapsed time since the previous frame.
        public override void _Process(float delta)
        {
            base._Process(delta);

            CheckForTimerFinishedAsync();

            if (!IsTimerPaused() && UnitRecoveryQueue.Count > 0)
            {
                TimerPaused = true;
                Unit unit = UnitRecoveryQueue.Dequeue();
                unit.AttackRecoverTimerFinished();
            }
        }

        public async void OnUnitDoSkill(Unit unit)
        {
            GD.Print($"{unit}: Use skill function. ({unit.QueuedSkill.Name})");

            UnitSkill usedSkill = unit.QueuedSkill;

            // Figure out which board we are targeting
            BoardSide targetBoard = unit.Enemy ? BoardSide.Player : BoardSide.Enemy;
            if (usedSkill.Target == SkillTarget.Self)
            {

            }
            else
            {
                /* TODO: Implement more target options
				if (usedSkill.Activate.Target == SkillTarget.Single)
				{

				}
				*/

            }


            // HELL YEAH WE'RE JUST GONNA ELSEIF THIS MESS
            SkillTarget target = usedSkill.Target;
            if (target.HasFlag(SkillTarget.Self))
            {
                ApplyAttackInfo(unit, usedSkill);
            }
            else if (target.HasFlag(SkillTarget.AllSelected))
            {
                // TODO: Apply the attack information to all cells selectable with an enemy in it.
            }
            else if (false)
            {
                // TODO: Figure out more targeting modes
            }
            else
            {
                // Nothing else matched, this is a single target.
                Unit targetUnit = GetUnitAt(targetBoard, unit.SkillBoardTarget);
                Global.Assert(targetUnit != unit);
                ApplyAttackInfo(targetUnit, usedSkill, unit.GetAttackModifierCalc()); // TODO: Rescale damage based on unit's damage bonus.
            }

            await ToSignal(GetTree().CreateTimer(1), "timeout");
            EmitSignal(nameof(ControllerWindupFinished));
        }

        public void UseItem(Inventory.InventoryItem item, int x, int y)
        {
            GD.Print(item);
            BoardSide targetBoard = BoardSide.Enemy;
            if (item.UseSkill.Target.HasFlag(SkillTarget.Friendly))
            {
                targetBoard = BoardSide.Player;
            }
            ApplyAttackInfo(GetUnitAt(targetBoard, x, y), item.UseSkill);
        }

        private bool ApplyAttackInfo(Unit unit, UnitSkill skill, BuffCalculator buff = null)
        {
            if (unit != null)
            {
                if (skill.Damage > 0)
                {
                    unit.Hurt(buff?.Calculate(skill.Damage) ?? skill.Damage);
                }

                if (skill.Heal > 0)
                {
                    unit.Heal(skill.Heal);
                }

                foreach (var status in skill.Statuses.AsEnumerable())
                {
                    unit.ApplyStatus(status.Status, status.Stacks);
                }

                return true;
            }
            return false;
        }

        public BattleLoc GetRandomEnemyTargetPosition()
        {
            // Get all cells that contain something
            List<BattleLoc> targets = new List<BattleLoc>();
            for (int x = 0; x < LineCount; x++)
            {
                for (int y = 0; y < LaneCount; y++)
                {
                    if (_playerBoard[x, y] != null && !_playerBoard[x, y].Dead)
                    {
                        targets.Add(new BattleLoc(x, y));
                    }
                }
            }

            Global.Assert(targets.Count != 0, "targets is {}");

            // TODO: colapse since this is only needed to fix a bug
            int idx = Global.Rng.RandiRange(0, targets.Count - 1); //(int)GD.Randi() % targets.Count;

            BattleLoc target;
            try
            {
                target = targets[idx];
            }
            catch (Exception e)
            {
                if (e is ArgumentOutOfRangeException)
                {
                    GD.Print("Got out of range!");
                    GD.Print($"Index: {idx}");
                    GD.Print($"Values {string.Join(", ", targets)}");
                }
                throw;
            }

            return target;
        }

        public (BoardSide, BattleLoc) FindUnitLocation(Unit unit)
        {
            for (int x = 0; x < LineCount; x++)
            {
                for (int y = 0; y < LaneCount; y++)
                {
                    if (_playerBoard[x, y] == unit)
                    {
                        return (BoardSide.Player, new BattleLoc(x, y));
                    }
                    else if (_enemyBoard[x, y] == unit)
                    {
                        return (BoardSide.Enemy, new BattleLoc(x, y));
                    }
                }
            }
            throw new InvalidOperationException($"Unknown unit {unit}.");
        }

        private void SaveUnitsToPartyMembers()
        {
            for (int x = 0; x < _playerBoard.GetLength(0); x++)
            {
                for (int y = 0; y < _playerBoard.GetLength(1); y++)
                {
                    if (_playerBoard[x, y] != null)
                    {
                        _playerBoard[x, y].UpdatePartyMember();
                    }
                }
            }
        }

        private async void EndBattle(BoardSide winner)
        {
            PauseBattleTimers();
            GD.Print($"{winner} win.");

            switch (winner)
            {
                case BoardSide.Player:
                    GetNode<AnimationPlayer>("BattleAnimation").Play("WinAnimation");
                    break;
                case BoardSide.Enemy:
                    GetNode<AnimationPlayer>("BattleAnimation").Play("LoseAnimation");
                    break;
            }

            BattleOver = true;

            await ToSignal(GetNode<AnimationPlayer>("BattleAnimation"), "animation_finished");
            // Usually there would be something that would happen here but we're just gonna load the battle select menu and exit this mess.

            if (winner == BoardSide.Player)
            {
                SaveUnitsToPartyMembers();
            }
            GetTree().ChangeSceneTo(_scnBattleSelect);
        }

        private bool IsUnitAlive(Unit unit)
        {
            return unit != null && unit.Health > 0;
        }

        private void OnUnitDie(Unit unit)
        {
            (BoardSide side, BattleLoc unitLocation) = FindUnitLocation(unit);
            switch (side)
            {
                case BoardSide.Player:
                    //_playerBoard[unitLocation.Line, unitLocation.Lane] = null;
                    break;
                case BoardSide.Enemy:
                    //_enemyBoard[unitLocation.Line, unitLocation.Lane] = null;
                    break;
            }

            // Check if either side has ran out of units.
            if (_enemyBoard.Cast<Unit>().Count(IsUnitAlive) <= 0)
            {
                EndBattle(BoardSide.Player);
                return;
            }
            else if (_playerBoard.Cast<Unit>().Count(IsUnitAlive) <= 0)
            {
                EndBattle(BoardSide.Enemy);
                return;
            }
        }

        public Unit GetUnitAt(BoardSide side, int x, int y)
        {
            switch (side)
            {
                case BoardSide.Player:
                    return _playerBoard[x, y];
                case BoardSide.Enemy:
                    return _enemyBoard[x, y];
                default:
                    throw new InvalidOperationException($"Unknown {nameof(BoardSide)} {side}");
            }
        }

        public Unit GetUnitAt(BoardSide side, BattleLoc position)
        {
            return GetUnitAt(side, position.Line, position.Lane);
        }

        public void PauseBattleTimers(bool paused = true)
        {
            /*
				//GD.Print($"Paused = {paused}");

				foreach (Unit node in GetTree().GetNodesInGroup("unit"))
				{
					if (IsUnitAlive(node))
					{
						node.GetNode<Timer>("AttackWindupTimer").Paused = paused;
						node.GetNode<Timer>("AttackRecoverTimer").Paused = paused;
					}
				}
			*/
        }

        public void ResumeBattle()
        {
            TimerPaused = false;
        }

        public void OnInventoryToggled(bool opened)
        {
            InventoryOpen = opened;
        }

        #region Unit skill menu

        private Unit _unitWaitingForSkill;

        public void OnUnitRecover(Unit unit)
        {
            //GD.Print($"{unit}: Recover function.");

            _unitWaitingForSkill = unit;
            ActivateUnitActMenu(unit);

            //await ToSignal(GetTree(), "idle_frame");
            //EmitSignal(nameof(ControllerRecoveredFinished));
        }

        /// <summary>
        /// Activates the skill selection menu for given unit.
        /// </summary>
        private void ActivateUnitActMenu(Unit unit)
        {
            // TODO: Spawn an action pick menu with options on what to do.
            // Also link the correct signals back to this object to run and update the waiting object.
            /*
			var skillMenu = _objSkillMenu.Instance<BattleSkillMenuController>();
			GetParent().CallDeferred("add_child", skillMenu);
			skillMenu.Connect(nameof(BattleSkillMenuController.BattleSkillMenuUsedSkill), this, nameof(HandleSkillMenuOptionSelected), flags: (uint)ConnectFlags.Oneshot);
			*/
            var skillMenu = GetNode<BattleSkillMenuController>("HudLayer/BattleSkillMenuController");
            skillMenu.Connect(nameof(BattleSkillMenuController.BattleSkillMenuUsedSkill), this, nameof(HandleSkillMenuOptionSelected), flags: (uint)ConnectFlags.Oneshot);
            skillMenu.StartSkillMenu(unit);
        }

        public void HandleSkillMenuOptionSelected(SkillSlot skill, int targetX, int targetY)
        {
            Global.Assert(IsInstanceValid(_unitWaitingForSkill));

            _unitWaitingForSkill.UseSkill(skill, new BattleLoc(targetX, targetY));
        }

        #endregion
    }
}
