using Common.Battle;
using Common.Data;
using GameServer.Battle;
using GameServer.Core;
using GameServer.Managers;
using SkillBridge.Message;
using System;
using System.Collections.Generic;

namespace GameServer.Entities
{
    class Creature : Entity
    {
        /// <summary>
        /// 数据库Id
        /// </summary>
        public int Id;

        public string Name { get { return Info.Name; } }

        /// <summary>
        /// 网络角色信息
        /// </summary>
        public NCharacterInfo Info;

        /// <summary>
        /// 角色配置信息（从Excel表读取）
        /// </summary>
        public CharacterDefine Define;

        public Attributes Attributes;

        public SkillManager SkillMgr;

        public BuffManager BuffMgr;

        public EffectManager EffectMgr;

        public bool IsDeath = false;

        public BattleState BattleState;

        public CharacterState State;

        public Creature(CharacterType type, int configId, int level, Vector3Int pos, Vector3Int dir) :
           base(pos, dir)
        {
            this.Define = DataManager.Instance.Characters[configId];

            Info = new NCharacterInfo();
            Info.Type = type;
            Info.Level = level;
            Info.ConfigId = configId;
            Info.Entity = this.EntityData;
            Info.EntityId = entityId;
            Info.Name = this.Define.Name;
            InitSkills();
            InitBuffs();
            InitEffects();

            Attributes = new Attributes();
            Attributes.Init(Define, Info.Level, GetEquips(), Info.attrDynamic);
            Info.attrDynamic = Attributes.DynamicAttr;
        }

        private void InitEffects()
        {
            EffectMgr = new EffectManager(this);
        }

        private void InitBuffs()
        {
            BuffMgr = new BuffManager(this);
        }

        public virtual List<EquipDefine> GetEquips()
        {
            return null;
        }

        private void InitSkills()
        {
            SkillMgr = new SkillManager(this);
            Info.Skills.AddRange(SkillMgr.Infos);
        }

        internal void CastSkill(BattleContext context, int skillId)
        {
            Skill skill = SkillMgr.GetSkill(skillId);
            context.Result = skill.Cast(context);
            if(context.Result == SkillResult.Ok)
            {
                BattleState = BattleState.InBattle;
            }

            if(context.CastSkill == null)
            {
                if(context.Result == SkillResult.Ok)
                {
                    context.CastSkill = new NSkillCastInfo()
                    {
                        casterId = entityId,
                        targetId = context.Target.entityId,
                        skillId = skill.Define.ID,
                        Position = new NVector3(),
                        Result = context.Result
                    };
                    context.Battle.AddCastSkillInfo(context.CastSkill);
                }
            }
            else
            {
                context.CastSkill.Result = context.Result;
                context.Battle.AddCastSkillInfo(context.CastSkill);
            }
        }

        internal void DoDamage(NDamageInfo damage, Creature source)
        {
            BattleState = BattleState.InBattle;
            Attributes.HP -= damage.Damage;
            if (Attributes.HP < 0)
            {
                IsDeath = true;
                damage.WillDead = true;
            }
            OnDamage(damage, source);
        }

        public virtual void OnDamage(NDamageInfo damage, Creature source)
        {
        }

        public override void Update()
        {
            SkillMgr.Update();
            BuffMgr.Update();
        }

        internal int Distance(Creature target)
        {
            return (int)Vector3Int.Distance(Position, target.Position);
        }
        internal int Distance(Vector3Int position)
        {
            return (int)Vector3Int.Distance(Position, position);
        }

        internal void AddBuff(BattleContext context, BuffDefine buffDefine)
        {
            BuffMgr.AddBuff(context, buffDefine);
        }
    }
}
