using Common.Data;
using Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIRideItem : ListViewMe.ListViewItemMe
{
    public Image rideIcon;
    public Text rideName;
    public Text level;


    public Image backgr;
    public Sprite originImg;
    public Sprite selectImg;

    public Item itemInfo;
    public override void OnSelect(bool val)
    {
        backgr.overrideSprite = val ? selectImg : originImg;
    }

    public void SetRideItemInfo(Item info)
    {
        itemInfo = info;
        if (itemInfo == null) return;
        rideIcon.overrideSprite = Resloader.Load<Sprite>(itemInfo.Define.Icon);
        rideName.text = itemInfo.Define.Name;
        level.text = string.Format("Lv.{0}", itemInfo.Define.Level);

    }
}
