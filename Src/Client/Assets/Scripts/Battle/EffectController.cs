using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Battle
{
    public class EffectController : MonoBehaviour
    {
        public float lifeTime = 1f;
        float time = 0;

        EffectType type;
        Transform target;

        Vector3 targetPos;
        Vector3 startPos;
        Vector3 offset;

        void OnEnable()
        {
            if (type != EffectType.Bullet)
            {
                StartCoroutine(Run());
            }
        }

        IEnumerator Run()
        {
            yield return new WaitForSeconds(lifeTime);
            gameObject.SetActive(false);
        }

        public void Init(EffectType type, Transform source, Transform target, Vector3 offset, float duration)
        {
            this.type = type;
            this.target = target;
            if (duration > 0)
                this.lifeTime = duration;
            this.time = 0;
            if (type == EffectType.Bullet)
            {
                this.startPos = transform.position;
                this.offset = offset;
                this.targetPos = this.target.position + offset;
            }
            else if (type == EffectType.Hit)
            {
                transform.position = target.position + offset;
            }
        }

        void Update()
        {
            if (type == EffectType.Bullet)
            {
                time += Time.deltaTime;
                if (target != null)
                {
                    targetPos = target.position + offset;
                }

                transform.LookAt(targetPos);


                if (Vector3.Distance(targetPos, transform.position) < 0.5f)
                {
                    Destroy(gameObject);
                    return;
                }

                if (lifeTime > 0 && time >= lifeTime)
                {
                    Destroy(gameObject);
                    return;
                }
                transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime / (lifeTime - time));
            }
        }
    }
}
