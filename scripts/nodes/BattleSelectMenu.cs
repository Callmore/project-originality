using Godot;
using System;

namespace ProjectOriginality.Nodes
{
    public class BattleSelectMenu : Control
    {
        public void OnButtonPush(int buttonID)
        {
            GD.Print($"Got {buttonID}");

            switch (buttonID)
            {
                case 0:
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
    }
}
