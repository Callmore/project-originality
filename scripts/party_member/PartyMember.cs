using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectOriginality.Battle.Units;
using ProjectOriginality;
using ProjectOriginality.Models;

namespace ProjectOriginality.Party
{
    public abstract class PartyMember
    {
        public int Health { get; private set; }
        public abstract int MaxHealth { get; protected set; }
        public int Experiance { get; protected set; } = 0;
        public int Level { get; protected set; } = 1;
        public List<UnitSkill> LearntSkills = new List<UnitSkill>(3);
        public abstract List<UnitSkill> LearnableSkills { get; }
        public abstract PackedScene UnitObject { get; }
        public Point BattleLocation { get; set; } = new Point(1, 1);

        public PartyMember()
        {
            Health = MaxHealth;
        }

        public PartyMember(Point initalBattleLocation)
        {
            Health = MaxHealth;
            BattleLocation = initalBattleLocation;
        }

        public void LearnSkill(int skillIndex)
        {
            Global.Assert(LearntSkills.Count < 3);

            UnitSkill skill = LearnableSkills[skillIndex];
            LearnableSkills.RemoveAt(skillIndex);
            LearntSkills.Add(skill);
        }

        public virtual void UpdateStatusFromUnit(Unit unit)
        {
            if (unit.Dead)
            {
                // In the future this might mark the unit is completely dead.
                Health = 0;
            }
            else
            {
                Health = unit.Health;
            }
        }

        public Unit GetUnit()
        {
            Unit unit = UnitObject.Instance<Unit>();
            unit.UpdateStatsFromPartyMember(this);
            return unit;
        }
    }
}
