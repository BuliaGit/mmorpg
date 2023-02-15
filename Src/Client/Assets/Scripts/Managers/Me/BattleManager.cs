using Assets.Scripts.Battle;
using Assets.Scripts.Services;
using Entities;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Managers.Me
{
    class BattleManager : Singleton<BattleManager>
    {
        public delegate void TargetChangedHandler(Creature target);
        public event TargetChangedHandler OnTargetChanged;

        private Creature currentTarget;
        public Creature CurrentTarget
        {
            get { return currentTarget; }
            set
            {
                this.SetTarget(value);
            }
        }

        private NVector3 currentPosition;
        public NVector3 CurrentPosition
        {
            get { return currentPosition; }
            set
            {
                this.SetPosition(value);
            }
        }

        public void Init()
        {

        }


        private void SetTarget(Creature target)
        {
            if (currentTarget != target && OnTargetChanged != null)
            {
                OnTargetChanged(target);
            }

            currentTarget = target;
            Debug.LogFormat("BattleManager.SetTarget:[{0}:{1}]", target.entityId, target.Name);
        }
        private void SetPosition(NVector3 position)
        {
            currentPosition = position;
            Debug.LogFormat("BattleManager.SetPosition:[{0},{1},{2}]", position.X, position.Y, position.Z);
        }
        public void CastSkill(Skill skill)
        {
            int targetId = currentTarget != null ? currentTarget.entityId : 0;
            BattleService.Instance.SendSkillCast(skill.define.ID, skill.owner.entityId, targetId, currentPosition);
        }
    }
}
