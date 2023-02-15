//using Common.Battle;
using Common.Battle;
using Managers;
using Models;
using SkillBridge.Message;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 装备窗口
/// </summary>
public class UIEquip : UIWindow
{
    public Text title;
    public Text money;

    public Text RoleTxt;

    public GameObject itemPrefab;
    public GameObject itemEquipedPrefab;

    public Transform itemListRoot;

    public List<Transform> slots;
    public Text hp;
    public Slider hpbar;
    public Text mp;
    public Slider mpbar;
    public Text[] attrs;

    public RawImage imgChar;

    private void Start()
    {
        //RegTouchEvts();
        ReFreshUI();
        EquipManager.Instance.OnEquipChanged += ReFreshUI;

        //if (EquipManager.Instance.charCamTrans == null)
        //{
        //    EquipManager.Instance.charCamTrans = GameObject.FindGameObjectWithTag("CharShowCam").transform;
        //    EquipManager.Instance.charCamTrans.gameObject.SetActive(false);
        //}

        //EquipManager.Instance.SetCharCam();
    }

    private void OnDestroy()
    {
        //EquipManager.Instance.SetPlayerRoate();
        EquipManager.Instance.OnEquipChanged -= ReFreshUI;

        //EquipManager.Instance.CloseCharCam();
    }

    private void ReFreshUI()
    {
        ClearAllEquipList();
        InitAllEquipItems();
        ClearEquipList();
        InitEquipedItem();
        this.money.text = User.Instance.CurrentCharacterInfo.Gold.ToString();

        InitAttributes();
    }

    #region 初始化
    private void InitAttributes()
    {
        var charattr = User.Instance.CurrentCharacter.Attributes;
        RoleTxt.text = string.Format("{0}  LV{1}", User.Instance.CurrentCharacterInfo.Name,User.Instance.CurrentCharacterInfo.Level);
        this.hp.text = string.Format("{0}/{1}", charattr.HP, charattr.MaxHP);
        this.mp.text = string.Format("{0}/{1}", charattr.MP, charattr.MaxMP);
        this.hpbar.maxValue = charattr.MaxHP;
        this.hpbar.value = charattr.HP;
        this.mpbar.maxValue = charattr.MaxMP;
        this.mpbar.value = charattr.MP;
        for (int i = (int)AttributeType.STR; i < (int)AttributeType.MAX; i++)
        {
            if (i == (int)AttributeType.CRI)
                this.attrs[i - 2].text = string.Format("{0:f2}%", charattr.Final.Data[i] * 100);
            else
                this.attrs[i - 2].text = ((int)charattr.Final.Data[i]).ToString();
        }
    }

    /// <summary>
    /// 初始化所有装备列表（左边面板）
    /// </summary> 
    public void InitAllEquipItems()
    {
        foreach (var kv in ItemManager.Instance.Items)
        {
            if (kv.Value.Define.Type == ItemType.Equip && kv.Value.Define.LimitClass == User.Instance.CurrentCharacterInfo.Class)
            {
                //已经装备的就不显示
                if (EquipManager.Instance.Contains(kv.Key))
                    continue; 

                GameObject go = Instantiate(itemPrefab, itemListRoot);
                UIEquipItem ui = go.GetComponent<UIEquipItem>();
                ui.SetEquipItem(kv.Key, kv.Value, this, false);
            }
        }
    }

    void ClearAllEquipList()
    {
        foreach (var item in itemListRoot.GetComponentsInChildren<UIEquipItem>())
        {
            Destroy(item.gameObject);
        }
    }

    void ClearEquipList()
    {
        foreach (var item in slots)
        {
            if (item.childCount > 0)
                Destroy(item.GetChild(0).gameObject);
        }
    }

    /// <summary>
    /// 初始化已经装备的列表（中间面板）
    /// </summary>
    public void InitEquipedItem()
    {
        for (int i = 0; i < (int)EquipSlot.SlotMax; i++)
        {
            var item = EquipManager.Instance.Equips[i];
            if (item != null)
            {
                GameObject go = Instantiate(itemEquipedPrefab, slots[i]);
                UIEquipItem ui = go.GetComponent<UIEquipItem>();

                ui.SetEquipItem(i, item, this, true);
            }
        }
    }
    #endregion

    public void DoEquip(Item item)
    {
        EquipManager.Instance.EquipItem(item);
    }

    public void UnEquip(Item item)
    {
        EquipManager.Instance.UnEquipItem(item);
    }


    private UIEquipItem selectEquipItem;
    public void SelectEquipItem(UIEquipItem item)
    {
        if (selectEquipItem != null)
        {
            selectEquipItem.Selected = false;
        }
        selectEquipItem = item;
    }

    private Vector2 startPos;
    private void RegTouchEvts()
    {
        PEListener listener = GameUtil.Instance.GetOrAddComponect<PEListener>(imgChar.gameObject);
        listener.onClickDown = (PointerEventData evt) =>
        {
            startPos = evt.position;
            EquipManager.Instance.SetStartRoate();
        };

        listener.onDrag = (PointerEventData evt) =>
        {
            float roate = -(evt.position.x - startPos.x) * 0.4f;
            EquipManager.Instance.SetPlayerRoate(roate);
        };
    }
}

