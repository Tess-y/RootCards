using ModdingUtils.MonoBehaviours;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace RootCards.MonoBehaviours
{
    internal class PotatoEffect : ReversibleEffect
    {
        float time = 0.5f;

        public override void OnUpdate()
        {
            SetLivesToEffect(int.MaxValue);
            characterStatModifiers.movementSpeed = 0.5f;
            base.OnUpdate();
            time -= Time.deltaTime;
            if (time < 0)
            {
                player.data.healthHandler.GetComponentInChildren<PlayerSkinHandler>().BlinkColor(Color.cyan);
                time = 0.5f;
            }
        }
    }
}
