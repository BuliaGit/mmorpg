using Assets.Scripts.Battle;
using Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class UIBuffIcons : MonoBehaviour
{
    Creature owner;
    public GameObject prefabBuff;
    Dictionary<int, GameObject> buffs = new Dictionary<int, GameObject>();

    void OnDestroy()
    { 
        Clear();
    }

    public void SetOwner(Creature owner)
    {
        if (owner != null && this.owner != owner)
        {
            Clear();
        }

        this.owner = owner;
        this.owner.OnBuffAdd += OnBuffAdd;
        this.owner.OnBuffRemove += OnBuffRemove;

        InitBuffs();
    }

    private void InitBuffs()
    {
        foreach (var item in owner.BuffMgr.Buffs)
        {
            OnBuffAdd(item.Value);
        }
    }

    private void OnBuffRemove(Buff buff)
    {
        GameObject go;
        if(buffs.TryGetValue(buff.buffId,out go))
        {
            buffs.Remove(buff.buffId);
            Destroy(go);
        }
    }

    private void OnBuffAdd(Buff buff)
    {
        GameObject go = Instantiate(prefabBuff, transform);
        go.name = buff.buffDefine.ID.ToString();
        UIBuffItem uIBuffItem = go.GetComponent<UIBuffItem>();
        uIBuffItem.SetItem(buff);
        go.SetActive(true);
        buffs[buff.buffId] = go;
    }

    private void Clear()
    {
        if(owner != null)
        {
            owner.OnBuffAdd -= OnBuffAdd;
            owner.OnBuffRemove -= OnBuffRemove;
        }
        foreach (var item in buffs)
        {
            Destroy(item.Value);
        }
        buffs.Clear();
    }
}
