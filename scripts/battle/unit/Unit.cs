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

namespace ProjectOriginality.Battle.Units
{
    enum UnitAnimation
    {
        Attack,
        Hurt,
        Die,
    }

    public abstract class Unit : Node2D
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

        public abstract int BaseMaxHealth { get; }

        // These will most likely be used later for mana.
        public int Mana { get; private set; }
        public int MaxMana { get; private set; }

        public int Level { get; private set; }

        public abstract UnitSkill BasicAttack { get; set; }
        public abstract UnitSkill BasicDefence { get; set; }
        public abstract UnitSkill CharacterSkill { get; set; }

        public UnitSkill[] Abilities { get; private set; } = new UnitSkill[3];
        public UnitSkill UltimateSkill { get; private set; }

        public double WindupTimeLeft { get; set; }
        public double RecoveryTimeLeft { get; set; }

        private bool _beenSetup = false;

        private Timer _attackWindupTimer;
        private Timer _attackRecoveryTimer;

        public UnitSkill QueuedSkill { get; private set; }
        public Point SkillBoardTarget { get; private set; }

        private static Mutex _actionMutex = new Mutex();

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

        // Animation variables
        private AnimationPlayer _animationPlayer;
        private Queue<UnitAnimation> _animationQueue = new Queue<UnitAnimation>();
        private const int AnimationQueueMax = 3;
        private bool _playingAnimation = false;
        private Sprite _spriteNode;

        // AI things
        private IEnemyAttack _AIAttackScript;

        public BattleController Controller { private get; set; }
        public Action<Unit> OnWindupTimerFinish { private get; set; }
        public Action<Unit> OnRecoveryTimerFinish { private get; set; }
        public Action<Unit> OnDieFunc { private get; set; }

        private bool _setupFromParty = false;

        public override void _Ready()
        {
            base._Ready();

            _attackWindupTimer = GetNode<Timer>("AttackWindupTimer");
            _attackRecoveryTimer = GetNode<Timer>("AttackRecoverTimer");
            _animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
            _spriteNode = GetNode<Sprite>("Sprite");
            StatusEffectController = GetNode<StatusEffectController>("StatusEffectController");

            if (!_setupFromParty)
            {
                _health = BaseMaxHealth;
                _maxHealth = BaseMaxHealth;
            }

            Texture texture = GD.Load<Texture>("res://res/unit.png");
            _spriteNode.Texture = texture;
            _spriteNode.Offset = new Vector2(0, -texture.GetHeight() / 2);

            QueuedSkill = BasicAttack;
            SkillBoardTarget = new Point(1, 1);

            //_attackWindupTimer.Start(GD.Randf());
            _attackRecoveryTimer.Start(GD.Randf());

            _AIAttackScript = GetAIAttackScript();
        }

        public override void _Process(float delta)
        {
            base._Process(delta);
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

            _setupFromParty = true;
        }

        protected abstract IEnemyAttack GetAIAttackScript();

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

        public void UseSkill(SkillSlot skill, Point target)
        {
            UseSkill(GetSkill(skill), target);
        }
        public void UseSkill(UnitSkill skill, Point target)
        {
            Global.Assert(skill.Valid);

            GD.Print($"SKILL - NAME: {skill.Name}");

            QueuedSkill = skill;
            SkillBoardTarget = target;

            _attackWindupTimer.Start(Math.Max((float)QueuedSkill.Windup, 0.01f));

            Controller.PauseBattleTimers(false);
            _actionMutex.Unlock();
        }

        public void Hurt(int amount)
        {
            Hurt(new AttackInfo(amount));
        }

        public void Hurt(AttackInfo attack)
        {
            // Pass the attack info object though the supply chain

            int amount = attack.Damage;
            if (StatusEffectController.CheckStatusEffect(StatusId.Block))
            {
                amount = (int)Math.Ceiling((double)amount / 2);
            }
            if (StatusEffectController.CheckStatusEffect(StatusId.Weak))
            {
                amount *= 2;
            }

            if (amount > 0)
            {
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

        public void ApplyStatus(StatusId id, int stacks = -1)
        {
            StatusEffectController.ApplyStatus(id, stacks);
        }
        private void Die()
        {
            OnDieFunc(this);
            PlayAnimation(UnitAnimation.Die);
            _attackWindupTimer.Paused = true;
            _attackRecoveryTimer.Paused = true;
        }

        public bool SkillHasValidTarget(SkillSlot slot)
        {
            return SkillHasValidTarget(GetSkill(slot));
        }
        public bool SkillHasValidTarget(UnitSkill skill)
        {
            (BoardSide side, Point position) = Controller.FindUnitLocation(this);

            if (skill.Activate.Target == SkillTarget.Self)
            {
                return skill.Usable(this);
            }
            else
            {
                // Find target enemy
                for (int x = 0; x < 2; x++)
                {
                    for (int y = 0; y < 3; y++)
                    {
                        Unit target = Controller.GetUnitAt(side, new Point(x, y));
                        if (target != null && skill.Usable(target))
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
            return skill.Valid && SkillHasValidTarget(skill);
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
            GetNode<Sprite>("Sprite").FlipH = newFlip;
        }

        public async void AttackWindupTimerFinished()
        {
            _actionMutex.Lock();
            if (Controller == null || Controller.BattleOver)
            {
                _actionMutex.Unlock();
                return;
            }

            Controller.PauseBattleTimers(true);

            PlayAnimation(UnitAnimation.Attack);

            // IDK do an attack
            OnWindupTimerFinish(this);
            await ToSignal(Controller, nameof(BattleController.ControllerWindupFinished));

            _attackRecoveryTimer.Start(Math.Max((float)QueuedSkill.RecoveryTime, 0.01f));

            Controller.PauseBattleTimers(false);
            _actionMutex.Unlock();
        }

        public void AttackRecoverTimerFinished()
        {
            _actionMutex.Lock();

            if (Controller == null || Controller.BattleOver)
            {
                _actionMutex.Unlock();
                return;
            }

            Controller.PauseBattleTimers();

            GD.Print(_enemy);
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
                    Point skillTarget = Controller.GetRandomEnemyTargetPosition();
                    UseSkill(_AIAttackScript.Next(), skillTarget);
                    _AIAttackScript.Advance();
                }
                else
                {
                    (_, Point location) = Controller.FindUnitLocation(this);
                    UseSkill(SkillNoOp, location);
                }
            }
            else
            {
                throw new InvalidOperationException("Unit is not marked an enemy and has no attack script set.");
            }
        }

        private readonly UnitSkill SkillNoOp = new UnitSkill("No-Op", 5, 5, new AttackInfo(target: SkillTarget.Self));
    }
}
