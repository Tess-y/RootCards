using HarmonyLib;
using Photon.Pun;
using System;
using System.Collections.Generic;
using System.Text;
using RootCards.Extensions;
using UnityEngine;

namespace RootCards.Patches
{

    [HarmonyPatch(typeof(DevConsole), "Send")]
    internal class DevConsolePatchSend
    {
        private static void Postfix(DevConsole __instance, string message)
        {
            RootCards.Debug(message);
            Player wisher = GetPlayerWithActorID(PhotonNetwork.LocalPlayer.ActorNumber, PlayerManager.instance.players);
            RootCards.Debug(wisher);
            if (wisher != null && wisher.data.stats.GetRootData().freeCards > 0)
            {
                CardInfo[] cards = CardChoice.instance.cards;
                int num = -1;
                float num2 = 0f;
                for (int i = 0; i < cards.Length; i++)
                {
                    string text = cards[i].GetComponent<CardInfo>().cardName.ToUpper();
                    text = text.Replace(" ", "");
                    if (text == "GENIE") continue;
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
                __instance.GetComponent<PhotonView>().RPC("RPCA_SendChat", RpcTarget.All, "[GRANT_WISH*****{{}}{{}}]" + num, PhotonNetwork.LocalPlayer.ActorNumber); //this is bad --Lilith
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

    [HarmonyPatch(typeof(DevConsole), "RPCA_SendChat")]
    internal class DevConsolePatchSendChat ///Litteraly ALL OF THIS needs ta be replaced with somethin more inteligent at somepoint --Lilith
    {
        [PunRPC]
        private static bool Prefix(DevConsole __instance, string message, int playerViewID)
        {
            try
            {
                if (message.Substring(0, "[GRANT_WISH*****{{}}{{}}]".Length) == "[GRANT_WISH*****{{}}{{}}]")
            {
                
                    Player wisher = DevConsolePatchSend.GetPlayerWithActorID(playerViewID, PlayerManager.instance.players);
                    int num = int.Parse(message.Substring("[GRANT_WISH*****{{}}{{}}]".Length));
                    CardInfo[] cards = CardChoice.instance.cards;

                    if (num != -1 && wisher.data.stats.GetRootData().freeCards > 0)
                    {
                        ModdingUtils.Utils.Cards.instance.AddCardToPlayer(wisher, cards[num], addToCardBar: true);
                        wisher.data.stats.GetRootData().freeCards -= 1;
                    }
                    return false;
                }
                else
                {
                    return true;
                }

            }
            catch (Exception e)
            {
                return true;
            }
        }
    }
}
