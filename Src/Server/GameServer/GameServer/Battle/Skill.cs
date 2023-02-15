using Common;
using Common.Battle;
using Common.Data;
using Common.Utils;
using GameServer.Core;
using GameServer.Entities;
using GameServer.Managers;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Battle
{
    class Skill
    {
        public NSkillInfo Info;
        public Creature Owner;
        public SkillDefine Define;

        public SkillStatus Status;

        private float cd = 0;
        public float CD
        {
            get { return cd; }
        }

        private float castingTime = 0;
        private float skillTime = 0;
        private int Hit = 0;
        BattleContext Context;

        List<Bullet> Bullets = new List<Bullet>();

        public bool Instant
        {
            get
            {
                if (Define.CastTime > 0) return false;
                if (Define.Bullet) return false;
                if (Define.Duration > 0) return false;
                if (Define.HitTimes != null && Define.HitTimes.Count > 0) return false;
                return true;
            }
        }

        public Skill(NSkillInfo info, Creature owner)
        {
            Info = info;
            Owner = owner;
            Define = DataManager.Instance.Skills[(int)owner.Define.TID][info.Id];
        }

        public SkillResult CanCast(BattleContext context)
        {
            if (this.Status != SkillStatus.None)
            {
                return SkillResult.Casting;
            }

            if (Define.CastTarget == Common.Battle.TargetType.Target)
            {
                if (context.Target == null || context.Target == this.Owner)
                {
                    return SkillResult.InvalidTarget;
                }

                int distance = Owner.Distance(context.Target);
                if (distance > Define.CastRange)
                {
                    return SkillResult.OutOfRange;
                }
            }

            if (Define.CastTarget == Common.Battle.TargetType.Position)
            {
                if (context.CastSkill.Position == null)
                {
                    return SkillResult.InvalidTarget;
                }
                if (Owner.Distance(context.Position) > Define.CastRange)
                {
                    return SkillResult.OutOfRange;
                }
            }

            if (Owner.Attributes.MP < Define.MPCost)
            {
                return SkillResult.OutOfMp;
            }

            if (cd > 0)
            {
                return SkillResult.CoolDown;
            }

            return SkillResult.Ok;
        }

        internal SkillResult Cast(BattleContext context)
        {
            SkillResult result = CanCast(context);
            if (result == SkillResult.Ok)
            {
                castingTime = 0;
                skillTime = 0;
                cd = Define.CD;
                this.Context = context;
                Hit = 0;
                Bullets.Clear();

                AddBuff(TriggerType.SkillCast,this.Context.Target);

                if (this.Instant)
                {
                    DoHit();
                }
                else
                {
                    if (Define.CastTime > 0)
                    {
                        Status = SkillStatus.Casting;
                    }
                    else
                    {
                        Status = SkillStatus.Running;
                    }
                }
            }
            Log.InfoFormat("SKill[{0}].Cast Result:[{1}] Status:{2}", Define.Name, result, Status);
            return result;
        }


        private void AddBuff(TriggerType trigger,Creature target)
        {
            if (Define.Buff == null || Define.Buff.Count == 0) return;

            foreach (var buffId in Define.Buff)
            {
                var buffDefine = DataManager.Instance.Buffs[buffId];

                if (buffDefine.Trigger != trigger) continue;

                if(buffDefine.Target == TargetType.Self)
                {
                    Owner.AddBuff(Context, buffDefine);
                }
                else if(buffDefine.Target == TargetType.Target)
                {
                    target.AddBuff(Context, buffDefine);
                }
            }
        }


        NSkillHitInfo InitHitInfo(bool isBullet)
        {
            NSkillHitInfo hitInfo = new NSkillHitInfo()
            {
                casterId = Context.Caster.entityId,
                skillId = Info.Id,
                hitId = Hit,
                isBullet = isBullet
            };
            return hitInfo;
        }
        private void DoHit()
        {
            NSkillHitInfo hitInfo = InitHitInfo(false);
            Log.InfoFormat("Skill[{0}].DoHit[{1}]", Define.Name, Hit);
            Hit++;

            if (Define.Bullet)
            {
                CastBullet(hitInfo);
                return;
            }

            DoHit(hitInfo);
        }

        public void DoHit(NSkillHitInfo hitInfo)
        {
            Context.Battle.AddHitInfo(hitInfo);
            if (Define.AOERange > 0)
            {
                HitRange(hitInfo);
                return;
            }

            if(Define.CastTarget == Common.Battle.TargetType.Target)
            {
                HitTarget(Context.Target,hitInfo);
            }
        }

        private void CastBullet(NSkillHitInfo hitInfo)
        {
            Context.Battle.AddHitInfo(hitInfo);
            Log.InfoFormat("Skill[{0}].CastBullet[{1}]", Define.Name, Define.BulletResource);

            Bullet bullet = new Bullet(this,Context.Target,hitInfo);
            Bullets.Add(bullet);
        }

        private void HitRange(NSkillHitInfo hitInfo)
        {
            Vector3Int pos;
            if (Define.CastTarget == Common.Battle.TargetType.Target)
            {
                pos = Context.Target.Position;
            }
            else if (Define.CastTarget == Common.Battle.TargetType.Position)
            {
                pos = Context.Position;
            }
            else
            {
                pos = Owner.Position;
            }

            List<Creature> units = Context.Battle.FindUnitsInMapRange(pos, Define.AOERange);
            foreach (var target in units)
            {
                HitTarget(target,hitInfo);
            }
        }

        private void HitTarget(Creature target,NSkillHitInfo hitInfo)
        {
            if(Define.CastTarget == Common.Battle.TargetType.Self && (target != Context.Caster))
            {
                return;
            }else if(target == Context.Caster)
            {
                return;
            }
            NDamageInfo damage = CalcSkillDamage(Context.Caster, target);
            Log.InfoFormat("Skill[{0}].HitTarget[{1}] Damage:{2} Crit:{3}", Define.Name, target.Name, damage.Damage, damage.Crit);
            target.DoDamage(damage,Context.Caster); 
            hitInfo.Damages.Add(damage);

            AddBuff(TriggerType.SkillHit,target);
        }


        


        private NDamageInfo CalcSkillDamage(Creature caster, Creature target)
        {
            float ad = Define.AD + caster.Attributes.AD * Define.ADFactor;
            float ap = Define.AP + caster.Attributes.AP * Define.APFactor;

            float addmg = ad * (1 - target.Attributes.DEF / (target.Attributes.DEF + 100));
            float apdmg = ap * (1 - target.Attributes.MDEF / (target.Attributes.MDEF + 100));

            float final = addmg + apdmg;
            bool isCrit = IsCrit(caster.Attributes.CRI);
            if (isCrit)
            {
                final = final * 2f;
            }

            //随机浮动
            final = final * (float)MathUtil.Random.NextDouble() * 0.1f - 0.05f;

            NDamageInfo damage = new NDamageInfo()
            {
                entityId = target.entityId,
                Damage = Math.Max(1, (int)final),
                Crit = isCrit
            };
            return damage;
        }

        private bool IsCrit(float crit)
        {
            return MathUtil.Random.NextDouble() < crit;
        }

        internal void Update()
        {
            UpdateCD();
            if (Status == SkillStatus.Casting)
            {
                UpdateCasting();
            }
            else
            {
                if (Status == SkillStatus.Running)
                {
                    UpdateSkill();
                }
            }
        }



        private void UpdateCasting()
        {
            if (castingTime < Define.CastTime)
            {
                castingTime += Time.deltaTime;
            }
            else
            {
                castingTime = 0;
                Status = SkillStatus.Running;
                Log.InfoFormat("Skill[{0}].UpdateCasting Finish", Define.Name);
            }
        }

        private void UpdateCD()
        {
            if (cd > 0)
            {
                cd -= Time.deltaTime;
            }
            if (cd < 0)
            {
                cd = 0;
            }
        }

        private void UpdateSkill()
        {
            skillTime += Time.deltaTime;

            if (Define.Duration > 0)
            {
                //持续技能
                if (skillTime > Define.Interval * (Hit + 1))
                {
                    DoHit();
                }

                if (skillTime >= Define.Duration)
                {
                    Status = SkillStatus.None;
                    Log.InfoFormat("SKill[{0}].UpdateSkill Finish", Define.Name);
                }
            }
            else if (Define.HitTimes != null && Define.HitTimes.Count > 0)
            {
                //次数技能
                if (Hit < Define.HitTimes.Count)
                {
                    if(skillTime >= Define.HitTimes[Hit])
                    {
                        DoHit();
                    }
                }
                else
                {
                    if (!Define.Bullet)
                    {
                        Status = SkillStatus.None;
                        Log.InfoFormat("SKill[{0}].UpdateSkill Finish", Define.Name);
                    }
                }
            }

            if (Define.Bullet)
            {
                bool finish = true;
                foreach (Bullet bullet in Bullets)
                {
                    bullet.Update();
                    if (!bullet.Stoped) finish = false;
                }

                if (finish && Hit >= Define.HitTimes.Count)
                {
                    Status = SkillStatus.None;
                    Log.InfoFormat("SKill[{0}].UpdateSkill Finish", Define.Name);
                }
            }

        }
    }
}
