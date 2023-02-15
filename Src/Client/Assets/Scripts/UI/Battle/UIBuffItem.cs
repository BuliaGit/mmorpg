using Assets.Scripts.Battle;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBuffItem : MonoBehaviour
{
    public Image icon;
    public Image overlay;
    Buff buff;

    void Update()
    {
        if (buff == null) return;
        if (buff.buffTime > 0)
        {
            if (!overlay.enabled) overlay.enabled = true;

            overlay.fillAmount = buff.buffTime / buff.buffDefine.Duration;
        }
        else
        {
            if (overlay.enabled) overlay.enabled = false;
        }
    }

    public void SetItem(Buff buff)
    {
        this.buff = buff;
        if (icon != null)
        {
            icon.overrideSprite = Resloader.Load<Sprite>(this.buff.buffDefine.Icon);
            icon.SetAllDirty();
        }
    }
}
