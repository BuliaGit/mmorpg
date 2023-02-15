using Managers;
using Models;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIEquipItem : UIWindow, IPointerClickHandler
{
    public Image icon;
    public Text title;
    public Text level;
    public Text limitClass;
    public Text LimitCategory;

    public Image background;
    public Sprite NormalBg;
    public Sprite selectedBg;

    private bool selected;
    public bool Selected
    {
        get { return selected; }
        set
        {
            selected = value;
            this.background.overrideSprite = selected ? selectedBg : NormalBg;
        }
    }

    public int EquipIndex { get; set; }
    private UIEquip owner;
    private Item item;
    private bool isEquiped;

    public void SetEquipItem(int idx, Item item, UIEquip owner, bool equiped)
    {
        this.owner = owner;
        this.EquipIndex = idx;
        this.item = item;
        this.isEquiped = equiped;
        if (this.title != null) this.title.text = this.item.Define.Name;
        if (this.level != null) this.level.text = item.Define.Level.ToString();
        if (this.limitClass != null) this.limitClass.text = item.Define.LimitClass.ToString();
        if (this.LimitCategory != null) this.LimitCategory.text = item.Define.Category;
        if (this.icon != null) this.icon.overrideSprite = Resloader.Load<Sprite>(this.item.Define.Icon);
    }


    #region 穿戴装备
    public void OnPointerClick(PointerEventData eventData)
    {
        if (this.isEquiped)
        {
            UnEquip();
        }
        else
        {
            if (this.selected)
            {
                DoEquip();
                this.Selected = false;

            }
            else
                this.Selected = true;
        }
        this.owner.SelectEquipItem(this);
    }

    /// <summary>
    /// 戴上装备
    /// </summary>
    void DoEquip()
    {
        var msg = MessageBox.Show(string.Format("要装备[{0}]吗？", this.item.Define.Name), "确认", MessageBoxType.Confirm);

        msg.OnYes = () =>
        {
            var oldEquip = EquipManager.Instance.GetEquip(item.EquipInfo.Slot);
            if (oldEquip != null)
            {
                var newmsg = MessageBox.Show(string.Format("要替换掉[{0}]吗？", oldEquip.Define.Name), "确认", MessageBoxType.Confirm);

                newmsg.OnYes = () =>
                {
                    this.owner.DoEquip(this.item);
                };
            }
            else
            {
                this.owner.DoEquip(this.item);
            }
        };
    }

    /// <summary>
    /// 卸下装备
    /// </summary>
    void UnEquip()
    {
        var msg = MessageBox.Show(string.Format("要取下装备[{0}]吗？", this.item.Define.Name), "确认", MessageBoxType.Confirm);

        msg.OnYes = () =>
        {
            this.owner.UnEquip(this.item);
        };
    }
    #endregion
}

