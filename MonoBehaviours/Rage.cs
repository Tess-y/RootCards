using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace RootCards.MonoBehaviours
{
    internal class Rage : WasDealtDamageEffect
    {
        public override void WasDealtDamage(Vector2 damage, bool selfDamage)
        {
            if (selfDamage) return;
            this.gameObject.AddComponent<RageBuff>();
        }
    }
}
