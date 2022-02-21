using System;
using System.Collections.Generic;
using System.Text;
using ModdingUtils.Extensions;
using System.Collections.ObjectModel;
using UnboundLib.Utils;
using UnityEngine;
using RootCards.Cards;
using System.Reflection;
using System.Linq;
using UnboundLib;

namespace RootCards.MonoBehaviours
{
    internal class PotatoPass : DealtDamageEffect
    {
        public override void DealtDamage(Vector2 damage, bool selfDamage, Player damagedPlayer)
        {
            damagedPlayer.gameObject.GetOrAddComponent<PotatoEffect>();
            damagedPlayer.gameObject.GetOrAddComponent<PotatoPass>();
            base.gameObject.GetComponent<PotatoEffect>().Destroy();
            Destroy(this);
        }
    }
}
