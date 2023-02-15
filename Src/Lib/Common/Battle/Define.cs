using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Battle
{
    public enum AttributeType
    {
        None = -1,
        /// <summary>
        /// 最大生命
        /// </summary>
        MaxHP = 0,
        /// <summary>
        /// 最大法力
        /// </summary>
        MaxMP = 1,
        /// <summary>
        /// 力量
        /// </summary>
        STR = 2,
        /// <summary>
        /// 智力
        /// </summary>
        INT = 3,
        /// <summary>
        /// 敏捷
        /// </summary>
        DEX = 4,
        /// <summary>
        /// 物理攻击
        /// </summary>
        AD = 5,
        /// <summary>
        /// 法术攻击
        /// </summary>
        AP = 6,
        /// <summary>
        /// 物理防御
        /// </summary>
        DEF = 7,
        /// <summary>
        /// 法术防御
        /// </summary>
        MDEF = 8,
        /// <summary>
        /// 攻击速度
        /// </summary>
        SPD = 9,
        /// <summary>
        /// 暴击概率
        /// </summary>
        CRI = 10,

        MAX
    }
    [Flags]
    public enum SkillType
    {
        All = -1,
        Normal = 1, //普通攻击
        Skill = 2,
        Passive = 4 //被动技能
    }

    public enum TargetType
    {
        None = 0,
        Target = 1,
        Self = 2,
        Position
    }

    public enum BuffEffect
    {
        None = 0,
        Stun = 1,   //眩晕
        Invincible = 2  //无敌
    }

    public enum TriggerType
    {
        None = 0,
        SkillCast = 1,  //技能释放时
        SkillHit = 2    //技能命中时
    }

    public enum BattleState
    {
        None,
        Idle,   //空闲
        InBattle    //战斗中
    }
}
