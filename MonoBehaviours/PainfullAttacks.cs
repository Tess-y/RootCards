using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace RootCards.MonoBehaviours
{
    internal class PainfullAttacks : DealtDamageEffect
    {
        public override void DealtDamage(Vector2 damage, bool selfDamage, Player damagedPlayer)
        {
            if (selfDamage || damagedPlayer == null) return;
            this.gameObject.GetComponent<Player>().data.healthHandler.DoDamage(damage, damagedPlayer.data.playerVel.position, Color.magenta, damagingPlayer: this.gameObject.GetComponent<Player>(), ignoreBlock: true);
        }
    }
}
