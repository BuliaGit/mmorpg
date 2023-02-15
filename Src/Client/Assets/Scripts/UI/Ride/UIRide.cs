using Managers;
using Models;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIRide : UIWindowMe
{
    public GameObject itemPrefab;
    public ListViewMe rideList;
    public Transform itemRoot;
    public Text rideDes;

    public UIRideItem selectRideItem;

    public void Start()
    {
        UpdateUIRide();
        rideList.OnItemSelect += OnItemSelect;
    }



    private void OnItemSelect(ListViewMe.ListViewItemMe item)
    {
        selectRideItem = item as UIRideItem;
        rideDes.text = selectRideItem.itemInfo.Define.Description;
    }

    public void UpdateUIRide()
    {
        ClearRideList();
        InitRideList();
    }

    private void InitRideList()
    {
        foreach (var item in ItemManager.Instance.Items)
        {
            if (item.Value.Define.Type == SkillBridge.Message.ItemType.Ride && (item.Value.Define.LimitClass == SkillBridge.Message.CharacterClass.None || item.Value.Define.LimitClass == User.Instance.CurrentCharacterInfo.Class))
            {
                GameObject itemGo = Instantiate(itemPrefab, itemRoot);
                UIRideItem itemUI = itemGo.GetComponent<UIRideItem>();
                itemUI.SetRideItemInfo(item.Value);
                rideList.AddItem(itemUI);
            }
        }
    }

    private void ClearRideList()
    {
        rideList.RemoveAll();
    }

    public void OnRide()
    {
        if (selectRideItem == null)
        {
            MessageBox.Show("请选择要召唤的坐骑", "提示");
            return;
        }
        User.Instance.Ride(selectRideItem.itemInfo.Id);
    }
}
