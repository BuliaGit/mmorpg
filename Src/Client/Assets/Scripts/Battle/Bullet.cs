using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Battle
{
    class Bullet
    {
        Skill skill;
        int hit = 0;
        float flyTime = 0;
        public float duration = 0;

        public bool Stoped = false;

        public Bullet(Skill skill)
        {
            this.skill = skill;
            var target = skill.Target;
            hit= skill.Hit;
            int distance = skill.owner.Distance(target);
            duration = distance / skill.define.BulletSpeed;
        }

        public void Update()
        {
            if (Stoped) return;
            flyTime += Time.deltaTime;
            if(flyTime > duration)
            {
                skill.DoHitDamages(hit);
                Stop();
            }
        }

        private void Stop()
        {
            Stoped = true;
        }
    }
}
