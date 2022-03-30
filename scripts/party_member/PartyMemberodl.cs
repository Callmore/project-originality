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
using System.Collections.ObjectModel;

namespace ProjectOriginality.Party
{
    public class PartyMember
    {
        public int Health;

        public int MaxHealth = 20;

        public int Experiance = 0;

        public int Level = 1;

        public List<UnitSkill> LearntSkills = new List<UnitSkill>(3);

        public UnitSkill DefaultAttackSkill = null;

        public UnitSkill DefaultDefendSkill = null;

        public UnitSkill DefaultCharacterSkill = null;

        public List<UnitSkill> LearnableSkills = new List<UnitSkill>();

        public UnitResource UnitRes = null;

        public BattleLoc BattleLocation = new BattleLoc(1, 1);

        public PartyMember()
        {
            Health = MaxHealth;
        }

        public PartyMember(BattleLoc initalBattleLocation)
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
            Unit unit = Unit.FromPartyMember(this);
            unit.UpdateStatsFromPartyMember(this);
            return unit;
        }

        static public PartyMember FromResource(PartyMemberResource resource)
        {
            PartyMember member = new PartyMember();
            member.Health = resource.MaxHealth;
            member.MaxHealth = resource.MaxHealth;
            member.DefaultAttackSkill = resource.DefaultAttackSkill;
            member.DefaultDefendSkill = resource.DefaultDefendSkill;
            member.DefaultCharacterSkill = resource.DefaultCharacterSkill;
            member.LearnableSkills = new List<UnitSkill>(resource.LearnableSkills);
            member.UnitRes = resource.UnitRes;

            return member;
        }
    }
}
