using Assets.Scripts.Battle;
using Common.Battle;
using Common.Data;
using Managers;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Runtime.Remoting.Channels;
using UnityEngine;

namespace Entities
{
    public class Creature : Entity
    {
        /// <summary>
        /// 角色信息（来自网络）
        /// </summary>
        public NCharacterInfo Info;

        /// <summary>
        /// 角色配置（来自本地表）
        /// </summary>
        public CharacterDefine Define;

        public Attributes Attributes;

        public SkillManager skillMgr;

        public BuffManager BuffMgr;

        public EffectManager EffectMgr;

        #region 属性
        public int Id
        {
            get { return this.Info.Id; }
        }

        public string Name
        {
            get
            {
                if (this.Info.Type == CharacterType.Player)
                    return this.Info.Name;
                else
                    return this.Define.Name;
            }
        }


        private bool battleState = false;
        public bool BattleState
        {
            get { return battleState; }
            set
            {
                if (battleState != value)
                {
                    battleState = value;
                    SetStandby(value);
                }
            }
        }

        public Skill CastingSkill = null;

        public bool IsPlayer
        {
            get
            {
                return this.Info.Type == CharacterType.Player;
            }
        }

        public bool IsCurrentPlayer
        {
            get
            {
                if (!IsPlayer) return false;
                return this.Info.Id == Models.User.Instance.CurrentCharacterInfo.Id;
            }
        }
        #endregion


        public Action<Buff> OnBuffAdd;
        public Action<Buff> OnBuffRemove;


        public Creature(NCharacterInfo info) : base(info.Entity)
        {
            this.Info = info;
            this.Define = DataManager.Instance.Characters[info.ConfigId];
            Attributes = new Attributes();
            Attributes.Init(Define, Info.Level, GetEquips(), Info.attrDynamic);
            skillMgr = new SkillManager(this);
            BuffMgr = new BuffManager(this);
            EffectMgr = new EffectManager(this);
        }

        public void UpdateInfo(NCharacterInfo info)
        {
            SetEntityData(info.Entity);
            this.Info = info;
            Attributes.Init(Define, info.Level, GetEquips(), info.attrDynamic);
            skillMgr.UpdateSkills();
        }

        public virtual List<EquipDefine> GetEquips()
        {
            return null;
        }

        #region 移动
        public void MoveForward()
        {
            //Debug.LogFormat("MoveForward");
            this.speed = this.Define.Speed;
        }

        public void MoveBack()
        {
            //Debug.LogFormat("MoveBack");
            this.speed = -this.Define.Speed;
        }

        public void Stop()
        {
            //Debug.LogFormat("Stop");
            this.speed = 0;
        }

        public void SetDirection(Vector3Int direction)
        {
            //Debug.LogFormat("SetDirection:{0}", direction);
            this.direction = direction;
        }

        public void SetPosition(Vector3Int position)
        {
            //Debug.LogFormat("SetPosition:{0}", position);
            this.position = position;
        }
        #endregion

        public void CastSkill(int skillId, Creature target, NVector3 position)
        {
            SetStandby(true);
            Skill skill = skillMgr.GetSkill(skillId);
            skill.BeginCast(target, position);
        }

        public void PlayAnim(string name)
        {
            if (Controller != null)
                Controller.PlayAnim(name);
        }

        public void SetStandby(bool standby)
        {
            if (Controller != null)
                Controller.SetStandby(standby);
        }

        public override void OnUpdate(float delta)
        {
            base.OnUpdate(delta);

            skillMgr.OnUpdate(delta);
            BuffMgr.OnUpdate(delta);
        }

        internal void DoDamage(NDamageInfo damage,bool playHurt)
        {
            Debug.LogFormat("DoDamage:{0} DMG:{1} CRI:{2}", Name, damage.Damage, damage.Crit);
            Attributes.HP -= damage.Damage;
            if (playHurt)
            {
                PlayAnim("Hurt");
            }
            if (Controller != null)
            {
                UIWorldEleManagerMe.Instance.ShowPopupText(PopupType.Damage, Controller.GetTransform().position + GetPopupOffset(), -damage.Damage, damage.Crit);
            }
        }

        internal void DoSkillHit(NSkillHitInfo hit)
        {
            var skill = skillMgr.GetSkill(hit.skillId);
            skill.DoHit(hit);
        }

        internal int Distance(Creature target)
        {
            return (int)Vector3Int.Distance(this.position, target.position);
        }

        internal void DoBuffAction(NBuffInfo buff)
        {
            switch (buff.Action)
            {
                case BuffAction.Add:
                    AddBuff(buff.buffId, buff.buffType, buff.casterId);
                    break;
                case BuffAction.Remove:
                    RemoveBuff(buff.buffId);
                    break;
                case BuffAction.Hit:
                    DoDamage(buff.Damage,false);
                    break;
                default:
                    break;
            }
        }

        private void AddBuff(int buffId, int buffType, int casterId)
        {
            var buff = BuffMgr.AddBuff(buffId, buffType, casterId);
            if (buff != null && OnBuffAdd != null)
            {
                OnBuffAdd(buff);
            }
        }

        public void RemoveBuff(int buffId)
        {
            var buff = BuffMgr.RemoveBuff(buffId);
            if (buff != null && OnBuffRemove != null)
            {
                OnBuffRemove(buff);
            }
        }

        internal void AddBuffEffect(BuffEffect effect)
        {
            EffectMgr.AddEffect(effect);
        }

        internal void RemoveBuffEffect(BuffEffect effect)
        {
            EffectMgr.RemoveEffect(effect);
        }

        internal void FaceTo(Vector3Int position)
        {
            SetDirection(GameObjectTool.WorldToLogic(GameObjectTool.LogicToWorld(position - this.position).normalized));
            UpdateEntityData();
            if (Controller != null)
            {
                Controller.UpdateDirection();
            }
        }

        internal void PlayEffect(EffectType type, string name, Creature target, float duration = 0)
        {
            if (string.IsNullOrEmpty(name)) return;
            if (Controller != null)
            {
                Controller.PlayEffect(type, name, target, duration);
            }
        }

        internal void PlayEffect(EffectType type, string name, NVector3 position)
        {
            if (string.IsNullOrEmpty(name)) return;
            if (Controller != null)
            {
                Controller.PlayEffect(type, name, position, 0);
            }
        }


        public Vector3 GetPopupOffset()
        {
            return new Vector3(0, Define.Height, 0);
        }

        internal Vector3 GetHitOffset()
        {
            return new Vector3(0, Define.Height * 0.8f, 0);
        }
    }
}
