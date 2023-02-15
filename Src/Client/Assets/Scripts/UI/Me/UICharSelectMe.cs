using Models;
using Services;
using SkillBridge.Message;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICharSelectMe : MonoBehaviour
{
    public Transform selectRoleRoot;
    private List<NCharacterInfo> nCharacterInfos = new List<NCharacterInfo>();
    public Transform roleRoot;
    public ToggleGroup toggleGroup;
    private int selectCharIdx;
    public void Start()
    {
        nCharacterInfos = User.Instance.Info.Player.Characters;
        //通过玩家角色数初始化角色选择界面
        InitCharSelect();
    }

    /// <summary>
    /// 初始化角色选择界面
    /// </summary>
    private void InitCharSelect()
    {
        for (int i = 1; i <= nCharacterInfos.Count; i++)
        {
            GameObject uiCharacterGo = Resloader.Load<GameObject>("UI/UICharacter");
            GameObject go = Instantiate(uiCharacterGo, selectRoleRoot);
            go.name = "Character" + i;
            go.transform.Find("Text_RoleName").GetComponent<Text>().text = nCharacterInfos[i - 1].Name;
            go.transform.Find("Image_Role").GetComponent<Image>().sprite = Resloader.Load<Sprite>(string.Format("Sprite/{0}", nCharacterInfos[i - 1].Class.ToString()));
            go.transform.Find("Text_RoleLv").GetComponent<Text>().text = String.Format("{0}级", nCharacterInfos[i - 1].Level.ToString());
            go.GetComponent<Toggle>().group = toggleGroup;
            if (i == 1)
            {
                go.GetComponent<Toggle>().isOn = true;
            }
            go.GetComponent<Toggle>().onValueChanged.AddListener((bool isOn) => { OnToggleOnclick(go, isOn); });
            go.transform.Find("id").GetComponent<Text>().text = Convert.ToInt32(nCharacterInfos[i - 1].Class).ToString();
        }
        GameObject.FindGameObjectWithTag("Player").SetActive(false);
        if (nCharacterInfos.Count > 0)
        {
            roleRoot.Find(nCharacterInfos[0].Class.ToString()).gameObject.SetActive(true);
        }

    }

    /// <summary>
    /// toggle点击事件
    /// </summary>
    /// <param name="go"></param>
    /// <param name="isOn"></param>
    private void OnToggleOnclick(GameObject go, bool isOn)
    {
        if (isOn)
        {
            string roleName = go.transform.Find("Image_Role").GetComponent<Image>().sprite.name;
            GameObject.FindGameObjectWithTag("Player").SetActive(false);
            roleRoot.Find(roleName).gameObject.SetActive(true);
            selectCharIdx = int.Parse(go.name.Replace("Character", string.Empty))-1;
        }
    }

    /// <summary>
    /// 进入主城
    /// </summary>
    public void EnterMainCity()
    {
        UserService.Instance.SendGameEnter(selectCharIdx);
    }
}
