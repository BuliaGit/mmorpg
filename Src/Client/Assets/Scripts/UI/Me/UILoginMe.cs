using Services;
using SkillBridge.Message;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILoginMe : MonoBehaviour {

	public InputField userName;
	public InputField passWord;

	private void Start()
	{
        //添加服务器登录回调事件
        UserService.Instance.OnLogin += ClientOnLogin;
	}

	/// <summary>
	/// 服务器登录回调事件
	/// </summary>
	/// <param name="result"></param>
	/// <param name="msg"></param>
	private void ClientOnLogin(Result result, string msg)
	{
		if(result == Result.Success)
		{
			SoundManager.Instance.PlayMusic(SoundDefine.Music_Select);
			SceneManager.Instance.LoadScene("CharSelect");
		}
		else
		{
			if(result == Result.Failed)
			{
                MessageBox.Show(String.Format("结果：{0}，信息：{1}", result, msg), "登录", MessageBoxType.Information);
            }
		}

	}

	/// <summary>
	/// 登录点击事件
	/// </summary>
	public void LoginOnClick()
	{
		SoundManager.Instance.PlaySound(SoundDefine.SFX_UI_Click);
		if (string.IsNullOrEmpty(userName.text))
		{
			MessageBox.Show("请输入账号");
			return;
		}
        if (string.IsNullOrEmpty(passWord.text))
        {
            MessageBox.Show("请输入密码");
            return;
        }
        //向服务器发送登录请求
        UserService.Instance.SendLogin(userName.text, passWord.text);
	}
}
