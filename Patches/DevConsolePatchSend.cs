using HarmonyLib;
using Photon.Pun;
using System;
using System.Collections.Generic;
using System.Text;
using RootCards.Extensions;
using UnityEngine;
using RootCards.Cards;

namespace RootCards.Patches
{
    /** /
    [HarmonyPatch(typeof(DevConsole), "Send")]
    internal class DevConsolePatchSend
    {
        private static void Postfix(DevConsole __instance, string message)
        {
            RootCards.Debug(message);
            Player wisher = GetPlayerWithActorID(PhotonNetwork.LocalPlayer.ActorNumber, PlayerManager.instance.players);
            RootCards.Debug(wisher);
            if (wisher != null && wisher.data.currentCards.Contains(ModdingUtils.Utils.Cards.instance.GetCardWithName("Genie")))
            {
                CardInfo[] cards = CardChoice.instance.cards;
                int num = findIdFromName(message);
                if (num != -1)
                {
                    string cardsToAdd = "-1";
                    switch (cards[num].rarity)
                    {
                        case CardInfo.Rarity.Common:
                            cardsToAdd = new System.Random().Next(10) == 0? "-2," + num+","+num : "-3," + num;
                            break;
                        case CardInfo.Rarity.Uncommon:
                            cardsToAdd = "";
                            if (new System.Random().Next(10) == 990) { cardsToAdd += "-7"; wisher.data.stats.GetRootData().lockedCard = cards[num]; } else { cardsToAdd += -4; } cardsToAdd += num.ToString();
                            break;
                        case CardInfo.Rarity.Rare:
                            cardsToAdd = new System.Random().Next(10) == 990 ? "-5," + num : "-6," + num;
                            break;
                        default: break; 
                    }
                    __instance.GetComponent<PhotonView>().RPC("RPCA_SendChat", RpcTarget.All, "[GRANT_WISH*****{{}}{{}}]" + cardsToAdd, PhotonNetwork.LocalPlayer.ActorNumber); //this is bad --Lilith
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
        internal static int findIdFromName(string name) {
            CardInfo[] cards = CardChoice.instance.cards;
            int num = -1;
            float num2 = 0f;
            for (int i = 0; i < cards.Length; i++)
            {
                string text = cards[i].GetComponent<CardInfo>().cardName.ToUpper();
                text = text.Replace(" ", "");
                if (text == "GENIE") continue;
                string text2 = name.ToUpper();
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
            return num;
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
                    List<int> nums = new List<int>();
                    string[] cardsToAdd = FuckingSplitAGodDamnStringWhyTheFuckIsStringDotSplitNotDefinedProperly(message.Substring("[GRANT_WISH*****{{}}{{}}]".Length));
                    foreach (string card in cardsToAdd)
                    {
                        nums.Add(int.Parse(card));
                    }
                    CardInfo[] cards = CardChoice.instance.cards;
                    if (wisher.data.currentCards.Contains(ModdingUtils.Utils.Cards.instance.GetCardWithName("Genie")))
                    {
                        ModdingUtils.Utils.Cards.instance.RemoveCardFromPlayer(wisher, ModdingUtils.Utils.Cards.instance.GetCardWithName("Genie"), ModdingUtils.Utils.Cards.SelectionType.Oldest);
                        foreach (int num in nums)
                        {
                            if(num < -1)
                            {
                                switch (num)
                                {
                                    case -2:
                                        ModdingUtils.Utils.Cards.instance.AddCardToPlayer(wisher, ModdingUtils.Utils.Cards.instance.GetCardWithName("Genie: Smiles"), addToCardBar: true);
                                        break;
                                    case -3:
                                        ModdingUtils.Utils.Cards.instance.AddCardToPlayer(wisher, ModdingUtils.Utils.Cards.instance.GetCardWithName("Genie: Granted"), addToCardBar: true);
                                        break;
                                    case -4:
                                        ModdingUtils.Utils.Cards.instance.AddCardToPlayer(wisher, ModdingUtils.Utils.Cards.instance.GetCardWithName("Genie: Fee"), addToCardBar: true);
                                        break;
                                    case -7:
                                        ModdingUtils.Utils.Cards.instance.AddCardToPlayer(wisher, ModdingUtils.Utils.Cards.instance.GetCardWithName("Genie: Eternity"), addToCardBar: true);
                                        break;
                                    case -5:
                                        ModdingUtils.Utils.Cards.instance.AddCardToPlayer(wisher, ModdingUtils.Utils.Cards.instance.GetCardWithName("Genie: Greed"), addToCardBar: true);
                                        break;
                                    case -6:
                                        ModdingUtils.Utils.Cards.instance.AddCardToPlayer(wisher, ModdingUtils.Utils.Cards.instance.GetCardWithName("Genie: Death"), addToCardBar: true);
                                        break;
                                    default:
                                        break;

                                }
                            }
                            else if (num != -1)
                            {
                                ModdingUtils.Utils.Cards.instance.AddCardToPlayer(wisher, cards[num], addToCardBar: true);
                            }
                        }
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
                RootCards.Debug(e);
                return true;
            }
        }
        private static string[] FuckingSplitAGodDamnStringWhyTheFuckIsStringDotSplitNotDefinedProperly(string str)
        {
            List<string> result = new List<string>();
            string temp = "";
            foreach (char c in str)
            {
                if (c != ',')
                {
                    temp += c;
                }
                else
                {
                    result.Add(temp);
                    RootCards.Debug(temp);
                    temp = "";
                }
            }
            result.Add(temp);
            return result.ToArray();
        }
    }/**/
}
