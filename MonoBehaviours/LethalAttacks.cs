using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace RootCards.MonoBehaviours
{
    internal class LethalAttacks : DealtDamageEffect
    {
        public override void DealtDamage(Vector2 damage, bool selfDamage, Player damagedPlayer = null)
        {
            damagedPlayer.data.healthHandler.DoDamage(new Vector2(9999999, 9999999), damagedPlayer.data.playerVel.position, Color.black, ignoreBlock: true);
        }
    }
}
