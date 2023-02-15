using Common.Data;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Battle
{
    public class BuffManager
    {
        private Creature Owner;

        public Dictionary<int, Buff> Buffs = new Dictionary<int, Buff>();

        public BuffManager(Creature creature)
        {
            this.Owner = creature;
        }

        internal Buff AddBuff(int buffId, int buffType, int casterId)
        {
            BuffDefine buffDefine;
            if(DataManager.Instance.Buffs.TryGetValue(buffType, out buffDefine))
            {
                Buff buff = new Buff(Owner, buffId, buffDefine, casterId);
                Buffs[buffId] = buff;
                return buff;
            }
            return null;
        }

        internal Buff RemoveBuff(int buffId)
        {
            Buff buff;
            if(Buffs.TryGetValue(buffId, out buff))
            {
                buff.OnRemove();
                Buffs.Remove(buffId);
                return buff;
            }
            return null;
        }

        public void OnUpdate(float delta)
        {
            List<int> needRemove = new List<int>();
            foreach (var item in Buffs)
            {
                item.Value.OnUpdate(delta);
                if (item.Value.Stoped)
                {
                    needRemove.Add(item.Key);
                }
            }
            foreach (var item in needRemove)
            {
                Owner.RemoveBuff(item);
            }
        }
    }
}
