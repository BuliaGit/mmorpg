using Common;
using GameServer.Entities;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Battle
{
    class Bullet
    {
        private Skill skill;
        NSkillHitInfo hitInfo;

        bool TimeMode = true;
        public bool Stoped = false;

        float flyTime = 0;
        float duration = 0;

        public Bullet(Skill skill, Creature target,NSkillHitInfo hitInfo)
        {
            this.skill = skill;
            this.hitInfo = hitInfo;
            int distance = skill.Owner.Distance(target);
            if (TimeMode)
            {
                duration = distance / skill.Define.BulletSpeed;
            }
            Log.InfoFormat("Bullet[{0}].CastBullet[{1}] Target:{2} Distance:{3} Time:{4}", skill.Define.Name, skill.Define.BulletResource, target.Name, distance, duration);
        }

        public void Update()
        {
            if (Stoped) return;
            if (TimeMode)
            {
                UpdateTime();
            }
            else
            {
                UpdatePos();
            }
        }

        private void UpdateTime()
        {
            flyTime += Time.deltaTime;
            if(flyTime > duration)
            {
                hitInfo.isBullet = true;
                skill.DoHit(hitInfo);
                Stoped = true;
            }
        }

        private void UpdatePos()
        {
            
        }
    }
}
