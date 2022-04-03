using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectOriginality.Battle.Units;
using ProjectOriginality;
using ProjectOriginality.Models;
using ProjectOriginality.Resources;
using ProjectOriginality.Battle;

namespace ProjectOriginality.Resources
{
    public class EncounterDefinition : Resource
    {
        [Export]
        public UnitResource UnitTopFront = null;
        [Export]
        public UnitResource UnitTopBack = null;
        [Export]
        public UnitResource UnitMiddleFront = null;
        [Export]
        public UnitResource UnitMiddleBack = null;
        [Export]
        public UnitResource UnitBottomFront = null;
        [Export]
        public UnitResource UnitBottomBack = null;

        public EncounterDefinition() { }

        public EncounterDefinition(
            UnitResource unitTopFront = null,
            UnitResource unitTopBack = null,
            UnitResource unitMiddleFront = null,
            UnitResource unitMiddleBack = null,
            UnitResource unitBottomFront = null,
            UnitResource unitBottomBack = null)
        {
            UnitTopFront = unitTopFront;
            UnitTopBack = unitTopBack;
            UnitMiddleFront = unitMiddleFront;
            UnitMiddleBack = unitMiddleBack;
            UnitBottomFront = unitBottomFront;
            UnitBottomBack = unitBottomBack;
        }

        public UnitResource[,] GetEncounterUnits()
        {
            return new UnitResource[BattleController.LineCount, BattleController.LaneCount] {
                {UnitTopBack, UnitMiddleBack, UnitBottomBack},
                {UnitTopFront, UnitMiddleFront, UnitBottomFront},
            };
        }
    }
}
