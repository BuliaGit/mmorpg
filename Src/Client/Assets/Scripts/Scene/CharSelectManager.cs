using Models;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CharSelectManager : MonoSingleton<CharSelectManager> {

    public UICharacterCreate UICharacterCreate;
    public UICharacterSelect UICharacterSelect;
    public UICharacterView UICharacterView;

    // Use this for initialization
    protected override void OnStart() {
        Init();
        RegTouchEvts();
    }


    #region 初始化
    public void Init()
    {
        if (User.Instance.Info.Player.Characters.Count != 0)    //角色创建回调获取的
        {
            InitCharacterSelect();
        }
        else
        {
            InitCharacterCreate();
        }
    }

    public void InitCharacterCreate()   
    {
        UICharacterCreate.gameObject.SetActive(true);
        UICharacterSelect.gameObject.SetActive(false);

        UICharacterCreate.OnClickClass(1);   //默认显示战士模型
    }

    public void InitCharacterSelect()
    {
        UICharacterCreate.gameObject.SetActive(false);
        UICharacterSelect.gameObject.SetActive(true);

        UICharacterSelect.InitCharacterSelect();
    }
    #endregion

    #region 旋转模型
    public RawImage imgChar;
    private Vector2 startPos;
    private float startRoate = 0;

    private void RegTouchEvts()
    {
        PEListener listener = GameUtil.Instance.GetOrAddComponect<PEListener>(imgChar.gameObject);
        listener.onClickDown = (PointerEventData evt) =>
        {
            startPos = evt.position;
            startRoate = UICharacterView.characterModel.localEulerAngles.y;
        };

        listener.onDrag = (PointerEventData evt) =>
        {
            float roate = -(evt.position.x - startPos.x) * 0.4f;
            UICharacterView.characterModel.localEulerAngles = new Vector3(0, startRoate + roate, 0);
        };
    }
    #endregion
}
