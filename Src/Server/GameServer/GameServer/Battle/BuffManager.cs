using Common.Data;
using GameServer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Battle
{
    internal class BuffManager
    {
        private Creature Owner;

        private List<Buff> Buffs = new List<Buff>();

        private int idx = 1;
        private int BuffID
        {
            get { return idx++; }
        }

        public BuffManager(Creature creature)
        {
            this.Owner = creature;
        }

        internal void AddBuff(BattleContext context, BuffDefine buffDefine)
        {
            Buff buff = new Buff(BuffID,Owner, buffDefine,context);
            Buffs.Add(buff);
        }

        internal void Update()
        {
            for (int i = 0; i < Buffs.Count; i++)
            {
                if (!Buffs[i].Stoped)
                {
                    Buffs[i].Update();
                }
            }
            Buffs.RemoveAll((b) => b.Stoped);
        }
    }
}
