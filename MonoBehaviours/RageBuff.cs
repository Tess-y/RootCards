using ModdingUtils.MonoBehaviours;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace RootCards.MonoBehaviours
{
    internal class RageBuff : ReversibleEffect
    {
        public float damage = 1f;
        public float speed = 1f;
        public override void OnUpdate()
        {
            if (gunStatModifier.damage_mult != damage)
            {
                this.ClearModifiers();
                characterStatModifiers.movementSpeed = Mathf.Clamp(speed,1,10);
                gunStatModifier.damage_mult = Mathf.Clamp(damage,1,5);
                this.ApplyModifiers();
            }
        }
    }
}
