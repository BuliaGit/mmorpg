using Common.Data;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Battle
{
    public class Attributes
    {
        public AttributeData Initial = new AttributeData();
        public AttributeData Growth = new AttributeData();
        public AttributeData Equip = new AttributeData();
        //初始+成长+装备
        public AttributeData Basic = new AttributeData();
        public AttributeData Buff = new AttributeData();
        public AttributeData Final = new AttributeData();

        int Level;

        public NAttributeDynamic DynamicAttr;

        public float HP
        {
            get { return DynamicAttr.Hp; }
            set { DynamicAttr.Hp = (int)Math.Min(MaxHP, value); }
        }
        public float MP
        {
            get { return DynamicAttr.Mp; }
            set { DynamicAttr.Mp = (int)Math.Min(MaxMP, value); }
        }

        /// <summary>
        /// 最大生命
        /// </summary>
        public float MaxHP { get { return Final.MaxHP; } }
        /// <summary>
        /// 最大法力
        /// </summary>
        public float MaxMP { get { return Final.MaxMP; } }
        /// <summary>
        /// 力量
        /// </summary>
        public float STR { get { return Final.STR; } }
        /// <summary>
        /// 智力
        /// </summary>
        public float INT { get { return Final.INT; } }
        /// <summary>
        /// 敏捷
        /// </summary>
        public float DEX { get { return Final.DEX; } }
        /// <summary>
        /// 物理攻击
        /// </summary>
        public float AD { get { return Final.AD; } }
        /// <summary>
        /// 法术攻击
        /// </summary>
        public float AP { get { return Final.AP; } }
        /// <summary>
        /// 物理防御
        /// </summary>
        public float DEF { get { return Final.DEF; } }
        /// <summary>
        /// 法术防御
        /// </summary>
        public float MDEF { get { return Final.MDEF; } }
        /// <summary>
        /// 暴击概率
        /// </summary>
        public float CRI { get { return Final.CRI; } }

        /// <summary>
        /// 初始化角色属性
        /// </summary>
        /// <param name="define"></param>
        /// <param name="level"></param>
        /// <param name="equips"></param>
        /// <param name="dynamicAttr"></param>
        public void Init(CharacterDefine define, int level, List<EquipDefine> equips, NAttributeDynamic dynamicAttr)
        {
            DynamicAttr = dynamicAttr;
            LoadInitAttribute(Initial, define);
            LoadGrowthAttributes(Growth, define);
            LoadEquipAttributes(Equip, equips);
            Level = level;
            InitBasicAttributes();
            InitSecondaryAttributes();

            InitFinalAttributes();
            if (DynamicAttr == null)
            {
                DynamicAttr = new NAttributeDynamic();
                HP = MaxHP;
                MP = MaxMP;
            }
            HP = DynamicAttr.Hp;
            MP = DynamicAttr.Mp;
        }

        /// <summary>
        /// 加载初始属性
        /// </summary>
        /// <param name="initial"></param>
        /// <param name="define"></param>
        private void LoadInitAttribute(AttributeData initial, CharacterDefine define)
        {
            initial.MaxHP = define.MaxHP;
            initial.MaxMP = define.MaxMP;

            initial.STR = define.STR;
            initial.INT = define.INT;
            initial.DEX = define.DEX;
            initial.AD = define.AD;
            initial.AP = define.AP;
            initial.DEF = define.DEF;
            initial.MDEF = define.MDEF;
            initial.SPD = define.SPD;
            initial.CRI = define.CRI;
        }

        /// <summary>
        /// 加载成长属性
        /// </summary>
        /// <param name="growth"></param>
        /// <param name="define"></param>
        private void LoadGrowthAttributes(AttributeData growth, CharacterDefine define)
        {
            growth.STR = define.GrowthSTR;
            growth.INT = define.GrowthINT;
            growth.DEX = define.GrowthDEX;
        }

        /// <summary>
        /// 加载装备叠加属性
        /// </summary>
        /// <param name="equip"></param>
        /// <param name="equips"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void LoadEquipAttributes(AttributeData equipAttr, List<EquipDefine> equips)
        {
            equipAttr.Reset();

            if (equips == null) return;

            foreach (var item in equips)
            {
                equipAttr.MaxHP += item.MaxHP;
                equipAttr.MaxMP += item.MaxMP;
                equipAttr.STR += item.STR;
                equipAttr.INT += item.INT;
                equipAttr.DEX += item.DEX;
                equipAttr.AD += item.AD;
                equipAttr.AP += item.AP;
                equipAttr.DEF += item.DEF;
                equipAttr.MDEF += item.MDEF;
                equipAttr.SPD += item.SPD;
                equipAttr.CRI += item.CRI;
            }
        }

        /// <summary>
        /// 初始基础一级属性
        /// </summary>
        private void InitBasicAttributes()
        {
            for (int i = (int)AttributeType.MaxHP; i < (int)AttributeType.MAX; i++)
            {
                Basic.Data[i] = Initial.Data[i];
            }
            for (int i = (int)AttributeType.STR; i < (int)AttributeType.DEX; i++)
            {
                Basic.Data[i] = Initial.Data[i] + Growth.Data[i] * (Level - 1);//一级属性成长
                Basic.Data[i] += Equip.Data[i];//装备一级属性加成在计算属性前
            }
        }

        /// <summary>
        /// 初始二级属性
        /// </summary>
        private void InitSecondaryAttributes()
        {
            //二级属性成长（包括装备）
            Basic.MaxHP = Basic.STR * 10 + Initial.MaxHP + Equip.MaxHP;
            Basic.MaxMP = Basic.INT * 10 + Initial.MaxMP + Equip.MaxMP;

            Basic.AD = Basic.STR * 5 + Initial.AD + Equip.AD;
            Basic.AP = Basic.INT * 5 + Initial.AP + Equip.AP;
            Basic.DEF = Basic.STR * 2 + Basic.DEX * 1 + Initial.DEF + Equip.DEF;
            Basic.MDEF = Basic.INT * 2 + Basic.DEX * 1 + Initial.MDEF + Equip.MDEF;

            Basic.SPD = Basic.DEX * 0.2f + Initial.SPD + Equip.SPD;
            Basic.CRI = Basic.DEX * 0.0002f + Initial.CRI + Equip.CRI;
        }

        /// <summary>
        /// 初始最终属性
        /// </summary>
        public void InitFinalAttributes()
        {
            for (int i = (int)AttributeType.MaxHP; i < (int)AttributeType.MAX; i++)
            {
                Final.Data[i] = Basic.Data[i] + Buff.Data[i];
            }
        }
    }
}
