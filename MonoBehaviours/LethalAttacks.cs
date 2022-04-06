using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace RootCards.MonoBehaviours
{
    internal class LethalAttacks : DealtDamageEffect
    {
        public override void DealtDamage(Vector2 damage, bool selfDamage, Player damagedPlayer)
        {
            damagedPlayer.data.health = -100000;
        }
    }
}
 