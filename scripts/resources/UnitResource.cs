using Godot;
using System;

namespace ProjectOriginality.Resources
{
    public class UnitResource : Resource
    {
        [Export]
        public int MaxHealth = 20;
        // Sprites
        [Export]
        public SpriteFrames Frames = null;
        [Export]
        public Vector2 SpriteOffset = Vector2.Zero;

        [Export]
        public UnitSkill BasicAttack = null;

        [Export]
        public UnitSkill BasicDefence = null;

        [Export]
        public UnitSkill CharacterSkill = null;

        [Export]
        public CSharpScript AIAttackScript = null;

        public UnitResource() { }
        public UnitResource(int maxHealth = 20, SpriteFrames frames = null, Vector2 spriteOffset = new Vector2(), UnitSkill basicAttack = null, UnitSkill basicDefence = null, UnitSkill characterSkill = null, CSharpScript aIAttackScript = null)
        {
            MaxHealth = maxHealth;
            Frames = frames;
            SpriteOffset = spriteOffset;
            BasicDefence = basicDefence;
            CharacterSkill = characterSkill;
            AIAttackScript = aIAttackScript;
        }
    }
}
