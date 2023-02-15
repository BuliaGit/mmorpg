using Common.Battle;
using Managers;
using Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISkill : UIWindowMe
{
    public GameObject itemPrefab;
    public ListViewMe skillList;
    public Text skillDes;

    public UISkillItem selectSkillItem;

    public void Start()
    {
        UpdateUIRide();
        skillList.OnItemSelect += OnItemSelect;
    }



    private void OnItemSelect(ListViewMe.ListViewItemMe item)
    {
        selectSkillItem = item as UISkillItem;
        skillDes.text = selectSkillItem.skill.define.Description;
    }

    public void UpdateUIRide()
    {
        ClearRideList();
        InitSkillList();
    }

    private void InitSkillList()
    {
        var Skills = User.Instance.CurrentCharacter.skillMgr.Skills;
        foreach (var skill in Skills)
        {
            if (skill.define.Type == SkillType.Skill)
            {
                GameObject itemGo = Instantiate(itemPrefab, skillList.transform);
                UISkillItem itemUI = itemGo.GetComponent<UISkillItem>();
                itemUI.SetSkillItemInfo(skill);
                skillList.AddItem(itemUI);
            }
        }
    }

    private void ClearRideList()
    {
        skillList.RemoveAll();
    }

    public void OnLevelUp()
    {
        if (selectSkillItem == null)
        {
            MessageBox.Show("请选择要升级的技能", "提示");
            return;
        }
    }
}
