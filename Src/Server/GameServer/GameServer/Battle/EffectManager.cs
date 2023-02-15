using Common;
using Common.Battle;
using GameServer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Battle
{
    internal class EffectManager
    {
        private Creature Owner;

        Dictionary<BuffEffect, int> Effects = new Dictionary<BuffEffect, int>();

        public EffectManager(Creature creature)
        {
            this.Owner = creature;
        }

        public bool HasEffect(BuffEffect effect)
        {
            if(Effects.TryGetValue(effect,out int val))
            {
                return val > 0;
            }
            return false;
        }

        internal void AddEffect(BuffEffect effect)
        {
            Log.InfoFormat("[{0}].AddEffect {1}", Owner.Name, effect);
            if (!Effects.ContainsKey(effect))
            {
                Effects[effect] = 1;
            }
            else
            {
                Effects[effect]++;
            }
        }
        
        public void RemoveEffect(BuffEffect effect)
        {
            Log.InfoFormat("[{0}].RemoveEffect {1}", Owner.Name, effect);
            if (!Effects.ContainsKey(effect))
            {
                Log.InfoFormat("don't include effect");
                return;
            }
            if (Effects[effect] > 0)
            {
                Effects[effect]--;
            }
        }
    }
}
