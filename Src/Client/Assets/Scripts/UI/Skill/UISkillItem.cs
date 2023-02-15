using Assets.Scripts.Battle;
using Common.Data;
using Models;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISkillItem : ListViewMe.ListViewItemMe
{
    public Image skillIcon;
    public Text skillName;
    public Text level;


    public Image backgr;
    public Sprite originImg;
    public Sprite selectImg;

    public Skill skill;
    public override void OnSelect(bool val)
    {
        backgr.overrideSprite = val ? selectImg : originImg;
    }

    public void SetSkillItemInfo(Skill skill)
    {
        this.skill = skill;
        if (this.skill == null) return;
        skillIcon.overrideSprite = Resloader.Load<Sprite>(skill.define.Icon);
        skillName.text = skill.define.Name;
        level.text = string.Format("Lv.{0}", skill.nSkillInfo.Level);
    }
}
