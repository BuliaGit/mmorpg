using Common;
using Common.Data;
using GameServer.Entities;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
 using System.Linq;
using System.Management.Instrumentation;
using System.Net.Mime;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GameServer.Battle
{
    class Buff
    {
        public int buffID;
        private Creature owner;
        private BuffDefine buffDefine;
        private BattleContext context;
        private float buffTime = 0;
        private int hit = 0;

        public bool Stoped { get; internal set; }

        public Buff(int buffID, Creature owner, BuffDefine buffDefine, BattleContext context)
        {
            this.buffID = buffID;
            this.owner = owner;
            this.buffDefine = buffDefine;
            this.context = context;

            OnAdd();
        }

        private void OnAdd()
        {
            if(buffDefine.Effect != Common.Battle.BuffEffect.None)
            {
                owner.EffectMgr.AddEffect(buffDefine.Effect);
            }

            AddAttr();

            NBuffInfo buffInfo = new NBuffInfo()
            {
                buffId = this.buffID,
                buffType = buffDefine.ID,
                casterId = context.Caster.entityId,
                ownerId = owner.entityId,
                Action = BuffAction.Add
            };
            context.Battle.AddBuffAction(buffInfo);
        }

        private void OnRemove()
        {
            RemoveAttr();

            Stoped = true;
            if (buffDefine.Effect != Common.Battle.BuffEffect.None)
            {
                owner.EffectMgr.RemoveEffect(buffDefine.Effect);
            }

            NBuffInfo buffInfo = new NBuffInfo()
            {
                buffId = this.buffID,
                buffType = buffDefine.ID,
                casterId = context.Caster.entityId,
                ownerId = owner.entityId,
                Action = BuffAction.Remove
            };
            context.Battle.AddBuffAction(buffInfo);
        }
        private void AddAttr()
        {
            if (buffDefine.DEFRatio != 0)
            {
                owner.Attributes.Buff.DEF += owner.Attributes.Basic.DEF * buffDefine.DEFRatio;
                owner.Attributes.InitFinalAttributes();
            }
        }

        private void RemoveAttr()
        {
            if (buffDefine.DEFRatio != 0)
            {
                owner.Attributes.Buff.DEF -= owner.Attributes.Basic.DEF * buffDefine.DEFRatio;
                owner.Attributes.InitFinalAttributes();
            }
        }

        

        internal void Update()
        {
            if (Stoped) return;

            buffTime += Time.deltaTime;

            if(buffDefine.Interval > 0)
            {
                if(buffTime > buffDefine.Interval*(hit + 1))
                {
                    DoBuffDamage();
                }
            }

            if(buffTime > buffDefine.Duration)
            {
                OnRemove();
            }
        }

        private void DoBuffDamage()
        {
            hit++;

            NDamageInfo damage = CalcBuffDamage(context.Caster);
            Log.InfoFormat("Buff[{0}].DoBuffDamage[{1}] Damage:{2} Crit:{3}", buffDefine.Name, owner.Name, damage.Damage, damage.Crit);
            owner.DoDamage(damage,context.Caster);

            NBuffInfo buff = new NBuffInfo()
            {
                buffId = this.buffID,
                buffType = buffDefine.ID,
                casterId = context.Caster.entityId,
                ownerId = owner.entityId,
                Action = BuffAction.Hit,
                Damage = damage
            };
            context.Battle.AddBuffAction(buff);
        }

        private NDamageInfo CalcBuffDamage(Creature caster)
        {
            float ad = buffDefine.AD + caster.Attributes.AD * buffDefine.ADFactor;
            float ap = buffDefine.AP + caster.Attributes.AP * buffDefine.APFactor;

            float adDmg = ad * (1 - owner.Attributes.DEF / (owner.Attributes.DEF + 100));
            float apDmg = ap * (1 - owner.Attributes.MDEF / (owner.Attributes.MDEF + 100));

            float final = adDmg + apDmg;

            NDamageInfo damage = new NDamageInfo();
            damage.entityId = owner.entityId;
            damage.Damage = Math.Max(1, (int)final);
            return damage;
        }
    }
}
