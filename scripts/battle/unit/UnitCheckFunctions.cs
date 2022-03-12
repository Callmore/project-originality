using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectOriginality.Battle.Units
{
    public static class UnitCheckFunctions
    {
        public static bool AliveCheck(Unit unit) => unit.Health > 0;
    }
}
