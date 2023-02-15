using Assets.Scripts.Managers.Me;
using Common;
using Common.Battle;
using Common.Data;
using Entities;
using Managers;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.Battle
{
    public class Skill
    {
        public NSkillInfo nSkillInfo;
        public Creature owner;
        public SkillDefine define;
        public Creature Target;
        private NVector3 TargetPosition;
        private float cd = 0;

        public float CD
        {
            get { return cd; }
        }

        public bool IsCasting = false;
        public float castTime = 0;

        private float skillTime = 0;
        public int Hit = 0;

        private SkillStatus Status;

        Dictionary<int, List<NDamageInfo>> HitMap = new Dictionary<int, List<NDamageInfo>>();

        List<Bullet> Bullets = new List<Bullet>();

        public Skill(NSkillInfo info, Creature owner)
        {
            nSkillInfo = info;
            this.owner = owner;
            define = DataManager.Instance.Skills[(int)owner.Define.TID][nSkillInfo.Id];
            cd = 0;
        }

        public SkillResult CanCast(Creature target)
        {
            if (define.CastTarget == Common.Battle.TargetType.Target)
            {
                if (target == null || target == owner)
                    return SkillResult.InvalidTarget;

                int distance = owner.Distance(target);
                if (distance > define.CastRange)
                {
                    return SkillResult.OutOfRange;
                }
            }

            if (owner.Attributes.MP < define.MPCost)
            {
                return SkillResult.OutOfMp;
            }

            if (define.CastTarget == Common.Battle.TargetType.Position && BattleManager.Instance.CurrentPosition == null)
            {
                return SkillResult.InvalidTarget;
            }

            if (cd > 0)
            {
                return SkillResult.CoolDown;
            }

            return SkillResult.Ok;
        }

        public void BeginCast(Creature target, NVector3 pos)
        {
            this.IsCasting = true;
            this.castTime = 0;
            this.skillTime = 0;
            this.Hit = 0;
            this.cd = this.define.CD;
            Target = target;
            this.TargetPosition = pos;
            owner.PlayAnim(define.SkillAnim);
            Bullets.Clear();
            HitMap.Clear();

            if (define.CastTarget == TargetType.Position)
            {
                owner.FaceTo(TargetPosition.ToVector3Int());
            }
            else if (define.CastTarget == TargetType.Target)
            {
                owner.FaceTo(target.position);
            }

            if (define.CastTime > 0)
            {
                Status = SkillStatus.Casting;
            }
            else
            {
                StartSkill();
            }
        }

        private void StartSkill()
        {
            Status = SkillStatus.Running;
            if (!string.IsNullOrEmpty(define.AOEEffect))
            {
                if (define.CastTarget == TargetType.Position)
                    owner.PlayEffect(EffectType.Position, define.AOEEffect, TargetPosition);
                else if (define.CastTarget == TargetType.Target)
                    owner.PlayEffect(EffectType.Position, define.AOEEffect, Target);
                else if (define.CastTarget == TargetType.Self)
                    owner.PlayEffect(EffectType.Position, define.AOEEffect, owner);
            }
        }

        internal void Cast()
        {
            throw new NotImplementedException();
        }

        public void OnUpdate(float delta)
        {
            UpdateCD(delta);

            if (Status == SkillStatus.Casting)
            {
                UpdateCasting();
            }
            else if (Status == SkillStatus.Running)
            {
                UpdateSkill();
            }

        }

        private void UpdateCasting()
        {
            if (castTime < define.CastTime)
            {
                castTime += Time.deltaTime;
            }
            else
            {
                castTime = 0;
                StartSkill();
                Debug.LogFormat("Skill[{0}].UpdateCasting Finish", define.Name);
            }
        }

        private void UpdateSkill()
        {
            skillTime += Time.deltaTime;

            if (define.Duration > 0)
            {
                //持续技能
                if (skillTime > define.Interval * (Hit + 1))
                {
                    DoHit();
                }

                if (skillTime >= define.Duration)
                {
                    Status = SkillStatus.None;
                    Debug.LogFormat("Skill[{0}].UpdateSkill Finish", define.Name);
                }
            }
            else if (define.HitTimes != null && define.HitTimes.Count > 0)
            {
                //多次打击技能
                if (Hit < define.HitTimes.Count)
                {
                    if (skillTime > define.HitTimes[Hit])
                    {
                        DoHit();
                    }
                }
                else
                {
                    if (!define.Bullet)
                    {
                        Status = SkillStatus.None;
                        IsCasting = false;
                        Debug.LogFormat("Skill[{0}].UpdateSkill Finish", define.Name);
                    }
                }
            }

            if (define.Bullet)
            {
                bool finish = true;
                foreach (Bullet bullet in Bullets)
                {
                    bullet.Update();
                    if (!bullet.Stoped) finish = false;
                }

                if (finish && Hit >= define.HitTimes.Count)
                {
                    Status = SkillStatus.None;
                    IsCasting = false;
                    Debug.LogFormat("Skill[{0}].UpdateSkill Finish", define.Name);
                }
            }
        }

        private void DoHit()
        {
            if (define.Bullet)
            {
                CastBullet();
            }
            else
            {
                DoHitDamages(Hit);
            }
            Hit++;
        }

        public void DoHitDamages(int hit)
        {
            List<NDamageInfo> damages;
            if (HitMap.TryGetValue(hit, out damages))
            {
                DoHitDamages(damages);
            }
        }

        private void CastBullet()
        {
            Bullet bullet = new Bullet(this);
            Debug.LogFormat("Skill[{0}].CastBullet[{1}] Target:{2}", define.Name, define.BulletResource, Target.Name);
            Bullets.Add(bullet);
            owner.PlayEffect(EffectType.Bullet, define.BulletResource, Target, bullet.duration);
        }

        private void UpdateCD(float delta)
        {
            if (cd > 0)
            {
                cd -= delta;
            }
            if (cd < 0)
            {
                cd = 0;
            }
        }

        internal void DoHit(NSkillHitInfo hitInfo)
        {
            if (hitInfo.isBullet || define.Bullet)
            {
                DoHit(hitInfo.hitId, hitInfo.Damages);
            }
        }

        internal void DoHit(int hitId, List<NDamageInfo> damages)
        {
            if (hitId > Hit)
            {
                HitMap[hitId] = damages;
            }
            else
            {
                DoHitDamages(damages);
            }
        }

        private void DoHitDamages(List<NDamageInfo> damages)
        {
            foreach (var dmg in damages)
            {
                Creature target = EntityManager.Instance.GetEntity(dmg.entityId) as Creature;
                if (target == null) continue;
                target.DoDamage(dmg,true);
                if(define.HitEffect != null)
                {
                    target.PlayEffect(EffectType.Hit, define.HitEffect, target);
                }
            }
        }
    }
}
