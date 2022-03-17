using Godot;
using System;
using ProjectOriginality;
using ProjectOriginality.Battle.Status;

namespace ProjectOriginality.Battle
{
    class StatusIcon : Control
    {
        [Export]
        public NodePath StatusImageNode = "";
        [Export]
        public NodePath StackLabelNode = "";

        private TextureRect _statusImage;
        private Label _stackLabel;

        public override void _Ready()
        {
            base._Ready();

            _statusImage = GetNode<TextureRect>(StatusImageNode);
            _stackLabel = GetNode<Label>(StackLabelNode);
        }

        public void Initialise(StatusEffect status)
        {
            _statusImage.Texture = status.Icon;
            _stackLabel.Text = $"{status.Stacks}";

            status.Connect(nameof(StatusEffect.StackCountModified), this, nameof(StatusIcon.OnStatusStacksUpdate));
            status.Connect(nameof(StatusEffect.Removed), this, nameof(StatusIcon.OnStatusRemoved));
        }

        public void OnStatusStacksUpdate(int newAmount)
        {
            _stackLabel.Text = $"{newAmount}";
        }

        public void OnStatusRemoved()
        {
            QueueFree();
        }
    }
}
