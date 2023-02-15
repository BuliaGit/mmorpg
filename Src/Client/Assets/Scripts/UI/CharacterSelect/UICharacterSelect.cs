using Models;
using Services;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICharacterSelect : MonoBehaviour
{
    public Transform scrollTran;
    public GameObject uiCharInfo;
    public GameObject newCharInfo;

    /// <summary>
    /// 角色滚动列表
    /// </summary>
    private List<GameObject> uiChars = new List<GameObject>();
    GameObject newChar;
    private int selectCharacterIdx = -1;


    public void InitCharacterSelect()
    {
        if (uiChars.Count != 0)
        {
            foreach (var old in uiChars)
            {
                Destroy(old);
            }
            uiChars.Clear();
        }

        if (newChar != null)
        {
            Destroy(newChar);
        }

        for (int i = 0; i < User.Instance.Info.Player.Characters.Count; i++)
        {
            GameObject go = Instantiate(uiCharInfo, this.scrollTran);
            UICharInfo chrInfo = go.GetComponent<UICharInfo>();
            chrInfo.info = User.Instance.Info.Player.Characters[i];

            int idx = i;
            Button button = go.GetComponent<Button>();
            button.onClick.AddListener(() =>
            {
                //Debug.Log("idx:" + idx + " ,i:" + i);
                SelectClass(idx);   //不能用i,它不是赋值，如果参数是i,当点击时因为循环早就结束，此时的i只能是3，所以要事先用临时值记录
            });

            uiChars.Add(go);
            go.SetActive(true);
        }

        SelectClass(0); //默认显示模型

        newChar = Instantiate(newCharInfo, this.scrollTran);
        newChar.SetActive(true);
    }

    public void SelectClass(int index)
    {
        CharSelectManager.Instance.UICharacterView.characterModel.localEulerAngles = Vector3.zero;

        this.selectCharacterIdx = index;
        var cha = User.Instance.Info.Player.Characters[index];
        Debug.LogFormat("Select Char:[{0}]{1}[{2}]", cha.Id, cha.Name, cha.Class);

        CharSelectManager.Instance.UICharacterView.CurrectCharacter = (int)cha.Class - 1;   //显示模型

        for (int i = 0; i < User.Instance.Info.Player.Characters.Count; i++)
        {
            UICharInfo info = uiChars[i].GetComponent<UICharInfo>();
            info.Selected = index == i;
        }
    }



    public void OnClickPlay()
    {
        if (selectCharacterIdx >= 0)
        {
            //MessageBox.Show("进入游戏", "进入游戏", MessageBoxType.Confirm);
            UserService.Instance.SendGameEnter(selectCharacterIdx);
        }
    }

    public void BackToLogin()
    {
        SceneManager.Instance.LoadScene("Loading");
    }
}
