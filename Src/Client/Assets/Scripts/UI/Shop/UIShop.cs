using Common.Data;
using Managers;
using Models;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIShop : UIWindow
{
    public Text title;
    public Text money;

    public GameObject shopItem;
    public Transform[] pages;
    public Scrollbar[] scrollbars;
    public TabView TabView;

    private List<GameObject> items = new List<GameObject>();
    private ShopDefine shop;

    public void SetShop(ShopDefine shop)
    {
        this.shop = shop;
        this.title.text = shop.Name;
        this.money.text = User.Instance.CurrentCharacterInfo.Gold.ToString();

        Init();
    }

    void Init()
    {
        for (int i = 0; i < items.Count; i++)
        {
            Destroy(items[i]);
        }

        for (int i = 1; i < TabView.tabButtons.Length; i++)
        {
            TabView.tabButtons[i].gameObject.SetActive(false);
        }

        int count = 0;
        int page = 0;
        foreach (var kv in DataManager.Instance.ShopItems[shop.ID])
        {
            if (kv.Value.Status > 0)
            {
                GameObject go = Instantiate(shopItem, pages[page]);
                UIShopItem ui = go.GetComponent<UIShopItem>();
                ui.SetShopItem(kv.Key, kv.Value, this);

                //分页
                count++;
                if (count >= 10)
                {
                    count = 0;
                    page++;
                    TabView.tabButtons[page].gameObject.SetActive(true);
                }

                items.Add(go);
            }
        }

        for (int i = 0; i < scrollbars.Length; i++)
        {
            scrollbars[i].value = 1;
        }

        TabView.SelectTab(0);
    }


    private UIShopItem selectShopItem;
    public void SelectShopItem(UIShopItem item)
    {
        if (selectShopItem != null)
        {
            selectShopItem.Selected = false;
        }
        selectShopItem = item;
    }

  

    public void OnClickBuy()
    {
        //todo:完善正在购买中提示
        if (ShopManager.Instance.isClick)
        {
            if (selectShopItem == null)
            {
                MessageBox.Show("请选择要购买的道具", "购买提示").OnYes = () =>
                {
                    ShopManager.Instance.isClick = true;
                };
                ShopManager.Instance.isClick = false;
                return;
            }

            ShopManager.Instance.BuyItem(this.shop.ID, this.selectShopItem.ShopItemID);
            ShopManager.Instance.isClick = false;
        }
    }
}


