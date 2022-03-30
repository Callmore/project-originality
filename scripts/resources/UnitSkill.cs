using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectOriginality.Models;
using ProjectOriginality.Battle.Units;
using ProjectOriginality.Enums;

/*
namespace ProjectOriginality.Resources
{
    public class UnitSkillResource : Resource
    {
        // Sprites
        [Export]
        public Texture IdleSprite = null;

        [Export]
        public Texture DownSprite = null;

        public UnitSkillResource() { }
        public UnitSkillResource(Texture idleSprite = null, Texture downSprite = null)
        {
            IdleSprite = idleSprite;
            DownSprite = downSprite;
        }
    }
}
*/


namespace ProjectOriginality.Resources
{
    public enum SkillUsability
    {
        None,
        TargetAlive,
        TargetMissingHealth,
    }

    public class UnitSkill : Resource
    {
        [Export]
        public string Name { get; set; } = "";
        [Export]
        public string Description { get; set; } = "";
        [Export]
        public int ManaCost { get; set; } = 0;
        [Export]
        public double Windup { get; set; } = 0;
        [Export]
        public double RecoveryTime { get; set; } = 0;

        // Oh boy time to keep an up to date list of all skill usablitiy...
        [Export]
        public SkillUsability Usable { get; set; } = SkillUsability.None;

        [Export]
        public int Damage = 0;

        [Export]
        public int Heal = 0;

        // This is a fun hack.
        public SkillTarget Target { get => (SkillTarget)_target; set => _target = (int)value; }

        [Export(PropertyHint.Flags, "AllSelected,Friendly,Self,SpotTopFront,SpotTopBack,SpotMiddleFront,SpotMiddleBack,SpotBottomFront,SpotBottomBack")]
        private int _target = (int)SkillTarget.Board;

        [Export]
        public StatusStack[] Statuses = new StatusStack[0];


        public UnitSkill() { }

        /// <summary>
        /// Define a unit skill for a playable character.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="manaCost"></param>
        /// <param name="windup"></param>
        /// <param name="recoveryTime"></param>
        /// <param name="usable"></param>
        /// <param name="activate"></param>
        public UnitSkill(
            string name = "",
            string description = "",
            int manaCost = 0,
            double windup = 0,
            double recoveryTime = 0,
            SkillUsability usable = SkillUsability.None,
            int damage = 0,
            int heal = 0,
            SkillTarget target = SkillTarget.Board,
            StatusStack[] applyStatuses = null)
        {
            Name = name;
            Description = description;
            ManaCost = manaCost;
            Windup = windup;
            RecoveryTime = recoveryTime;
            Usable = usable;

            Damage = damage;
            Heal = heal;
            Target = target;
            Statuses = applyStatuses ?? new StatusStack[0];
        }
    }
}
