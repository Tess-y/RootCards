using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;
using ModdingUtils.Utils;
using RootCards.Cards.Util;
using ClassesManagerReborn;

namespace RootCards.Patches
{
    [Serializable]
    [HarmonyPatch(typeof(ModdingUtils.Utils.Cards), "PlayerIsAllowedCard")]
    public class CardsPatchPlayerIsAllowedCard
    {
        public static void Postfix(ref bool __result, Player player, CardInfo card)
        {
            if (__result && NullManager.NulledLibrary.ContainsKey(player.playerID))
            {
                if (!card.allowMultiple)
                {
                    __result = !NullManager.NulledLibrary[player.playerID].Exists(c => c.cardName == card.cardName);
                }
                if(__result && ClassesRegistry.Get(card) != null)
                {
                    List<CardInfo> cards = player.data.currentCards.FindAll(c => c != Cards.Null.NULLCARD);
                    cards.AddRange(NullManager.NulledLibrary[player.playerID]);
                    __result = ClassesRegistry.Get(card).SimulatedPlayerIsAllowedCard(player.playerID, cards);
                }
            }
        }
    }
}
