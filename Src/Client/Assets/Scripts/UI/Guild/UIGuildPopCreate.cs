using Assets.Scripts.Services;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGuildPopCreate : UIWindowMe 
{
    public InputField inputGuildName;
    public InputField inputGuildAim;

    public void Start()
    {
        GuildService.Instance.OnGuildCreateResult = OnGuildCreated;
    }

    public void OnDestroy()
    {
        GuildService.Instance.OnGuildCreateResult = null;
    }

    public override void OnYesClick()
    {
        if (string.IsNullOrEmpty(inputGuildName.text))
        {
            MessageBox.Show("请输入公会名称","错误",MessageBoxType.Error);
            return;
        }
        if (inputGuildName.text.Length < 2 || inputGuildName.text.Length > 5)
        {
            MessageBox.Show("公会名称为2-5个字符", "错误",MessageBoxType.Error);
            return;
        }
        if (string.IsNullOrEmpty(inputGuildAim.text))
        {
            MessageBox.Show("请输入公会宗旨", "错误", MessageBoxType.Error);
            return;
        }
        if (inputGuildAim.text.Length < 3 || inputGuildAim.text.Length > 50)
        {
            MessageBox.Show("公会宗旨为3-50个字符", "错误", MessageBoxType.Error);
            return;
        }
        GuildService.Instance.SendGuildCreate(inputGuildName.text, inputGuildAim.text);
    }

    public void OnGuildCreated(bool result)
    {
        if (result)
        {
            Close(WindowResult.Yes);
        }
    }
}
