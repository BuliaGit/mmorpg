using Assets.Scripts.Managers.Me;
using Entities;
using Managers;
using Models;
using Services;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMainMe : MonoSingleton<UIMainMe>
{
    public Text roleNameTxt;
    public Text roleLvTxt;
    public Transform roleSprite;
    public UITeamMe uiTeam;

    public UICreatureInfo targetUI;

    public UISkillSlots skillSlots;

    // Use this for initialization
    protected override void OnStart()
    {
        UpdateAvater();
        targetUI.gameObject.SetActive(false);
        BattleManager.Instance.OnTargetChanged += OnTargetChanged;
        User.Instance.OnCharacterInit += skillSlots.RefreshUI;
        skillSlots.RefreshUI();
    }



    public void UpdateAvater()
    {
        roleNameTxt.text = User.Instance.CurrentCharacterInfo.Name;
        roleLvTxt.text = (User.Instance.CurrentCharacterInfo.Level).ToString();
        string currentCharClass = User.Instance.CurrentCharacterInfo.Class.ToString();
        roleSprite.GetComponent<Image>().sprite = Resloader.Load<Sprite>(string.Format("Sprite/{0}", currentCharClass));
    }

    public void BagOnclick()
    {
        //更新背包信息并显示
        UIBagMe uIBagMe = UIManagerMe.Instance.Show<UIBagMe>();
        if (uIBagMe.isSoltsInit)
        {
            uIBagMe.ClearBag();
            StartCoroutine(uIBagMe.InitBags());
        }
    }

    public void EquipOnclick()
    {
        UIManagerMe.Instance.Show<UIEquip>();
    }

    public void QuestOnClick()
    {
        UIManagerMe.Instance.Show<UIQuestSystemMe>();
    }
    public void FriendOnClick()
    {
        UIManagerMe.Instance.Show<UIFriends>();
    }

    public void ShowUITeam(bool isShow)
    {
        uiTeam.ShowTeamUI(isShow);
    }

    public void OnClickGuild()
    {
        GuildManager.Instance.ShowGuild();
    }

    public void OnClickRide()
    {
        UIManagerMe.Instance.Show<UIRide>();
    }

    public void OnClickSetting()
    {
        UIManagerMe.Instance.Show<UISetting>();
    }

    public void OnClickSkill()
    {
        UIManagerMe.Instance.Show<UISkill>();
    }

    private void OnTargetChanged(Creature target)
    {
        if (target != null)
        {
            if (targetUI.isActiveAndEnabled)
            {
                targetUI.Target = target;
            }
            else
            {
                targetUI.gameObject.SetActive(true);
                targetUI.Target = target;
            }
        }
        else
        {
            targetUI.gameObject.SetActive(false);
        }
    }
}
