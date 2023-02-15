using Services;
using SkillBridge.Message;
using UnityEngine;
using UnityEngine.UI;

public class UIRegister : MonoBehaviour {

    public InputField username;
    public InputField password;
    public InputField passwordConfirm;

    public GameObject uiLogin;

    // Use this for initialization
    void Start () {
        ResetText();
        UserService.Instance.OnRegister = OnRegister;
    }

    void OnEnable()
    {
        ResetText();
    }

    void ResetText()
    {
        username.text = "";
        password.text = "";
        passwordConfirm.text = "";
    }

    public void OnClickRegister()
    {
        if (string.IsNullOrEmpty(this.username.text))
        {
            MessageBox.Show("请输入账号");
            return;
        }
        if (string.IsNullOrEmpty(this.password.text))
        {
            MessageBox.Show("请输入密码");
            return;
        }
        if (string.IsNullOrEmpty(this.passwordConfirm.text))
        {
            MessageBox.Show("请输入确认密码");
            return;
        }
        if (this.password.text != this.passwordConfirm.text)
        {
            MessageBox.Show("两次输入的密码不一致");
            return;
        }

        UserService.Instance.SendRegister(this.username.text,this.password.text);
    }


    void OnRegister(Result result, string message)
    {
        if (result == Result.Success)
        {
            //登录成功，进入角色选择
            MessageBox.Show("注册成功,请登录", "提示", MessageBoxType.Information).OnYes = this.CloseRegister;
        }
        else
            MessageBox.Show(message, "错误", MessageBoxType.Error);
    }

    void CloseRegister()
    {
        this.gameObject.SetActive(false);
        uiLogin.SetActive(true);
    }
}
