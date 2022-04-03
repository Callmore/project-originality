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

namespace ProjectOriginality.Resources
{
    public class PartyMemberResource : Resource
    {
        [Export]
        public int MaxHealth = 20;

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

        public PartyMemberResource() { }

        public PartyMemberResource(int maxHealth = 20, UnitSkill defaultAttackSkill = null, UnitSkill defaultDefendSkill = null, UnitSkill defaultCharacterSkill = null, List<UnitSkill> learnableSkills = null, UnitResource unit = null)
        {
            MaxHealth = maxHealth;
            DefaultAttackSkill = defaultAttackSkill;
            DefaultDefendSkill = defaultDefendSkill;
            DefaultCharacterSkill = defaultCharacterSkill;
            LearnableSkills = learnableSkills;
            UnitRes = unit;
        }
    }
}
