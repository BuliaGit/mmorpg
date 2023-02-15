using Assets.Scripts.Battle;
using Assets.Scripts.Managers.Me;
using Common.Battle;
using Common.Data;
using JetBrains.Annotations;
using Models;
using SkillBridge.Message;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UISkillSlot : MonoBehaviour, IPointerClickHandler
{
    public Image icon;
    public Image overlay;
    public Text cdText;
    public Skill skill;

    private void Start()
    {
        overlay.enabled = false;
        cdText.enabled = false;
    }

    private void Update()
    {
        if (skill == null) return;
        if (skill.CD > 0)
        {
            if (!overlay.enabled) overlay.enabled = true;
            if (!cdText.enabled) cdText.enabled = true;

            overlay.fillAmount = skill.CD / skill.define.CD;
            cdText.text = ((int)Math.Ceiling(skill.CD)).ToString();
        }
        else
        {
            if (overlay.enabled) overlay.enabled = false;
            if (cdText.enabled) cdText.enabled = false;
        }
    }

    public void OnPositionSelected(Vector3 pos)
    {
        BattleManager.Instance.CurrentPosition = GameObjectTool.WorldToLogicN(pos);
        CastSkill();
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if(skill.define.CastTarget == TargetType.Position)
        {
            TargetSelector.ShowSelector(User.Instance.CurrentCharacter.position, skill.define.CastRange, skill.define.AOERange, OnPositionSelected);
            return;
        }
        CastSkill();
    }

    private void CastSkill()
    {
        SkillResult result = skill.CanCast(BattleManager.Instance.CurrentTarget);
        switch (result)
        {
            case SkillResult.InvalidTarget:
                MessageBox.Show("技能：" + skill.define.Name + "目标无效");
                return;
            case SkillResult.OutOfMp:
                MessageBox.Show("技能：" + skill.define.Name + "MP不足");
                return;
            case SkillResult.CoolDown:
                MessageBox.Show("技能：" + skill.define.Name + "正在冷却");
                return;
            case SkillResult.OutOfRange:
                MessageBox.Show("技能：" + skill.define.Name + "释放距离不足");
                return;
        }
        //释放技能
        BattleManager.Instance.CastSkill(this.skill);
    }

    public void SetSkill(Skill skill)
    {
        this.skill = skill;
        if (icon != null)
        {
            icon.overrideSprite = Resloader.Load<Sprite>(skill.define.Icon);
        }
    }
}
