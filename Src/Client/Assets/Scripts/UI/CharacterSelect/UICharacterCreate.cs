using Services;
using SkillBridge.Message;
using UnityEngine;
using UnityEngine.UI;

public class UICharacterCreate : MonoBehaviour
{
    public Text descs;
    public InputField charName;
    public TabView TabView;

    /// <summary>
    /// 角色种类(枚举)
    /// </summary>
    CharacterClass charClass;


    void Start()
    {
        UserService.Instance.OnCharacterCreate = OnCharacterCreate;
    }


    /// <summary>
    /// 创建角色回调
    /// </summary>
    /// <param name="result"></param>
    /// <param name="message"></param>
    void OnCharacterCreate(Result result, string message)
    {
        if (result == Result.Success)
        {
            CharSelectManager.Instance.Init(); //返回选择界面
        }
        else
        {
            MessageBox.Show(message, "错误", MessageBoxType.Error);
        }
    }


    /// <summary>
    /// 显示角色描述
    /// </summary>
    /// <param name="charClass"></param>
    public void OnClickClass(int charClass)
    {
        CharSelectManager.Instance.UICharacterView.characterModel.localEulerAngles = Vector3.zero;

        this.charClass = (CharacterClass)charClass;

        CharSelectManager.Instance.UICharacterView.CurrectCharacter = charClass - 1; //显示模型

        TabView.SelectTab(charClass - 1);

        descs.text = DataManager.Instance.Characters[charClass].Description;
    }

    /// <summary>
    /// 点击创建角色
    /// </summary>
    public void OnClickCreate()
    {
        if (string.IsNullOrEmpty(this.charName.text))
        {
            MessageBox.Show("请输入角色名称");
            return;
        }

        UserService.Instance.SendCharacterCreate(this.charName.text, this.charClass);
    }
}
