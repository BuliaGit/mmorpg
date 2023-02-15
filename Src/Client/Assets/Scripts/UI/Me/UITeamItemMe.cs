using SkillBridge.Message;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITeamItemMe : ListViewMe.ListViewItemMe 
{
    public Text lv;
    public Text memberName;
    public Image leaderImg;
    public Transform bgTF;
    public Image originImg;
    public Image selectImg;
    public Image classImg;

    public int index;
    public NCharacterInfo info;
    public override void OnSelect(bool val)
    {
        bgTF.GetComponent<Image>().overrideSprite = val ? selectImg.sprite : originImg.sprite;
    }

    public void SetTeamItemInfo(int index,NCharacterInfo item,bool isLeader)
    {
        this.index = index;
        info = item;
        lv.text = info.Level.ToString();
        memberName.text = info.Name;
        leaderImg.gameObject.SetActive(isLeader);
        classImg.overrideSprite = SpriteManagerMe.Instance.classIcons[(int)item.Class-1];
    }
}
