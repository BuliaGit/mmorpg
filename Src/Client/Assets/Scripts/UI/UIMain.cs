using Models;
using Services;
using UnityEngine.UI;

public class UIMain : MonoSingleton<UIMain>
{
    public Text txtName;
    public Text txtLevel;
    public Image[] imgHeadPic;
    public Slider sliderHp;
    public Slider sliderMp;
    public Slider sliderExp;


    // Use this for initialization
    protected override void OnStart()
    {
        UpdateHeadInfo();
    }

    /// <summary>
    /// 更新头像信息
    /// </summary>
    void UpdateHeadInfo()
    {
        txtName.text = User.Instance.CurrentCharacterInfo.Name;
        txtLevel.text = User.Instance.CurrentCharacterInfo.Level.ToString();
        for (int i = 0; i < 3; i++)
        {
            this.imgHeadPic[i].gameObject.SetActive(i == (int)User.Instance.CurrentCharacterInfo.Class - 1);
        }
        //todo:Hp、Mp、Exp等

    }

    #region 按钮
    public void BackToCharSelect()
    {
        SceneManager.Instance.LoadScene("CharSelect");
        UserService.Instance.SendGameLeave();
    }

    public void OnClickBag()
    {
        UIManager.Instance.Show<UIBag>();
    }


    public void OnClickEquip()
    {
        UIManager.Instance.Show<UIEquip>();
    }

    public void OnClickQuest()
    {
        UIManager.Instance.Show<UIQuestSystem>();
    }

    public void OnClickFriend()
    {
        UIManager.Instance.Show<UIFriends>();
    }
    #endregion
}
