using RootCards.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using UnboundLib;
using UnboundLib.Networking;
using UnboundLib.Utils;
using UnityEngine;

namespace RootCards.Cards.Util
{
    public class NullManager
    {
        internal static Dictionary<int, List<CardInfo>> NulledLibrary = new Dictionary<int, List<CardInfo>>();
        internal static Dictionary<int, List<CardInfo>> RemovedNulledLibrary = new Dictionary<int, List<CardInfo>>();

        public static IEnumerator ResetLibrary()
        {
            NulledLibrary = new Dictionary<int, List<CardInfo>>();
            RemovedNulledLibrary = new Dictionary<int, List<CardInfo>>();
            yield break;
        }

        public static IEnumerator CleanupRemovedNulls()
        { 
            RemovedNulledLibrary = new Dictionary<int, List<CardInfo>>();
            yield break;
        }

        public static IEnumerator CalculateNulls()
        {
            foreach (Player player in PlayerManager.instance.players) {
                CharacterStatModifiers characterstats = player.data.stats;
                if (characterstats.GetRootData().nulls > 0 || NulledLibrary[player.playerID].Count > 0)
                {
                    ModdingUtils.Extensions.CharacterStatModifiersExtension.GetAdditionalData(characterstats).blacklistedCategories.Remove(Null.NeedsNull);
                }
                else
                {
                    ModdingUtils.Extensions.CharacterStatModifiersExtension.GetAdditionalData(characterstats).blacklistedCategories.Add(Null.NeedsNull);
                }
            }
            yield break;
        }

        public static IEnumerator DoHandleReroll(int playerID)
        {
            RootCards.Debug(Photon.Pun.PhotonNetwork.IsMasterClient);
            RootCards.Debug(NulledLibrary.ContainsKey(playerID));
            RootCards.Debug(NulledLibrary[playerID].Count);
            if (!Photon.Pun.PhotonNetwork.IsMasterClient || !NulledLibrary.ContainsKey(playerID)) yield break;
            List<CardInfo> cardInfos = new List<CardInfo>();
            foreach(CardInfo cardInfo in NulledLibrary[playerID])
            {
                cardInfos.Add(ModdingUtils.Utils.Cards.instance.NORARITY_GetRandomCardWithCondition(PlayerManager.instance.players.Find(p => p.playerID == playerID), null, null, null, null, null, null, null,
                    (CardInfo c, Player _1, Gun _2, GunAmmo _3, CharacterData _4, HealthHandler _5, Gravity _6, Block _7, CharacterStatModifiers _8) => c.rarity == cardInfo.rarity));
            }
            NetworkingManager.RPC(typeof(NullManager), nameof(ClearNullsRPC), playerID);
            yield return new WaitForSecondsRealtime(2);

            RootCards.Debug($"{NulledLibrary[playerID].Count},{cardInfos.Count}");
            foreach (CardInfo card in cardInfos)
            {
                NetworkingManager.RPC(typeof(NullManager), nameof(RegisterNullRPC), playerID, card.name);
            }
            yield break;
        }

        public static int GetNullIndx(int playerID, int inx)
        {
            int nullcout = -1;
            List<CardInfo> currentCards = PlayerManager.instance.players.Find(p => p.playerID == playerID).data.currentCards;
            if(!(inx < currentCards.Count))return nullcout;
            for (int i = 0; i <= inx; i++)
            {
                if (currentCards[i] == Null.NULLCARD) nullcout++;
            }
            return nullcout;
        }

        private static int GetNullCount(int playerID)
        {

            int nullcout = 0;
            List<CardInfo> currentCards = PlayerManager.instance.players.Find(p => p.playerID == playerID).data.currentCards;
            for (int i = 0; i < currentCards.Count; i++)
            {
                if (currentCards[i] == Null.NULLCARD) nullcout++;
            }
            return nullcout;
        }

        public static void AssignNull(int playerID, CardInfo card)
        {
            int nullcout = GetNullCount(playerID);
            List<CardInfo> currentCards = PlayerManager.instance.players.Find(p => p.playerID == playerID).data.currentCards;
            for (int i = 0; i < currentCards.Count; i++)
            {
                if (currentCards[i] == Null.NULLCARD) nullcout--;
                if(nullcout == 0)
                {
                    PlayerManager.instance.players.Find(p => p.playerID == playerID).data.currentCards[i].GetComponent<NullCard>().NulledCard = card;
                    break;
                }
            }

        }

        [UnboundRPC]
        public static void RegisterNullRPC(int playerID, string cardName)
        {
            RegisterNull(playerID, ModdingUtils.Utils.Cards.instance.GetCardWithObjectName(cardName));
        }

        [UnboundRPC]
        public static void ClearNullsRPC(int playerID)
        {
            NulledLibrary[playerID].Clear();
        }

        public static void RegisterNull(int playerID, CardInfo? card)
        {
            RootCards.Debug($"Trying to register Null:{(card == null ? "unknown" :card.cardName)}");
            if (!NulledLibrary.ContainsKey(playerID)) NulledLibrary[playerID] = new List<CardInfo>();
            if (card == null){ HandleReassign(playerID); return; }
            NulledLibrary[playerID].Add(card);
            AssignNull(playerID,card);
        }
        public static void RemoveNull(int playerID, CardInfo? card)
        {
            RootCards.Debug($"Trying to remove Null:{(card == null ? "unknown" : card.cardName)}");
            if (!RemovedNulledLibrary.ContainsKey(playerID)) RemovedNulledLibrary[playerID] = new List<CardInfo>();
            if (card == null) {
                try
                {
                    RemovedNulledLibrary[playerID].Add(NulledLibrary[playerID][0]);
                    RootCards.Debug($"Removing Null:{NulledLibrary[playerID][0].cardName}");
                    NulledLibrary[playerID].RemoveAt(0);
                    RootCards.Debug(RemovedNulledLibrary[playerID].Count);
                }
                catch (Exception ex) { RootCards.Debug(ex); }
                return; }
            NulledLibrary[playerID].Remove(card);
            RemovedNulledLibrary[playerID].Add(card);
        }


        private static void HandleReassign(int playerID)
        {
            RootCards.Debug("Entering Handle Reassign");
            if (NulledLibrary[playerID].Count >= GetNullCount(playerID)) { RootCards.Debug("Enough cards exist, returning");  return; } 
            if(RemovedNulledLibrary.ContainsKey(playerID) && RemovedNulledLibrary[playerID].Count > 0)
            {
                RootCards.Debug("Removed null found using");
                RegisterNull(playerID, RemovedNulledLibrary[playerID][0]);

                AssignNull(playerID, RemovedNulledLibrary[playerID][0]);
                RemovedNulledLibrary[playerID].RemoveAt(0);

                return;
            }
            bool reassigned = false;
            PlayerManager.instance.players.ForEach(p =>
            {
                if (!reassigned && RemovedNulledLibrary.ContainsKey(p.playerID))
                {
                    if (RemovedNulledLibrary[p.playerID].Count > 0)
                    {
                        RootCards.Debug("Removed null from other player found found using");
                        reassigned = true;
                        RegisterNull(playerID, RemovedNulledLibrary[p.playerID][0]);
                        AssignNull(playerID, RemovedNulledLibrary[p.playerID][0]);
                        RemovedNulledLibrary[p.playerID].RemoveAt(0);
                    }
                }
            });
            if (!reassigned)
            {
                RootCards.Debug("Everything falled, exicuting fall back method");
                CardInfo[] allCards = ((ObservableCollection<CardInfo>)typeof(CardManager).GetField("activeCards", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null)).ToList().Concat((List<CardInfo>)typeof(CardManager).GetField("inactiveCards", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null)).ToArray();
                CardInfo randomCard = ModdingUtils.Utils.Cards.instance.DrawRandomCardWithCondition(allCards, PlayerManager.instance.players.Find(p => p.playerID == playerID), null, null, null, null, null, null, null,
                    (CardInfo _0, Player _1, Gun _2, GunAmmo _3, CharacterData _4, HealthHandler _5, Gravity _6, Block _7, CharacterStatModifiers _8) => true);
                if (PlayerManager.instance.players.Find(p => p.playerID == playerID).data.view.IsMine)
                    NetworkingManager.RPC(typeof(NullManager), nameof(RegisterNullRPC), playerID, randomCard.name);
            }
        }

        public static CardInfo GetNulledForPlayer(int playerID, int index, bool is_ablsolute = true)
        {
            CardInfo card = Null.NULLCARD;
            if(is_ablsolute) index = GetNullIndx(playerID, index);
            try
            {
                card = NulledLibrary[playerID][index];
            }
            catch { /*something whent wrong this shouldn't happen*/ RootCards.Debug($"Tried to find null {index} on player{playerID}, but failed for some reaon."); }
            return card;
        }
    }
}
