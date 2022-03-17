using System;
using Godot;
using ProjectOriginality.Battle.Units;
using ProjectOriginality.Enums;

namespace ProjectOriginality.Battle
{
    public class BattleSkillMenuController : Control
    {
        [Signal]
        public delegate void BattleSkillMenuUsedSkill(SkillSlot skill, int targetX, int targetY);

        // Exports
        [Export]
        private NodePath _buttonBasicAttackNode = "";
        [Export]
        private NodePath _buttonBasicDefenseNode = "";
        [Export]
        private NodePath _buttonCharacterSkillNode = "";
        [Export]
        private NodePath _buttonAbility1Node = "";
        [Export]
        private NodePath _buttonAbility2Node = "";
        [Export]
        private NodePath _buttonAbility3Node = "";
        [Export]
        private NodePath _buttonUltimateNode = "";
        [Export]
        private PackedScene _targetSelectorMenuObj = null;

        private Unit _targetUnit;
        private SkillSlot _selectedSkill;
        private (Button, SkillSlot)[] _allSkillButtons;
        private BattleTargetSelectorController _targetSelector;

        static private SkillSlot GetSkillFromString(string skillString)
        {
            switch (skillString)
            {
                case "BasicAttack":
                    return SkillSlot.BasicAttack;
                case "BasicDefense":
                    return SkillSlot.BasicDefense;
                case "CharacterSkill":
                    return SkillSlot.CharacterSkill;
                case "Ability1":
                    return SkillSlot.Ability1;
                case "Ability2":
                    return SkillSlot.Ability2;
                case "Ability3":
                    return SkillSlot.Ability3;
                case "Ultimate":
                    return SkillSlot.Ultimate;
                default:
                    throw new ArgumentException($"Invalid skill {skillString}");
            }
        }

        public override void _Ready()
        {
            base._Ready();
            Hide();
            //GetNode<PanelContainer>(_TargetSelectorPanelNode).Hide();

            _allSkillButtons = new[] {
                (GetNode<Button>(_buttonBasicAttackNode), SkillSlot.BasicAttack),
                (GetNode<Button>(_buttonBasicDefenseNode), SkillSlot.BasicDefense),
                (GetNode<Button>(_buttonCharacterSkillNode), SkillSlot.CharacterSkill),
                (GetNode<Button>(_buttonAbility1Node), SkillSlot.Ability1),
                (GetNode<Button>(_buttonAbility2Node), SkillSlot.Ability2),
                (GetNode<Button>(_buttonAbility3Node), SkillSlot.Ability3),
                (GetNode<Button>(_buttonUltimateNode), SkillSlot.Ultimate),
            };

            // Connect all the skill buttons to the appropiate function call
            foreach (var btn in _allSkillButtons)
            {
                btn.Item1.Connect("pressed", this, nameof(HandleSkillButtonPush), new Godot.Collections.Array { btn.Item2 });
            }
        }

        /// <summary>
        /// Enable skill selection buttons on UI.
        /// </summary>
        private void EnableSkillButtons()
        {
            foreach ((Button skillButton, SkillSlot slot) in _allSkillButtons)
            {
                // TODO: Figure out if a skill is usable
                skillButton.Disabled = !_targetUnit.IsSkillUsable(slot);
            }
        }

        /// <summary>
        /// Disable skill selection buttons on UI.
        /// </summary>
        private void DisableSkillButtons()
        {
            foreach (var skillButton in _allSkillButtons)
            {
                skillButton.Item1.Disabled = true;
            }
        }

        public void StartSkillMenu(Unit unit)
        {
            Show();

            _targetUnit = unit;

            EnableSkillButtons();
        }

        private void ExitSkillMenu()
        {
            DisableSkillButtons();
            Hide();
        }

        private void SpawnTargetSelector()
        {
            Global.Assert(_targetSelector == null || !IsInstanceValid(_targetSelector));

            var targetSelector = _targetSelectorMenuObj.Instance<BattleTargetSelectorController>();
            targetSelector.Connect(nameof(BattleTargetSelectorController.Canceled), this, nameof(HandleTargetSelectorCancelButton));
            targetSelector.Connect(nameof(BattleTargetSelectorController.PressedLocation), this, nameof(HandleTargetSelectorButton));
            GetParent().AddChild(targetSelector);
        }

        public void HandleSkillButtonPush(SkillSlot skill)
        {
            //SkillSlot skill = GetSkillFromString(skillString);

            GD.Print(skill);

            UnitSkill pickedSkill = _targetUnit.GetSkill(skill);
            Global.Assert(pickedSkill.Valid);
            _selectedSkill = skill;

            DisableSkillButtons();

            // TODO: Replace this with a better system for target selection
            if (pickedSkill.Activate.Target.HasFlag(SkillTarget.Self))
            {
                EmitSignal(nameof(BattleSkillMenuUsedSkill), _selectedSkill, 0, 0);
                ExitSkillMenu();
            }
            else
            {
                // Single target, display the target menu.
                SpawnTargetSelector();
            }
        }

        public void HandleTargetSelectorCancelButton()
        {
            EnableSkillButtons();
        }

        public void HandleTargetSelectorButton(int x, int y)
        {
            EmitSignal(nameof(BattleSkillMenuUsedSkill), _selectedSkill, x, y);
            ExitSkillMenu();
        }

        // TODO: Implement the rest of the options in this menu.
    }
}
