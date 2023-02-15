using Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.UI;

public class UITeamMe : MonoBehaviour 
{
    public Text titleTxt;
    public UITeamItemMe[] members;
    public ListViewMe list;

    public void Start()
    {
        if (User.Instance.TeamInfo == null)
        {
            gameObject.SetActive(false);
        }
        foreach (var item in members)
        {
            list.AddItem(item);
        }
    }

    void OnEnable()
    {
        UpdateTeamUI();
    }

    public void UpdateTeamUI()
    {
        if (User.Instance.TeamInfo == null) return;
        titleTxt.text = string.Format("我的队伍（{0}/5",User.Instance.TeamInfo.Members.Count);

        for (int i = 0; i < 5; i++)
        {
            if (i < User.Instance.TeamInfo.Members.Count)
            {
                members[i].SetTeamItemInfo(i, User.Instance.TeamInfo.Members[i], User.Instance.TeamInfo.Members[i].Id == User.Instance.TeamInfo.Leader);
                members[i].gameObject.SetActive(true);
            }
            else
            {
                members[i].gameObject.SetActive(false);
            }
        }
    }

    public void OnClickLeave()
    {
        MessageBox.Show("确定要离开队伍吗？", "退出队伍", MessageBoxType.Confirm, "确定离开", "取消").OnYes = () =>
        {
            TeamService.Instance.SendTeamLeaveRequest();
        };
    }

    public void ShowTeamUI(bool isShow)
    {
        gameObject.SetActive(isShow);
        if (isShow)
        {
            UpdateTeamUI();
        }
    }
}
