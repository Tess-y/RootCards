using System;
using System.Collections.Generic;
using System.Text;

namespace RootCards.Cards.Utill
{
    public class NullManager
    {
        private static Dictionary<int, List<CardInfo>> NulledLibrary = new Dictionary<int, List<CardInfo>>();
        private static Dictionary<int, List<CardInfo>> RemovedNulledLibrary = new Dictionary<int, List<CardInfo>>();


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

        public static void RegisterNull(int playerID, CardInfo? card) 
        {
            if (!NulledLibrary.ContainsKey(playerID)) NulledLibrary[playerID] = new List<CardInfo>();
            if (card == null){ HandleReassign(playerID); return; }
            NulledLibrary[playerID].Add(card);
        }
        public static void RemoveNull(int playerID, CardInfo? card)
        {
            if (!RemovedNulledLibrary.ContainsKey(playerID)) RemovedNulledLibrary[playerID] = new List<CardInfo>();
            if (card == null) return;
            NulledLibrary[playerID].Remove(card);
            RemovedNulledLibrary[playerID].Add(card);
        }


        private static void HandleReassign(int playerID)
        {
            if (NulledLibrary[playerID].Count <= GetNullCount(playerID)) return;
            if(RemovedNulledLibrary.ContainsKey(playerID) && RemovedNulledLibrary[playerID].Count > 0)
            {
                RegisterNull(playerID, RemovedNulledLibrary[playerID][0]);
                RemovedNulledLibrary[playerID].RemoveAt(0);
            }
            bool reassigned = false;
            PlayerManager.instance.players.ForEach(p =>
            {
                if (!reassigned && RemovedNulledLibrary.ContainsKey(p.playerID))
                {
                    if (RemovedNulledLibrary[p.playerID].Count > 0)
                    {
                        reassigned = true;
                        RegisterNull(playerID, RemovedNulledLibrary[p.playerID][0]);
                        RemovedNulledLibrary[p.playerID].RemoveAt(0);
                    }
                }
            });
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
