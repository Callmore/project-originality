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

        private Unit _targetUnit;
        private SkillSlot _selectedSkill;
        private (Button, SkillSlot)[] _allSkillButtons;

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
            GetNode<PanelContainer>("SkillMenuPanel").Hide();
            GetNode<PanelContainer>("TargetSelectorPanel").Hide();

            _allSkillButtons = new[] {
            (GetNode<Button>("SkillMenuPanel/VBoxContainer/ButtonBasicAttack"), SkillSlot.BasicAttack),
            (GetNode<Button>("SkillMenuPanel/VBoxContainer/ButtonBasicDefense"), SkillSlot.BasicDefense),
            (GetNode<Button>("SkillMenuPanel/VBoxContainer/ButtonCharacterSkill"), SkillSlot.CharacterSkill),
            (GetNode<Button>("SkillMenuPanel/VBoxContainer/ButtonAbility1"), SkillSlot.Ability1),
            (GetNode<Button>("SkillMenuPanel/VBoxContainer/ButtonAbility2"), SkillSlot.Ability2),
            (GetNode<Button>("SkillMenuPanel/VBoxContainer/ButtonAbility3"), SkillSlot.Ability3),
            (GetNode<Button>("SkillMenuPanel/VBoxContainer/ButtonUltimate"), SkillSlot.Ultimate),
        };
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
            GetNode<PanelContainer>("SkillMenuPanel").Show();

            _targetUnit = unit;

            EnableSkillButtons();
        }

        private void ExitSkillMenu()
        {
            DisableSkillButtons();
            GetNode<PanelContainer>("SkillMenuPanel").Hide();
            GetNode<PanelContainer>("TargetSelectorPanel").Hide();
        }

        public void HandleSkillButtonPush(string skillString)
        {
            SkillSlot skill = GetSkillFromString(skillString);

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
                GetNode<PanelContainer>("TargetSelectorPanel").Show();
            }
        }

        public void HandleTargetSelectorCancelButton()
        {
            GetNode<PanelContainer>("TargetSelectorPanel").Hide();
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
