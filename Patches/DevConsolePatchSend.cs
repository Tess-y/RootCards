using HarmonyLib;
using Photon.Pun;
using System;
using System.Collections.Generic;
using System.Text;
using RootCards.Extensions;
using UnityEngine;

namespace RootCards.Patches
{

    [HarmonyPatch(typeof(DevConsole), "RPCA_SendChat")]
    internal class DevConsolePatchSend
    {
        private static void Postfix(string message, int playerViewID)
        {
            RootCards.Debug(message + " " + playerViewID);
            Player controller = GetPlayerWithActorID(PhotonNetwork.LocalPlayer.ActorNumber, PlayerManager.instance.players);
            RootCards.Debug(controller);
            if (controller != null && controller.data.stats.GetRootData().freeCards > 0)
            {
                CardInfo[] cards = CardChoice.instance.cards;
                int num = -1;
                float num2 = 0f;
                for (int i = 0; i < cards.Length; i++)
                {
                    string text = cards[i].GetComponent<CardInfo>().cardName.ToUpper();
                    text = text.Replace(" ", "");
                    string text2 = message.ToUpper();
                    text2 = text2.Replace(" ", "");
                    float num3 = 0f;
                    for (int j = 0; j < text2.Length; j++)
                    {
                        if (text.Length > j && text2[j] == text[j])
                        {
                            num3 += 1f / (float)text2.Length;
                        }
                    }

                    num3 -= (float)Mathf.Abs(text2.Length - text.Length) * 0.001f;
                    if (num3 > 0.1f && num3 > num2)
                    {
                        num2 = num3;
                        num = i;
                    }
                }

                if (num != -1)
                {
                    ModdingUtils.Utils.Cards.instance.AddCardToPlayer(controller, cards[num], addToCardBar: true);
                    controller.data.stats.GetRootData().freeCards -= 1;
                }
            }
        }
        internal static Player GetPlayerWithActorID(int actorID, List<Player> players)
        {
            for (int i = 0; i < players.Count; i++)
            {
                if (players[i].data.view.OwnerActorNr == actorID)
                {
                    return players[i];
                }
            }

            return null;
        }
    }
}
