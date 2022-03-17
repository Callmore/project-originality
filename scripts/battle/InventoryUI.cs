using System;
using System.Linq;
using Godot;
using ProjectOriginality;
using System.Collections.Generic;
using ProjectOriginality.Inventory;

namespace ProjectOriginality.Battle
{
    public class InventoryUI : Control
    {
        [Signal]
        public delegate void ItemSlotClicked(int slot);

        [Signal]
        public delegate void InventoryToggled(bool opened);

        [Export]
        private PackedScene _buttonObj = null;
        private List<Button> _inventoryButtons = new List<Button>();

        [Export]
        private NodePath _buttonPanelNode = "";
        private Control _buttonPanel = null;

        [Export]
        private NodePath _buttonContainerNode = "";

        [Export]
        private PackedScene _targetSelectorObj = null;

        [Export]
        private NodePath _battleControllerNode = "";
        private BattleController _controller;

        public bool InventoryOpen = false;

        private int _itemSelected = 0;

        public override void _Ready()
        {
            base._Ready();

            for (int i = 0; i < PlayerStatus.MaxInventorySize; i++)
            {
                Button button = _buttonObj.Instance<Button>();

                // Do something with the button
                button.Connect("pressed", this, nameof(OnUseItem), new Godot.Collections.Array { i });

                _inventoryButtons.Add(button);
                GetNode(_buttonContainerNode).AddChild(button);
            }

            _controller = GetNode<BattleController>(_battleControllerNode);
            _buttonPanel = GetNode<PanelContainer>(_buttonPanelNode);

            UpdateInventory();
        }

        public void UpdateInventory()
        {
            for (int i = 0; i < PlayerStatus.MaxInventorySize; i++)
            {
                Button button = _inventoryButtons[i];
                if (PlayerStatus.Inventory.Count <= i)
                {
                    button.Disabled = true;
                    button.Icon = null;
                }
                else
                {
                    button.Disabled = false;
                    InventoryItem item = PlayerStatus.Inventory[i];
                    // Update button with information.
                    button.Icon = GD.Load<Texture>(item.IconResourcePath);
                }
            }
        }

        public void OnUseItem(int slot)
        {
            GD.Print(slot);

            _itemSelected = slot;
            var targetSelector = _targetSelectorObj.Instance<BattleTargetSelectorController>();
            targetSelector.Connect(nameof(BattleTargetSelectorController.PressedLocation), this, nameof(HandleTargetSelectorLocation), flags: (uint)ConnectFlags.Oneshot);
            GetParent().AddChild(targetSelector);
        }

        public void HandleTargetSelectorLocation(int x, int y)
        {
            InventoryItem item = PlayerStatus.Inventory[_itemSelected];
            _controller.UseItem(item, x, y);

            PlayerStatus.Inventory.RemoveAt(_itemSelected);
            PlayerStatus.SortInventory();

            UpdateInventory();
        }

        public void CloseInventory()
        {
            _buttonPanel.Hide();
            InventoryOpen = false;
            EmitSignal(nameof(InventoryToggled), false);
        }

        public void OpenInventory()
        {
            _buttonPanel.Show();
            InventoryOpen = true;
            EmitSignal(nameof(InventoryToggled), true);
        }

        public void OnInventoryButtonPressed()
        {
            if (InventoryOpen)
            {
                CloseInventory();
            }
            else
            {
                OpenInventory();
            }
        }
    }
}
