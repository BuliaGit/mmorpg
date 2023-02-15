using Assets.Scripts.Battle;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityEffectManager : MonoBehaviour
{
    public Transform Root;

    public Dictionary<string, GameObject> Effects = new Dictionary<string, GameObject>();

    public Transform[] Props;

    void Start()
    {
        Effects.Clear();

        if (Root != null && Root.childCount > 0)
        {
            for (int i = 0; i < Root.childCount; i++)
            {
                Effects[Root.GetChild(i).name] = Root.GetChild(i).gameObject;
            }
        }

        if (Props != null && Props.Length > 0)
        {
            for (int i = 0; i < Props.Length; i++)
            {
                Effects[Props[i].name] = Props[i].gameObject;
            }
        }
    }


    public void PlayEffect(string name)
    {
        Debug.LogFormat("PlayEffect:{0}:{1}", this.name, name);
        if (Effects.ContainsKey(name))
        {
            Effects[name].SetActive(true);
        }
    }

    internal void PlayEffect(EffectType type, string name, Transform target, Vector3 pos, float duration)
    {
        if (type == EffectType.Bullet)
        {
            EffectController effect = InstantiateEffect(name);
            effect.Init(type, this.transform, target, pos, duration);
            effect.gameObject.SetActive(true);
        }
        else
        {
            PlayEffect(name);
        }
    }

    private EffectController InstantiateEffect(string name)
    {
        GameObject prefab;
        if (Effects.TryGetValue(name, out prefab))
        {
            GameObject go = Instantiate(prefab, GameObjectManager.Instance.transform, true);
            go.transform.position = prefab.transform.position;
            go.transform.rotation = prefab.transform.rotation;
            return go.GetComponent<EffectController>();
        }
        return null;
    }
}
