using Common.Battle;
using GameServer.Battle;
using GameServer.Entities;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.AI
{
    internal class AIBase
    {

        private Creature target;

        private Monster owner;

        Skill normalSKill;

        public AIBase(Monster monster)
        {
            this.owner = monster;
            normalSKill = owner.SkillMgr.NormalSkill;
        }

        internal void OnDamage(NDamageInfo damage, Creature source)
        {
            target = source;
        }

        internal void Update()
        {
            if (owner.BattleState == BattleState.InBattle)
            {
                UpdateBattle();
            }
        }

        private void UpdateBattle()
        {
            if (target == null)
            {
                owner.BattleState = BattleState.Idle;
                return;
            }

            if (!TryCastSkill())
            {
                if (!TryCastNormal())
                {
                    FollowTarget();
                }
            }
        }

        private bool TryCastSkill()
        {
            if (target != null)
            {
                BattleContext context = new BattleContext(owner.Map.Battle)
                {
                    Target = this.target,
                    Caster = this.owner
                };
                Skill skill = owner.FindSkill(context, SkillType.Skill);
                if (skill != null)
                {

                    owner.CastSkill(context, skill.Define.ID);
                    return true;
                }
            }
            return false;
        }

        private bool TryCastNormal()
        {
            if (target != null)
            {
                BattleContext context = new BattleContext(owner.Map.Battle)
                {
                    Target = this.target,
                    Caster = this.owner
                };
                var result = normalSKill.CanCast(context);
                if (result == SkillResult.Ok)
                {
                    owner.CastSkill(context, normalSKill.Define.ID);
                }
                if (result == SkillResult.OutOfRange)
                {
                    return false;
                }
            }
            return true;
        }
        private void FollowTarget()
        {
            int distance = owner.Distance(target);
            if (distance > normalSKill.Define.CastRange - 50)
            {
                owner.MoveTo(target.Position);
            }
            else
            {
                owner.StopMove();
            }
        }
    }
}
