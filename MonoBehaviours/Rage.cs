using System;
using System.Collections.Generic;
using System.Text;
using UnboundLib;
using UnityEngine;

namespace RootCards.MonoBehaviours
{
    internal class Rage : WasDealtDamageEffect
    {
        public float cooldown = 0;
        public override void WasDealtDamage(Vector2 damage, bool selfDamage)
        {
            if (selfDamage || cooldown > 0) return;
            this.gameObject.GetOrAddComponent<RageBuff>().damage += 0.1f;
            this.gameObject.GetOrAddComponent<RageBuff>().speed += 0.2f;
            cooldown = 1;
        }

        private void Update()
        {
            if (cooldown > 0) cooldown -= TimeHandler.deltaTime;
        }
    }
}
