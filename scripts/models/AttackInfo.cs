using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectOriginality.Enums;

namespace ProjectOriginality.Models
{
    /// <summary>
    /// Describes an attack
    /// </summary>
    public struct AttackInfo
    {
        public readonly int Damage;
        public readonly int Heal;
        public readonly SkillTarget Target;
        public readonly (StatusId, int)[] Statuses;

        public AttackInfo(int damage = 0, int heal = 0, SkillTarget target = SkillTarget.Board, (StatusId, int)[] applyStatuses = null)
        {
            Damage = damage;
            Heal = heal;
            Target = target;
            Statuses = applyStatuses ?? new (StatusId, int)[0];
        }
    }
}
