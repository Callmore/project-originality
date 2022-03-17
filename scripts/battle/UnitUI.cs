using System;
using Godot;
using ProjectOriginality.Battle.Status;

namespace ProjectOriginality.Battle
{
    public class UnitUI : Control
    {
        [Export]
        private NodePath _healthBarNode = "";
        private TextureProgress _healthBar;

        [Export]
        private NodePath _healthBarFadeNode = "";
        private TextureProgress _healthBarFade;
        [Export]
        private NodePath _fadeTimerNode = "";
        private Timer _fadeTimer;
        [Export]
        private NodePath _fadeTweenNode = "";
        private Tween _fadeTween;

        [Export]
        private NodePath _healthLabelNode = "";
        private Label _healthLabel;
        [Export]
        private NodePath _healthMaxLabelNode = "";
        private Label _healthMaxLabel;

        [Export]
        private NodePath _statusEffectContainerNode = "";
        private HBoxContainer _statusEffectContainer;

        int _health = 0;
        int _maxHealth = 0;

        public override void _Ready()
        {
            base._Ready();
            _healthBar = GetNode<TextureProgress>(_healthBarNode);
            _healthBarFade = GetNode<TextureProgress>(_healthBarFadeNode);

            _fadeTimer = GetNode<Timer>(_fadeTimerNode);
            _fadeTween = GetNode<Tween>(_fadeTweenNode);

            _healthLabel = GetNode<Label>(_healthLabelNode);
            _healthMaxLabel = GetNode<Label>(_healthMaxLabelNode);

            _statusEffectContainer = GetNode<HBoxContainer>(_statusEffectContainerNode);
        }

        public void OnMaxHealthChanged(int oldMax, int newMax)
        {
            _maxHealth = newMax;

            _healthBar.MinValue = 0;
            _healthBar.MaxValue = _maxHealth;

            _healthBarFade.MinValue = 0;
            _healthBarFade.MaxValue = _maxHealth;

            UpdateLabelText();
        }

        public void OnHealthChanged(int oldHealth, int newHealth)
        {
            _health = newHealth;

            _healthBar.Value = _health;

            _fadeTimer.Start();

            UpdateLabelText();
        }

        public void InitHealthBar(int health, int maxHealth)
        {
            _health = health;
            _maxHealth = maxHealth;

            _healthBar.MinValue = 0;
            _healthBar.MaxValue = _maxHealth;
            _healthBar.Value = _health;

            _healthBarFade.MinValue = 0;
            _healthBarFade.MaxValue = _maxHealth;
            _healthBarFade.Value = _health;

            UpdateLabelText();
        }

        private void UpdateLabelText()
        {
            _healthLabel.Text = $"{_health}";
            if (_health == _maxHealth)
            {
                _healthMaxLabel.Hide();
            }
            else
            {
                _healthMaxLabel.Show();
                _healthMaxLabel.Text = $"{_maxHealth}";
            }
        }

        public void OnDrainTimerDone()
        {
            _fadeTween.InterpolateProperty(_healthBarFade, "value", _healthBarFade.Value, _health, 1f, Tween.TransitionType.Quint, Tween.EaseType.Out);
            _fadeTween.Start();
        }
        private static PackedScene _statusEffectUI = GD.Load<PackedScene>("res://objects/battle_unit_ui/status_effect_ui.tscn");

        public void OnStatusAdded(StatusEffect status)
        {
            // Create a new status icon
            StatusIcon statusUI = _statusEffectUI.Instance<StatusIcon>();
            _statusEffectContainer.AddChild(statusUI);

            statusUI.Initialise(status);
        }
    }

}
