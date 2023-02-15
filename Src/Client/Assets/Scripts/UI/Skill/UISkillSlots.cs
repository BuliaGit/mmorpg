using Models;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

public class UISkillSlots : MonoBehaviour
{
    public UISkillSlot[] slots;

    public void Start()
    {
    }

    public void RefreshUI()
    {
        if (User.Instance.CurrentCharacter == null) return;
        var skills = User.Instance.CurrentCharacter.skillMgr.Skills;
        int skillIdx = 0;
        foreach (var item in skills)
        {
            slots[skillIdx].SetSkill(item);
            skillIdx++;
        }
    }
}
