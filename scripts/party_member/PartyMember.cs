using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectOriginality.Battle.Units;

namespace ProjectOriginality.Party
{
    public abstract class PartyMember
    {
        public int Health { get; }
        public abstract int MaxHealth { get; }
        public int Experiance;
        public int Level;
        public List<UnitSkill> LearntSkills = new List<UnitSkill>(3);
        public abstract List<UnitSkill> LearnableSkills { get; }
        public abstract PackedScene UnitObject { get; }

        public void LearnSkill(int skillIndex)
        {
            Global.Assert(LearntSkills.Count < 3);

            UnitSkill skill = LearnableSkills[skillIndex];
            LearnableSkills.RemoveAt(skillIndex);
            LearntSkills.Add(skill);
        }

        public void UpdateStatusFromUnit(Unit unit)
        {

        }

        public Unit GetUnit()
        {
            Unit unit = UnitObject.Instance<Unit>();
            unit.UpdateStatsFromPartyMember(this);
            return unit;
        }
    }
}
