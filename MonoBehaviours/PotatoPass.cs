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
            if (player.data.view.IsMine && player.data.currentCards.Contains(Cards.FrozenPotato.cardInfo))
            {
                NetworkingManager.RPC(typeof(PotatoPass),nameof(PassPotato),damagedPlayer.playerID,player.playerID);
            }
        }

        [UnboundRPC]
        public static void PassPotato(int playerID, int myID)
        {
            Player damagedPlayer = PlayerManager.instance.players.Find(p => p.playerID == playerID);
            Player player = PlayerManager.instance.players.Find(p => p.playerID == myID);
            player.data.currentCards.Remove(Cards.FrozenPotato.cardInfo);
            CardBar cardbar = ModdingUtils.Utils.CardBarUtils.instance.PlayersCardBar(player);
            for (int num = cardbar.transform.childCount - 1; num >= 0; num--)
            {
                if ((CardInfo)cardbar.transform.GetChild(num).gameObject.GetComponent<CardBarButton>().GetFieldValue("card") == Cards.FrozenPotato.cardInfo)
                {
                    Destroy(cardbar.transform.GetChild(num).gameObject);
                    break;
                }
            }
            damagedPlayer.data.currentCards.Add(Cards.FrozenPotato.cardInfo);
            ModdingUtils.Utils.Cards.SilentAddToCardBar(playerID, Cards.FrozenPotato.cardInfo);
            if (damagedPlayer.data.view.IsMine)
            {
                ModdingUtils.Utils.CardBarUtils.instance.PlayersCardBar(playerID).OnHover(Cards.FrozenPotato.cardInfo, Vector3.zero);
                ((GameObject)Traverse.Create(ModdingUtils.Utils.CardBarUtils.instance.PlayersCardBar(playerID)).Field("currentCard").GetValue()).gameObject.transform.localScale = Vector3.one * ModdingUtils.Utils.CardBarUtils.cardLocalScaleMult;

                RootCards.instance.ExecuteAfterSeconds(0.75f, () => ModdingUtils.Utils.CardBarUtils.instance.PlayersCardBar(playerID).StopHover());
            }
        }
    }
}
