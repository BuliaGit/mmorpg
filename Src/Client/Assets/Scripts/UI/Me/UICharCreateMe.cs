using Models;
using Services;
using SkillBridge.Message;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICharCreateMe : MonoBehaviour {

	public Text warriorText;
	public Text wizardText;
	public Text archerText;
	public Text playerName;
	public Transform charItemRoot;
	private int selectCharIdx;

	public void Start()
	{
        warriorText.text = DataManager.Instance.Characters[1].Description;
		wizardText.text = DataManager.Instance.Characters[2].Description;
		archerText.text = DataManager.Instance.Characters[3].Description;
        
        //添加角色创建回调
        UserService.Instance.OnCharacterCreate += OnCharCreate;
    }

	/// <summary>
	/// 角色创建回调
	/// </summary>
	/// <param name="arg0"></param>
	/// <param name="arg1"></param>
	/// <exception cref="NotImplementedException"></exception>
	private void OnCharCreate(Result result, string msg)
	{
		if(result == Result.Success)
		{
			//向服务端发送进入游戏请求
			UserService.Instance.SendGameEnter(selectCharIdx);
		}
		else
		{
			playerName.text = String.Empty;
			MessageBox.Show(String.Format("结果：{0}，信息：{1}", result, msg));
		}
	}

	/// <summary>
	/// 开启冒险点击方法
	/// </summary>
	public void StartAdventureOnclick()
	{
		SoundManager.Instance.PlaySound(SoundDefine.SFX_UI_Click);
		selectCharIdx = charItemRoot.childCount;
        if (string.IsNullOrEmpty(playerName.text))
		{
			MessageBox.Show("玩家名字不能为空");
			return;
		}
        string characterClass = GameObject.FindGameObjectWithTag("Player").name;
		switch (characterClass)
		{
            //向服务端发送角色创建请求
            case "Warrior":
                UserService.Instance.SendCharacterCreate(playerName.text, CharacterClass.Warrior);
                break;
			case "Wizard":
                UserService.Instance.SendCharacterCreate(playerName.text, CharacterClass.Wizard);
                break;
			case "Archer":
                UserService.Instance.SendCharacterCreate(playerName.text, CharacterClass.Archer);
                break;
			default:
				break;
		}
		
		
	}
}
