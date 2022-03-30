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

namespace ProjectOriginality.Party
{
    public class PartyMember : Resource
    {
        public int Health;

        [Export]
        public int MaxHealth = 20;

        public int Experiance = 0;

        public int Level = 1;

        public List<UnitSkill> LearntSkills = new List<UnitSkill>(3);

        [Export]
        public UnitSkill DefaultAttackSkill = null;

        [Export]
        public UnitSkill DefaultDefendSkill = null;

        [Export]
        public UnitSkill DefaultCharacterSkill = null;

        [Export]
        public List<UnitSkill> LearnableSkills = new List<UnitSkill>();

        [Export]
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

        public PartyMember(int maxHealth = 20, UnitSkill defaultAttackSkill = null, UnitSkill defaultDefendSkill = null, UnitSkill defaultCharacterSkill = null, List<UnitSkill> learnableSkills = null, UnitResource unit = null)
        {
            Health = maxHealth;
            MaxHealth = maxHealth;
            DefaultAttackSkill = defaultAttackSkill;
            DefaultDefendSkill = defaultDefendSkill;
            DefaultCharacterSkill = defaultCharacterSkill;
            LearnableSkills = learnableSkills;
            UnitRes = unit;
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
    }
}
