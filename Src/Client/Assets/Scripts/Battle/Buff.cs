using Common.Data;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Battle
{
    public class Buff
    {
        private Creature owner;
        public int buffId;
        public BuffDefine buffDefine;
        private int casterId;

        public float buffTime;
        private int hit;

        public bool Stoped { get; internal set; }

        public Buff(Creature owner, int buffId, BuffDefine buffDefine, int casterId)
        {
            this.owner = owner;
            this.buffId = buffId;
            this.buffDefine = buffDefine;
            this.casterId = casterId;

            OnAdd();
        }

        private void OnAdd()
        {
            Debug.LogFormat("BuffOnAdd[{0}:{1}]", buffId, buffDefine.Name);
            if(buffDefine.Effect != Common.Battle.BuffEffect.None)
            {
                owner.AddBuffEffect(buffDefine.Effect);
            }

            AddAttr();
        }

        internal void OnRemove()
        {
            Debug.LogFormat("BuffOnRemove[{0}:{1}]", buffId, buffDefine.Name);
            RemoveAttr();

            Stoped = true;
            if (buffDefine.Effect != Common.Battle.BuffEffect.None)
            {
                owner.RemoveBuffEffect(buffDefine.Effect);
            }
        }
        
        internal void OnUpdate(float delta)
        {
            if (Stoped) return;

            buffTime += delta;

            if (buffTime > buffDefine.Duration)
            {
                OnRemove();
            }
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

    }
}
