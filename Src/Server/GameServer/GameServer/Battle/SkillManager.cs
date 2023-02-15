using GameServer.Entities;
using GameServer.Managers;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Battle
{
    class SkillManager
    {
        private Creature Owner;
        public Skill NormalSkill { get; private set; }

        public List<Skill> Skills { get; private set; }
        public List<NSkillInfo> Infos { get; private set; }

        public SkillManager(Creature owner)
        {
            Owner = owner;
            Skills = new List<Skill>();
            Infos = new List<NSkillInfo>();
            InitSkills();
        }

        private void InitSkills()
        {
            Skills.Clear();
            Infos.Clear();
            /*正常这里应该从数据库读取当前技能信息，（扩展作业）*/
            if (!DataManager.Instance.Skills.ContainsKey(Owner.Define.TID))
                return;
            foreach (var define in DataManager.Instance.Skills[Owner.Define.TID])
            {
                NSkillInfo info = new NSkillInfo();
                info.Id = define.Key;
                if (Owner.Info.Level >= define.Value.UnlockLevel)
                {
                    info.Level = 5;
                }
                else
                {
                    info.Level = 1;
                }
                Infos.Add(info);
                var skill = new Skill(info, Owner);
                if(define.Value.Type == Common.Battle.SkillType.Normal)
                {
                    NormalSkill = skill;
                }
                AddSkill(skill);
            }
        }

        public void AddSkill(Skill skill)
        {
            Skills.Add(skill);
        }

        internal Skill GetSkill(int skillId)
        {
            for (int i = 0; i < Skills.Count; i++)
            {
                if (Skills[i].Define.ID == skillId)
                {
                    return Skills[i];
                }
            }
            return null;
        }

        internal void Update()
        {
            for (int i = 0; i < Skills.Count; i++)
            {
                Skills[i].Update();
            }
        }
    }
}
