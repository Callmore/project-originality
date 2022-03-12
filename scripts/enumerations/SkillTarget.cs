using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectOriginality.Enums
{
    [Flags]
    public enum SkillTarget
    {
        /// <summary>
        /// Targets all selected spots.
        /// </summary>
        AllSelected = 1 << 0,

        /// <summary>
        /// Targets units on the player's side of the board.
        /// </summary>
        Friendly = 1 << 1,

        /// <summary>
        /// Targets the user.
        /// </summary>
        Self = 1 << 2,

        // Spots

        SpotTopFront = 1 << 3,
        SpotTopBack = 1 << 4,
        SpotMiddleFront = 1 << 5,
        SpotMiddleBack = 1 << 6,
        SpotBottomFront = 1 << 7,
        SpotBottomBack = 1 << 8,

        // Lanes and lines

        LaneTop = SpotTopFront | SpotTopBack,
        LaneMiddle = SpotMiddleFront | SpotMiddleBack,
        LaneBottom = SpotBottomFront | SpotBottomBack,

        LineFront = SpotTopFront | SpotMiddleFront | SpotBottomFront,
        LineBack = SpotTopBack | SpotMiddleBack | SpotBottomBack,

        Board = SpotTopFront | SpotTopBack | SpotMiddleFront | SpotMiddleBack | SpotBottomFront | SpotBottomBack

    }
}
