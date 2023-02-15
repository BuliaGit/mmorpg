using Common;
using Common.Battle;
using Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Assets.Scripts.Battle
{
    public class EffectManager
    {
        private Creature Owner;

        Dictionary<BuffEffect, int> Effects = new Dictionary<BuffEffect, int>();

        public EffectManager(Creature creature)
        {
            this.Owner = creature;
        }

        public bool HasEffect(BuffEffect effect)
        {
            int val;
            if (Effects.TryGetValue(effect, out val))
            {
                return val > 0;
            }
            return false;
        }

        internal void AddEffect(BuffEffect effect)
        {
            Debug.LogFormat("[{0}].AddEffect {1}", Owner.Name, effect);
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
            Debug.LogFormat("[{0}].RemoveEffect {1}", Owner.Name, effect);
            if (!Effects.ContainsKey(effect))
            {
                Debug.LogFormat("don't include effect");
                return;
            }
            if (Effects[effect] > 0)
            {
                Effects[effect]--;
            }
        }
    }
}
