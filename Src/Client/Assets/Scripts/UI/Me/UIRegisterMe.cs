using Services;
using SkillBridge.Message;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIRegisterMe : MonoBehaviour
{

    public InputField email;
    public InputField password;
    public InputField isPassword;

    private void Start()
    {
        //添加注册服务器回调事件
        UserService.Instance.OnRegister += ClinentOnRegister;
    }

    /// <summary>
    /// 注册服务器回调事件
    /// </summary>
    /// <param name="result"></param>
    /// <param name="msg"></param>
    private void ClinentOnRegister(Result result, string msg)
    {
        MessageBox.Show(String.Format("结果：{0}，信息：{1}", result, msg));
    }

    /// <summary>
    /// 注册按钮点击事件
    /// </summary>
    public void RegisterOnclick()
    {
        if (string.IsNullOrEmpty(email.text))
        {
            MessageBox.Show("请输入邮箱","信息",MessageBoxType.Information, "好的", "");
            return;
        }
        if (string.IsNullOrEmpty(password.text))
        {
            MessageBox.Show("请输入密码", "信息", MessageBoxType.Information, "好的", "");
            return;
        }
        if (string.IsNullOrEmpty(isPassword.text))
        {
            MessageBox.Show("请再次输入密码", "信息", MessageBoxType.Information, "好的", "");
            return;
        }
        if(password.text != isPassword.text)
        {
            MessageBox.Show("两次密码输入不一致请重新输入", "信息", MessageBoxType.Information, "好的", "");
            isPassword.text = string.Empty;
            return;
        }
        SoundManager.Instance.PlaySound(SoundDefine.SFX_UI_Click);
        //向服务器发送注册请求
        UserService.Instance.SendRegister(email.text, password.text);
    }
}
