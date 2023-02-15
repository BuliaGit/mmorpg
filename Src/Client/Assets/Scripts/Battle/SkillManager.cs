using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Battle
{
    public class SkillManager
    {
        Creature owner;

        //public delegate void SkillInfoUpdateHandle();
        //public event SkillInfoUpdateHandle OnSkillInfoUpdate;

        public List<Skill> Skills { get;private set; }
        
        public SkillManager(Creature owner)
        {
            this.owner = owner;
            Skills = new List<Skill>();
            InitSkills();
        }

        private void InitSkills()
        {
            Skills.Clear();
            foreach (var skillInfo in owner.Info.Skills)
            {
                Skill skill = new Skill(skillInfo, owner);
                AddSkill(skill);
            }
            //if(OnSkillInfoUpdate != null)
            //{
            //    OnSkillInfoUpdate();
            //}
        }

        public void UpdateSkills()
        {
            foreach (var skillInfo in owner.Info.Skills)
            {
                Skill skill = GetSkill(skillInfo.Id);
                if(skill != null)
                {
                    skill.nSkillInfo = skillInfo;
                }
                else
                {
                    AddSkill(skill);
                }
            }
            //if (OnSkillInfoUpdate != null)
            //{
            //    OnSkillInfoUpdate();
            //}
        }

        public void AddSkill(Skill skill)
        {
            Skills.Add(skill);
        }

        public Skill GetSkill(int skillId)
        {
            for (int i = 0; i < Skills.Count; i++)
            {
                if (Skills[i].define.ID == skillId)
                {
                    return Skills[i];
                }
            }
            return null;
        }

        public void OnUpdate(float delta)
        {
            for (int i = 0; i < Skills.Count; i++)
            {
                Skills[i].OnUpdate(delta);
            }
        }

        
    }
}
