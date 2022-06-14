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
using UnboundLib.Networking;
using HarmonyLib;

namespace RootCards.MonoBehaviours
{
    internal class PotatoPass : DealtDamageEffect
    {
        public override void DealtDamage(Vector2 damage, bool selfDamage, Player damagedPlayer)
        {
            if (selfDamage) return;
            Player player = this.gameObject.GetComponent<Player>();
            if (player.data.view.IsMine && player.data.currentCards.Any(c => c.categories.Contains(RootCards.PotatoCategory)))
            {
                player.data.currentCards.Remove(player.data.currentCards.Find(c => c.categories.Contains(RootCards.PotatoCategory)));
                NetworkingManager.RPC(typeof(PotatoPass),nameof(PassPotato),damagedPlayer.playerID,player.playerID); 
            }
        }

        [UnboundRPC]
        public static void PassPotato(int playerID, int myID)
        {
            Player damagedPlayer = PlayerManager.instance.players.Find(p => p.playerID == playerID);
            damagedPlayer.gameObject.GetOrAddComponent<PotatoEffect>(); damagedPlayer.gameObject.GetOrAddComponent<PotatoPass>();
            Player player = PlayerManager.instance.players.Find(p => p.playerID == myID);
            if (!player.data.view.IsMine)
                player.data.currentCards.Remove(player.data.currentCards.Find(c => c.categories.Contains(RootCards.PotatoCategory)));
            CardBar cardbar = ModdingUtils.Utils.CardBarUtils.instance.PlayersCardBar(player);
            CardInfo card = null;
            for (int num = cardbar.transform.childCount - 1; num >= 0; num--)
            {
                if (((CardInfo)cardbar.transform.GetChild(num).gameObject.GetComponent<CardBarButton>().GetFieldValue("card")).categories.Contains(RootCards.PotatoCategory))
                {
                    card = (CardInfo)cardbar.transform.GetChild(num).gameObject.GetComponent<CardBarButton>().GetFieldValue("card");
                    Destroy(cardbar.transform.GetChild(num).gameObject);
                    break;
                }
            }
            damagedPlayer.data.currentCards.Add(card);
            ModdingUtils.Utils.Cards.SilentAddToCardBar(playerID, card);
            if (damagedPlayer.data.view.IsMine)
            {
                ModdingUtils.Utils.CardBarUtils.instance.PlayersCardBar(playerID).OnHover(card, Vector3.zero);
                ((GameObject)Traverse.Create(ModdingUtils.Utils.CardBarUtils.instance.PlayersCardBar(playerID)).Field("currentCard").GetValue()).gameObject.transform.localScale = Vector3.one * ModdingUtils.Utils.CardBarUtils.cardLocalScaleMult;

                RootCards.instance.ExecuteAfterSeconds(0.75f, () => ModdingUtils.Utils.CardBarUtils.instance.PlayersCardBar(playerID).StopHover());
            }
        }
    }
}
