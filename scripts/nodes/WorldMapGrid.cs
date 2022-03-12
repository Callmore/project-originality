using Godot;
using System;
using System.Collections.Generic;
using ProjectOriginality.Party;
using ProjectOriginality.Battle.Units;

namespace ProjectOriginality.Nodes
{
    public class WorldMapGrid : Node
    {
        private PackedScene _tile = GD.Load<PackedScene>("res://objects/world_map_tile/world_map_tile.tscn");
        private PackedScene _unit = GD.Load<PackedScene>("res://objects/world_map_unit/world_map_unit.tscn");

        private const string TileGroup = "map_tile";

        public static Queue<UnitSkill> UnitUseActionQueue = new Queue<UnitSkill>();

        private void SetupBoard()
        {
            for (int x = 0; x < 7; x++)
            {
                for (int y = 0; y < 4; y++)
                {
                    if (x % 2 == 0 && y == 3)
                        continue;

                    var tile = _tile.Instance<Sprite>();
                    GetParent().CallDeferred("add_child", tile);
                    tile.Position = new Vector2(200 + (x * 96), 200 + (y * 128) - (64 * (x % 2)));
                    tile.AddToGroup(TileGroup);
                }
            }
        }

        private void SpawnUnit()
        {

        }

        private void SpawnCharacters()
        {

        }

        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
        {
            SetupBoard();
            SpawnCharacters();
        }

        // Called every frame. 'delta' is the elapsed time since the previous frame.
        public override void _Process(float delta)
        {

        }

        public void OnButtonPressed()
        {
            var allTiles = GetTree().GetNodesInGroup(TileGroup);
            Node tile = allTiles[(int)(GD.Randi() % allTiles.Count)] as Node;
            var unit = _unit.Instance();
            tile.CallDeferred("add_child", unit);
        }
    }
}
