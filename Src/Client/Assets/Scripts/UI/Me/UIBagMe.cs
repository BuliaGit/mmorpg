using Managers;
using Models;
using Services;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBagMe : UIWindowMe
{

    public Transform[] Pages;
    public Text goldTxt;
    public GameObject bagItem;

    List<Image> solts;
    public bool isSoltsInit = false;

    private void Start()
    {
        if (solts == null)
        {
            solts = new List<Image>();
            for (int i = 0; i < Pages.Length; i++)
            {
                solts.AddRange(Pages[i].GetComponentsInChildren<Image>(true));
            }
            isSoltsInit = true;
        }
        StartCoroutine(InitBags());
        SetMoney();

    }

    public IEnumerator InitBags()
    {
        for (int i = 0; i < BagManager.Instance.items.Length; i++)
        {
            var item = BagManager.Instance.items[i];
            if (item.ItemId > 0)
            {
                GameObject itemGO = Instantiate(bagItem, solts[i].transform);
                var itemUI = itemGO.GetComponent<UIIconItemMe>();
                var def = ItemManager.Instance.Items[item.ItemId].Define;
                itemUI.SetIconItem(def.Icon, item.Count.ToString());
            }
        }
        for (int i = BagManager.Instance.items.Length; i < solts.Count; i++)
        {
            solts[i].color = Color.gray;
        }
        yield return null;
    }

    public void ClearBag()
    {
        for (int i = 0; i < solts.Count; i++)
        {
            if (solts[i].transform.childCount > 0)
            {
                Destroy(solts[i].transform.GetChild(0).gameObject);
            }
        }
    }

    public void ResetBag()
    {
        BagManager.Instance.Reset();
        ClearBag();
        StartCoroutine(InitBags());
    }

    private void SetMoney()
    {
        goldTxt.text = User.Instance.CurrentCharacterInfo.Gold.ToString();
    }
}
