using Assets.Scripts.Battle;
using Entities;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Entities
{
    public interface IEntityController
    {
        Transform GetTransform();
        void PlayAnim(string name);
        void PlayEffect(EffectType type, string name, Creature target, float duration);
        void PlayEffect(EffectType type, string name, NVector3 position, float duration);
        void SetStandby(bool standby);
        void UpdateDirection();
    }
}
