using Godot;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectOriginality.Battle.Status;
using ProjectOriginality.Models;
using ProjectOriginality.Enums;
using ProjectOriginality.Battle.Units.EnemyAttackScripts;
using ProjectOriginality.Party;
using ProjectOriginality.Resources;

namespace ProjectOriginality.Battle.Units
{
    enum UnitAnimation
    {
        Attack,
        Hurt,
        Die,
        Idle,
    }

    public class Unit : Node2D
    {
        [Signal]
        public delegate void MaxHealthModified(int oldValue, int newValue);

        [Signal]
        public delegate void HealthModified(int oldValue, int newValue);

        public string UnitName { get; private set; }
        public int Health
        {
            get => _health;
            private set
            {
                EmitSignal(nameof(HealthModified), _health, value);
                _health = value;
            }
        }
        public int MaxHealth
        {
            get => _maxHealth;
            private set
            {
                EmitSignal(nameof(MaxHealthModified), _maxHealth, value);
                _maxHealth = value;
            }
        }
        private int _maxHealth;

        private int _health;


        // These will most likely be used later for mana.
        public int Mana { get; private set; }
        public int MaxMana { get; private set; }

        public int Level { get; private set; }

        public UnitSkill BasicAttack { get; set; }
        public UnitSkill BasicDefence { get; set; }
        public UnitSkill CharacterSkill { get; set; }

        public UnitSkill[] Abilities { get; private set; } = new UnitSkill[3];
        public UnitSkill UltimateSkill { get; private set; }

        public double WindupTimeLeft { get; set; }
        public double RecoveryTimeLeft { get; set; }

        private bool _beenSetup = false;

        public double AttackWindupTimer { get; private set; }
        public double AttackRecoveryTimer { get; private set; }

        public UnitSkill QueuedSkill { get; private set; }
        public BattleLoc SkillBoardTarget { get; private set; }

        private Vector2 _spriteOffset = Vector2.Zero;

        public bool Enemy
        {
            get => _enemy;
            set
            {
                UpdateFlip(value);
                _enemy = value;
            }
        }

        private static readonly PackedScene _objDamageNumber = GD.Load<PackedScene>("res://objects/damage_number/damage_number.tscn");

        private bool _enemy = false;

        public StatusEffectController StatusEffectController { get; private set; }

        public bool Dead = false;

        // Animation variables
        private AnimationPlayer _animationPlayer;
        private Queue<UnitAnimation> _animationQueue = new Queue<UnitAnimation>();
        private const int AnimationQueueMax = 3;
        private bool _playingAnimation = false;
        private AnimatedSprite _spriteNode;

        // AI things
        private EnemyAttackScript _AIAttackScript;

        public BattleController Controller { private get; set; }
        public Action<Unit> OnWindupTimerFinish { private get; set; }
        public Action<Unit> OnRecoveryTimerFinish { private get; set; }
        public Action<Unit> OnDieFunc { private get; set; }

        private bool _setupFromParty = false;
        public PartyMember PartyMemberRef { get; private set; } = null;

        public override void _Ready()
        {
            base._Ready();

            _animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
            _spriteNode = GetNode<AnimatedSprite>("AnimatedSprite");
            StatusEffectController = GetNode<StatusEffectController>("StatusEffectController");

            _spriteNode.Offset = _spriteOffset;

            QueuedSkill = BasicAttack;
            SkillBoardTarget = new BattleLoc(1, 1);

            //_attackWindupTimer.Start(GD.Randf());
            AttackRecoveryTimer = GD.Randf();

            //_AIAttackScript = GetAIAttackScript();

            PlayAnimation(UnitAnimation.Idle);
        }

        public override void _Process(float delta)
        {
            base._Process(delta);

            if (!Dead && !Controller.IsTimerPaused())
            {
                if (AttackWindupTimer > 0)
                {
                    AttackWindupTimer -= delta;
                    if (AttackWindupTimer <= 0)
                    {
                        Controller.WindupTimerFinished.Add(this);
                    }
                }
                else if (AttackRecoveryTimer > 0)
                {
                    AttackRecoveryTimer -= delta;
                    if (AttackRecoveryTimer <= 0)
                    {
                        Controller.RecoveryTimerFinished.Add(this);
                    }
                }

                StatusEffectController.Tick(delta);
            }
        }

        public void UpdateStatsFromPartyMember(PartyMember member)
        {
            _maxHealth = member.MaxHealth;
            _health = member.Health;
            for (int i = 0; i < member.LearntSkills.Count; i++)
            {
                Abilities[i] = member.LearntSkills[i];
            }
            Level = member.Level;

            PartyMemberRef = member;
            _setupFromParty = true;

            EmitSignal(nameof(HealthModified), _health, _health);
            EmitSignal(nameof(MaxHealthModified), _maxHealth, _maxHealth);
        }

        public void UpdatePartyMember()
        {
            PartyMemberRef.UpdateStatusFromUnit(this);
        }

        //protected abstract IEnemyAttack GetAIAttackScript();

        public UnitSkill GetSkill(SkillSlot skill)
        {
            switch (skill)
            {
                case SkillSlot.BasicAttack:
                    return BasicAttack;
                case SkillSlot.BasicDefense:
                    return BasicDefence;
                case SkillSlot.CharacterSkill:
                    return CharacterSkill;
                case SkillSlot.Ability1:
                    return Abilities[0];
                case SkillSlot.Ability2:
                    return Abilities[1];
                case SkillSlot.Ability3:
                    return Abilities[2];
                case SkillSlot.Ultimate:
                    return UltimateSkill;
                default:
                    throw new InvalidOperationException($"Invalid {nameof(SkillSlot)} {skill}");
            }

        }

        public void UseSkill(SkillSlot skill, BattleLoc target)
        {
            UseSkill(GetSkill(skill), target);
        }
        public void UseSkill(UnitSkill skill, BattleLoc target)
        {
            GD.Print($"SKILL - NAME: {skill.Name}");

            QueuedSkill = skill;
            SkillBoardTarget = target;

            AttackWindupTimer = Math.Max(QueuedSkill.Windup, 0.01);

            Controller.ResumeBattle();
        }

        public void Hurt(int amount)
        {
            Hurt(new UnitSkill(damage: amount));
        }

        public void Hurt(UnitSkill attack)
        {

            int amount = attack.Damage;


            if (amount > 0)
            {
                // Pass the attack info object though the supply chainain

                amount = GetDefenseModifierCalc().Calculate(amount);
                Health -= amount;

                // Spawn the damage number
                Node2D dmgNumber = _objDamageNumber.Instance<Node2D>();
                Label dmgLbl = dmgNumber.GetNode<Label>("Label");
                AnimationPlayer animationPlayer = dmgNumber.GetNode<AnimationPlayer>("AnimationPlayer");
                AddChild(dmgNumber);
                dmgNumber.Position = Position + new Vector2(0, -64);
                dmgLbl.Text = $"-{amount}";
                animationPlayer.Play("Raise");

                if (Health <= 0)
                {
                    Die();
                }
                else
                {
                    PlayAnimation(UnitAnimation.Hurt);
                }
            }
        }

        public void Heal(int amount)
        {
            amount = Math.Min(amount, MaxHealth - Health);
            if (amount > 0)
            {
                Health += amount;

                // Spawn the damage number
                Node2D dmgNumber = _objDamageNumber.Instance<Node2D>();
                Label dmgLbl = dmgNumber.GetNode<Label>("Label");
                AnimationPlayer animationPlayer = dmgNumber.GetNode<AnimationPlayer>("AnimationPlayer");
                AddChild(dmgNumber);
                dmgNumber.Position = Position + new Vector2(0, -64);
                dmgLbl.Text = $"+{amount}";
                animationPlayer.Play("Raise");
            }
        }

        public void ApplyStatus(StatusId id, int stacks = -1)
        {
            StatusEffectController.ApplyStatus(id, stacks);
        }

        private void Die()
        {
            OnDieFunc(this);
            PlayAnimation(UnitAnimation.Die);
            Dead = true;
        }

        public bool SkillHasValidTarget(SkillSlot slot)
        {
            return SkillHasValidTarget(GetSkill(slot));
        }

        public bool SkillHasValidTarget(UnitSkill skill)
        {
            if (skill == null)
            {
                return false;
            }

            (BoardSide side, BattleLoc position) = Controller.FindUnitLocation(this);

            if (skill.Target == SkillTarget.Self)
            {
                return true; // TODO: Replace true with a function to check if skill usablity is true.
            }
            else
            {
                // Find target enemy
                for (int x = 0; x < 2; x++)
                {
                    for (int y = 0; y < 3; y++)
                    {
                        Unit target = Controller.GetUnitAt(side, new BattleLoc(x, y));
                        if (target != null && true) // TODO: Replace true with a function to check if skill usablity is true.
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Return if <paramref name="skillSlot"/> is usable (Slot has a valid skill, user has enough mana, at least 1 target avalible).
        /// </summary>
        /// <param name="skillSlot">Skill slot to check.</param>
        /// <returns>If the specified skill can be used.</returns>
        public bool IsSkillUsable(SkillSlot skillSlot)
        {
            UnitSkill skill = GetSkill(skillSlot);
            return SkillHasValidTarget(skill);
        }

        public bool IsSkillUsable(UnitSkill skill)
        {
            return SkillHasValidTarget(skill);
        }

        #region Animation

        private void PlayAnimation(UnitAnimation animation)
        {
            // Run a function based on the animation being played
            string animationName;
            switch (animation)
            {
                case UnitAnimation.Attack:
                    animationName = "Attack";
                    break;
                case UnitAnimation.Hurt:
                    animationName = "Hurt";
                    break;
                case UnitAnimation.Die:
                    animationName = "Die";
                    break;
                case UnitAnimation.Idle:
                    animationName = "Idle";
                    break;
                default:
                    throw new Exception($"No animation found for {nameof(UnitAnimation)} {animation}");
            }

            if (Enemy)
            {
                animationName += "Enemy";
            }

            _animationPlayer.Play(animationName);
        }

        #endregion

        private void UpdateFlip(bool newFlip)
        {
            GetNode<AnimatedSprite>("AnimatedSprite").FlipH = newFlip;
        }

        private IEnumerable<StatusEffect> GetAllStatusNodes()
        {
            return StatusEffectController.GetAllStatusesAndStacks().Select(pair => pair.Item1).Select(id => StatusEffectController.GetStatusEffectOrNull(id));
        }

        public BuffCalculator GetAttackModifierCalc(int min = 0, int max = int.MaxValue)
        {
            var allStatuses = GetAllStatusNodes();
            BuffCalculator calc = new BuffCalculator(min, max);
            calc.AddRange(allStatuses.Select(status => status.AttackModifier));
            return calc;
        }

        public BuffCalculator GetDefenseModifierCalc(int min = 0, int max = int.MaxValue)
        {
            var allStatuses = GetAllStatusNodes();
            BuffCalculator calc = new BuffCalculator(min, max);
            calc.AddRange(allStatuses.Select(status => status.DefenseModifier));
            return calc;
        }

        public async Task AttackWindupTimerFinished()
        {
            Controller.PauseBattleTimers(true);

            PlayAnimation(UnitAnimation.Attack);

            // IDK do an attack
            OnWindupTimerFinish(this);
            await ToSignal(Controller, nameof(BattleController.ControllerWindupFinished));

            AttackRecoveryTimer = Math.Max(QueuedSkill.RecoveryTime, 0.01);

            Controller.PauseBattleTimers(false);
        }

        public void AttackRecoverTimerFinished()
        {
            Controller.PauseBattleTimers();

            if (!_enemy)
            {
                OnRecoveryTimerFinish(this);
                return;
            }
            else if (_AIAttackScript != null)
            {
                UnitSkill skill = _AIAttackScript.Next();
                if (SkillHasValidTarget(skill))
                {
                    BattleLoc skillTarget = Controller.GetRandomEnemyTargetPosition();
                    UseSkill(_AIAttackScript.Next(), skillTarget);
                    _AIAttackScript.Advance();
                }
                else
                {
                    (_, BattleLoc location) = Controller.FindUnitLocation(this);
                    UseSkill(SkillNoOp, location);
                }
            }
            else
            {
                throw new InvalidOperationException("Unit is marked as an enemy and has no attack script set.");
            }
        }

        private readonly UnitSkill SkillNoOp = new UnitSkill(name: "No-Op", windup: 5, recoveryTime: 5, target: SkillTarget.Self);

        public static Unit FromResource(UnitResource resource)
        {
            // Get unit scene
            Unit unit = GD.Load<PackedScene>("res://objects/battle_unit/base_unit.tscn").Instance<Unit>();
            unit._health = resource.MaxHealth;
            unit._maxHealth = resource.MaxHealth;
            unit.BasicAttack = resource.BasicAttack;
            unit.BasicDefence = resource.BasicDefence;
            unit.CharacterSkill = resource.CharacterSkill;
            unit._AIAttackScript = (EnemyAttackScript)resource.AIAttackScript?.New() ?? null;
            unit.GetNode<AnimatedSprite>("AnimatedSprite").Frames = resource.Frames;
            unit._spriteOffset = resource.SpriteOffset;

            return unit;
        }

        public static Unit FromPartyMember(PartyMember member)
        {
            Unit unit = FromResource(member.UnitRes);
            unit._health = member.Health;
            unit._maxHealth = member.MaxHealth;
            unit.Level = member.Level;
            unit.BasicAttack = member.DefaultAttackSkill;
            unit.BasicDefence = member.DefaultDefendSkill;
            unit.CharacterSkill = member.DefaultCharacterSkill;
            unit.Abilities = member.LearntSkills.ToArray();
            return unit;
        }
    }
}
