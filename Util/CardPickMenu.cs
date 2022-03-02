using CardChoiceSpawnUniqueCardPatch.CustomCategories;
using Photon.Pun;
using RootCards.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using TMPro;
using UnboundLib;
using UnboundLib.Networking;
using UnboundLib.Utils;
using UnboundLib.Utils.UI;
using UnityEngine;
using UnityEngine.UI;

namespace RootCards.Util
{
    internal class CardPickMenu
    {

        private static TextMeshProUGUI text;
        internal static GameObject textCanvas = null;
        // method to pick from the entire deck using any given rarity
        internal static IEnumerator PickCard(Player player)
        {
            // if the player is not the local player then just skip
            if (!player.data.view.IsMine)
            {
                yield break;
            }

            int currentPicks = 0;

            int cardsToPick = player.data.stats.GetRootData().freeCards;
            string rarityString = "";

            // set up button actions
            var actions = ToggleCardsMenuHandler.cardObjs.Values.ToArray();
            for (var i = 0; i < actions.Length; i++)
            {
                var i1 = i;
                actions[i] = () =>
                {
                    RootCards.Debug(ToggleCardsMenuHandler.cardObjs.ElementAt(i1).Key.name);
                    // each action checks if the player is allowed the card, and if so assigns it using ModdingUtils
                    if (PhotonNetwork.OfflineMode || player.data.view.IsMine)
                    {
                        if (ToggleCardsMenuHandler.cardObjs.ElementAt(i1).Key.name == "Genie")
                        {
                            cardsToPick = 0;
                            return;
                        }
                        // player is allowed card, increase the number of picks
                        currentPicks++;

                       

                            List<CardInfo> cards = new List<CardInfo>();
                        int chance = 0;
                        switch (CardManager.cards[ToggleCardsMenuHandler.cardObjs.ElementAt(i1).Key.name].cardInfo.rarity)
                        {
                            case CardInfo.Rarity.Common:
                                chance = new System.Random().Next(10);
                                if(chance == 0)
                                {
                                    ReplaceGenie(player, ModdingUtils.Utils.Cards.instance.GetCardWithName("Genie: Smiles"));
                                    cards.Add(CardManager.cards[ToggleCardsMenuHandler.cardObjs.ElementAt(i1).Key.name].cardInfo);
                                }
                                else
                                {
                                    ReplaceGenie(player, ModdingUtils.Utils.Cards.instance.GetCardWithName("Genie: Granted"));
                                }
                                break;
                            case CardInfo.Rarity.Uncommon:
                                chance = new System.Random().Next(10);
                                if (chance == 0)
                                {
                                    ReplaceGenie(player, ModdingUtils.Utils.Cards.instance.GetCardWithName("Genie: Eternity"));
                                    player.data.stats.GetRootData().lockedCard = CardManager.cards[ToggleCardsMenuHandler.cardObjs.ElementAt(i1).Key.name].cardInfo;
                                }
                                else
                                {
                                    if (PhotonNetwork.OfflineMode)
                                    {
                                        ReplaceGenie(player, ModdingUtils.Utils.Cards.instance.GetCardWithName("Genie: Fee"));
                                    }
                                    else
                                    {
                                        NetworkingManager.RPC(typeof(CardPickMenu), nameof(RPCA_LockCard), new object[] { player.playerID, ToggleCardsMenuHandler.cardObjs.ElementAt(i1).Key.name });
                                    }
                                }
                                break;
                            case CardInfo.Rarity.Rare:
                                chance = new System.Random().Next(10);
                                if (chance == 0)
                                {
                                    if (PhotonNetwork.OfflineMode)
                                    {
                                        CardInfo[] pCards = player.data.currentCards.FindAll((c) => { return !c.categories.Contains(CustomCardCategories.instance.CardCategory("GenieCard")); }).ToArray();
                                        ModdingUtils.Utils.Cards.instance.RemoveCardsFromPlayer(player, pCards, editCardBar: false);
                                    }
                                    else
                                    {
                                        NetworkingManager.RPC(typeof(CardPickMenu), nameof(RPCA_RemoveAllCards), new object[] { player.playerID });
                                    }
                                    ReplaceGenie(player, ModdingUtils.Utils.Cards.instance.GetCardWithName("Genie: Greed"));
                                }
                                else
                                {
                                    ReplaceGenie(player, ModdingUtils.Utils.Cards.instance.GetCardWithName("Genie: Death"));
                                }
                                break;
                        }

                        cards.Add(CardManager.cards[ToggleCardsMenuHandler.cardObjs.ElementAt(i1).Key.name].cardInfo);

                        // assign offline
                        if (PhotonNetwork.OfflineMode)
                        {
                            foreach (CardInfo card in cards)
                            {
                                ModdingUtils.Utils.Cards.instance.AddCardToPlayer(player, card, false, "", 2f, 2f, addToCardBar: true);
                            }
                        }
                        // assign via RPC
                        else
                        {
                            foreach (CardInfo card in cards)
                            {
                                NetworkingManager.RPC(typeof(CardPickMenu), nameof(RPCA_AddCardToPlayer), new object[] { player.playerID, card.cardName });
                                new WaitForSecondsRealtime(0.05f);
                            }
                        }
                        // sync the current number of picks
                        //NetworkingManager.RPC(typeof(PreGamePickBanHandler), nameof(RPCA_UpdateCardCount), new object[] { currentPicks });
                    }
                };
            }

            // create the pick text canvas if it doesn't already exist
            if (textCanvas == null)
            {
                CreateText();
            }
            // if the client is the player thats picking, show the card choice menu with disabled cards greyed out
            if (player.data.view.IsMine)
            {
                ToggleCardsMenuHandler.Open(true, true, actions, (
                    (List<CardInfo>)typeof(CardManager).GetField("inactiveCards", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null)).Select(card => CardManager.cards.First(kv => kv.Value.cardInfo == card).Key).ToList().ToArray());
            }
            textCanvas.SetActive(true);

            // wait until all the picks are done
            while (currentPicks < cardsToPick)
            {
                // tell them how many of what rarity they have left to choose
                // make sure the menu stays open
                ToggleCardsMenuHandler.Open(true, true, actions, (
                    (List<CardInfo>)typeof(CardManager).GetField("inactiveCards", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null)).Select(card => CardManager.cards.First(kv => kv.Value.cardInfo == card).Key).ToList().ToArray());
                text.text = "PICK " + (cardsToPick - currentPicks).ToString() + " " + rarityString + " CARD" + ((cardsToPick - currentPicks != 1) ? "S" : "") + "\n Select Genie to save remaining wishes till next round.";

                yield return null;
            }
            // hide the text once everything is done
            textCanvas.SetActive(false);
            // close the menu
            ToggleCardsMenuHandler.Close();

            yield break;
        }

        private static void CreateText()
        {
            textCanvas = new GameObject("TextCanvas", typeof(Canvas));
            textCanvas.transform.SetParent(Unbound.Instance.canvas.transform);
            GameObject textBackground = new GameObject("TextBackground", typeof(Image));
            textBackground.transform.SetParent(textCanvas.transform);
            textBackground.GetComponent<Image>().color = new Color(0f, 0f, 0f, 0f);
            //textBackground.GetComponent<Image>().rectTransform.anchorMin = new Vector2(-2, 0f);
            //textBackground.GetComponent<Image>().rectTransform.anchorMax = new Vector2(3, 0f);
            GameObject textObj = new GameObject("Text", typeof(TextMeshProUGUI));
            textObj.transform.SetParent(textBackground.transform);

            CardPickMenu.text = textObj.GetComponent<TextMeshProUGUI>();
            CardPickMenu.text.text = "";
            CardPickMenu.text.fontSize = 45f;
            CardPickMenu.text.color = Color.magenta;
            CardPickMenu.text.outlineColor = Color.black;
            textCanvas.transform.position = new Vector2((float)Screen.width / 2f, (float)Screen.height - 50f);
            CardPickMenu.text.enableWordWrapping = false;
            CardPickMenu.text.overflowMode = TextOverflowModes.Overflow;
            CardPickMenu.text.alignment = TextAlignmentOptions.Center;
            textCanvas.SetActive(false);
        }



        [UnboundRPC]
        private static void RPCA_LockCard(int playerID, string cardName)
        {
            if (!PhotonNetwork.OfflineMode && !PhotonNetwork.IsMasterClient)
            {
                return;
            }
            Player player = (Player)PlayerManager.instance.InvokeMethod("GetPlayerWithID", playerID);
            CardInfo card = ModdingUtils.Utils.Cards.instance.GetCardWithName(cardName);
            player.data.stats.GetRootData().lockedCard = card;
        }

        [UnboundRPC]
        private static void RPCA_AddCardToPlayer(int playerID, string cardName)
        {
            if (!PhotonNetwork.OfflineMode && !PhotonNetwork.IsMasterClient)
            {
                return;
            }
            Player player = (Player)PlayerManager.instance.InvokeMethod("GetPlayerWithID", playerID);
            CardInfo card = ModdingUtils.Utils.Cards.instance.GetCardWithName(cardName);
            ModdingUtils.Utils.Cards.instance.AddCardToPlayer(player, card,false,"",2f,2f, addToCardBar: true);
        }

        [UnboundRPC]
        public static void RPCA_RemoveAllCards(int playerID)
        {
            if (!PhotonNetwork.OfflineMode && !PhotonNetwork.IsMasterClient)
            {
                return;
            }
            Player player = (Player)PlayerManager.instance.InvokeMethod("GetPlayerWithID", playerID);
            CardInfo[] cards = player.data.currentCards.FindAll((c) => { return !c.categories.Contains(CustomCardCategories.instance.CardCategory("GenieCard")); }).ToArray();
            ModdingUtils.Utils.Cards.instance.RemoveCardsFromPlayer(player,cards,editCardBar: false);
        }

        [UnboundRPC]
        public static void RPCA_ReplaceGenie(int playerID, string cardName)
        {
            if (!PhotonNetwork.OfflineMode && !PhotonNetwork.IsMasterClient)
            {
                return;
            }
            Player player = (Player)PlayerManager.instance.InvokeMethod("GetPlayerWithID", playerID);
            CardInfo card = ModdingUtils.Utils.Cards.instance.GetCardWithName(cardName);
            //ModdingUtils.Utils.Cards.instance.ReplaceCard(player, ModdingUtils.Utils.Cards.instance.GetCardWithName("Genie"), ModdingUtils.Utils.Cards.instance.GetCardWithName(cardName), "GE", 2f, 2f, selectType: ModdingUtils.Utils.Cards.SelectionType.Oldest, editCardBar: true);
            try { ModdingUtils.Utils.Cards.instance.RemoveCardFromPlayer(player, ModdingUtils.Utils.Cards.instance.GetCardWithName("Genie"), ModdingUtils.Utils.Cards.SelectionType.Oldest, false); }catch (Exception e) { RootCards.Debug(e); }
            ModdingUtils.Utils.Cards.instance.AddCardToPlayer(player, card, true, "ge", 2f, 2f, true);
        }

        public static void ReplaceGenie(Player player, CardInfo card)
        {
            if (PhotonNetwork.OfflineMode)
            {
                //ModdingUtils.Utils.Cards.instance.ReplaceCard(player, ModdingUtils.Utils.Cards.instance.GetCardWithName("Genie"), card, "GE", 2f, 2f, selectType: ModdingUtils.Utils.Cards.SelectionType.Oldest, editCardBar: true);
                try { ModdingUtils.Utils.Cards.instance.RemoveCardFromPlayer(player, ModdingUtils.Utils.Cards.instance.GetCardWithName("Genie"), ModdingUtils.Utils.Cards.SelectionType.Oldest, false); } catch (Exception e) { RootCards.Debug(e); }
                ModdingUtils.Utils.Cards.instance.AddCardToPlayer(player, card, true, "ge", 2f, 2f, true);
            }
            else
            {
                NetworkingManager.RPC(typeof(CardPickMenu), nameof(RPCA_ReplaceGenie), new object[] { player.playerID, card.name });
            }
        }
    }
}
