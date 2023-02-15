using Assets.Scripts.Managers.Me;
using Assets.Scripts.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIGuildApplyList : UIWindowMe 
{
    public GameObject applyItem;
    public Transform itemRoot;
    public ListViewMe applyList;

    public void Start()
    {
        GuildService.Instance.OnGuildUpdate += UpdateApplyList;
        GuildService.Instance.SendGuildListRequest();
        UpdateApplyList();
    }

    public void OnDestroy()
    {
        GuildService.Instance.OnGuildUpdate -= UpdateApplyList;
    }


    private void UpdateApplyList()
    {
        ClearApplyList();
        InitApplyList();
    }

    private void InitApplyList()
    {
        foreach (var item in GuildManager.Instance.guildInfo.Applies)
        {
            GameObject itemGo = Instantiate(applyItem, itemRoot.transform);
            var itemUI = itemGo.GetComponent<UIGuildApplyItem>();
            itemUI.SetApplyItemUIInfo(item);
            applyList.AddItem(itemUI);
        }
    }

    private void ClearApplyList()
    {
        applyList.RemoveAll();
    }
}
