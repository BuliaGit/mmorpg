using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Battle
{
    class FXManager : MonoSingleton<FXManager>
    {
        public GameObject[] prefabs;

        private Dictionary<string, GameObject> Effects = new Dictionary<string, GameObject>();

        protected override void OnStart()
        {
            for (int i = 0; i < prefabs.Length; i++)
            {
                prefabs[i].SetActive(false);
                Effects[prefabs[i].name] = prefabs[i];
            }
        }

        EffectController CreateEffect(string name,Vector3 pos)
        {
            GameObject prefab;
            if(Effects.TryGetValue(name,out prefab))
            {
                GameObject go = Instantiate(prefab, FXManager.Instance.transform, true);
                go.transform.position = pos;
                return go.GetComponent<EffectController>();
            }
            return null;
        }


        internal void PlayEffect(EffectType type, string name, Transform target, Vector3 pos, float duration)
        {
            EffectController effect = FXManager.Instance.CreateEffect(name, pos);
            if(effect == null)
            {
                Debug.LogFormat("Effect:[{0}] not existed",name);
            }
            effect.Init(type, transform, target, pos, duration);
            effect.gameObject.SetActive(true);
        }
    }
}
