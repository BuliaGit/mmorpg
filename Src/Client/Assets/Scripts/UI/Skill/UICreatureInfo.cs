using Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICreatureInfo : MonoBehaviour
{

    public Text Name;
    public Slider HPBar;
    public Slider MPBar;
    public Text HPText;
    public Text MPText;

    public UIBuffIcons UIBuff;

    private Creature target;
    public Creature Target
    {
        get { return target; }
        set
        {
            target = value;
            UIBuff.SetOwner(value);
            UpdateUI();
        }
    }

    void Update()
    {
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (target == null) return;
        Name.text = String.Format("{0} Lv.{1}", target.Name, target.Info.Level);

        HPBar.maxValue = target.Attributes.MaxHP;
        HPBar.value = target.Attributes.HP;
        HPText.text = String.Format("{0}/{1}", target.Attributes.HP, target.Attributes.MaxHP);

        MPBar.maxValue = target.Attributes.MaxMP;
        MPBar.value = target.Attributes.MP;
        MPText.text = String.Format("{0}/{1}", target.Attributes.MP, target.Attributes.MaxMP);
    }
}
