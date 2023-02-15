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
    internal class AIAgent
    {
        private Monster monster;

        private AIBase ai;

        public AIAgent(Monster monster)
        {
            this.monster = monster;

            string ainame = monster.Define.AI;
            if (string.IsNullOrEmpty(ainame))
            {
                ainame = AIMonsterPassive.ID;
            }

            switch (ainame)
            {
                case AIMonsterPassive.ID:
                    this.ai = new AIMonsterPassive(monster);
                    break;
                case AIBoss.ID:
                    this.ai = new AIBoss(monster);
                    break;
            }
        }

        internal void OnDamage(NDamageInfo damage, Creature source)
        {
            if (ai != null)
            {
                ai.OnDamage(damage, source);
            }
        }

        internal void Update()
        {
            if (ai != null)
            {
                ai.Update();
            }
        }

    }
}
