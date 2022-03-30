using System;
using Godot;
using ProjectOriginality;
using ProjectOriginality.Party;

namespace ProjectOriginality.Nodes
{
    public class PartyDisplayController : Control
    {
        [Signal]
        public delegate void MemberPressed();

        [Export]
        private NodePath _healthBarNode = "";
        private TextureProgress _healthBar = null;

        [Export]
        private NodePath _healthBarLabelNode = "";
        private Label _healthBarLabel = null;

        [Export]
        private NodePath _memberIconNode = "";
        private TextureRect _memberIcon = null;

        [Export]
        private NodePath _buttonNode = "";
        private Button _button = null;

        [Export]
        private NodePath _disabledIndicatorNode = "";
        private Control _disabledIndicator = null;

        public override void _Ready()
        {
            base._Ready();

            _healthBar = GetNode<TextureProgress>(_healthBarNode);
            _healthBarLabel = GetNode<Label>(_healthBarLabelNode);
            _memberIcon = GetNode<TextureRect>(_memberIconNode);
            _button = GetNode<Button>(_buttonNode);
            _disabledIndicator = GetNode<Control>(_disabledIndicatorNode);

            ClearDisplay();
        }

        /// <summary>
        /// Update display with properties from specified <paramref name="member"/>.
        /// </summary>
        /// <param name="member">Member to update display from.</param>
        public void UpdateDisplay(PartyMember member)
        {
            _healthBar.Show();
            _healthBarLabel.Show();
            _memberIcon.Show();
            _disabledIndicator.Hide();

            _healthBar.Value = member.Health;
            _healthBar.MaxValue = member.MaxHealth;

            if (member.Health > 0)
            {
                _healthBarLabel.Text = $"{member.Health}/{member.MaxHealth}";
                _memberIcon.RectRotation = 90;
            }
            else
            {
                _healthBarLabel.Text = "DOWN";
                _memberIcon.RectRotation = 0;
            }

            _memberIcon.Texture = member.UnitRes.Frames.GetFrame("idle", 0);
        }

        /// <summary>
        /// Clears the display.
        /// </summary>
        public void ClearDisplay()
        {
            _healthBar.Hide();
            _healthBarLabel.Hide();
            _memberIcon.Hide();
            _disabledIndicator.Show();
        }

        public void OnButtonPressed()
        {
            EmitSignal(nameof(MemberPressed));
        }
    }
}
