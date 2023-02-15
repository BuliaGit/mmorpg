using UnityEngine;
using UnityEngine.UI;

public class UICharInfo : MonoBehaviour
{
    public SkillBridge.Message.NCharacterInfo info;

    public Text charClass;
    public Text charName;
    public Image[] charIcon;

    public Image imgHightlight;
    public bool  Selected
    {
        get { return imgHightlight.IsActive(); }
        set 
        {
            imgHightlight.gameObject.SetActive(value);
        }
    }

    // Use this for initialization
    void Start()
    {
        if (info != null)
        {
            this.charClass.text = this.info.Level + "级 " + this.info.Class.ToString();
            this.charName.text = this.info.Name;
            for (int i = 0; i < 3; i++)
            {
                this.charIcon[i].gameObject.SetActive(i == (int)info.Class - 1);
            }
        }
    }
}
