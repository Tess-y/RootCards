using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;
using UnboundLib;
using UnboundLib.Utils;
using System.Reflection;
using RootCards.Extensions;
using UnityEngine;
using System.Linq;

namespace RootCards.Patches
{
    [Serializable]
    [HarmonyPatch(typeof(CardChoice), "Spawn")]
    internal class CardChoicePatchSpawn
    {
        [HarmonyPriority(int.MinValue)] 
        private static void Prefix(CardChoice __instance, ref GameObject objToSpawn, ref AdjustedCards __state, int ___pickrID, List<GameObject> ___spawnedCards, Transform[] ___children)
        {
            __state = new AdjustedCards();
            if (__instance.IsPicking)
            {
                var player = GetPlayerWithID(___pickrID);

                CardInfo[] spawnedCards = ___spawnedCards.Select(obj => obj.GetComponent<CardInfo>().sourceCard).ToArray();
                int nulls = 0;
                foreach(CardInfo card in spawnedCards)
                {
                    if(card.cardName == "NULL")
                    {
                        nulls++;
                    }
                }
                if(nulls < player.data.stats.GetRootData().nulls)
                {
                    objToSpawn = Cards.Null.NULLCARD.gameObject;

                    __state.adjusted = true;
                    __state.newCard = Cards.Null.NULLCARD;
                }
                if(player.data.stats.GetRootData().lockedCard != null)
                {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                    objToSpawn = player.data.stats.GetRootData().lockedCard.gameObject;
#pragma warning restore CS8602 // Dereference of a possibly null reference.

                    __state.adjusted = true;
#pragma warning disable CS8601 // Possible null reference assignment.
                    __state.newCard = player.data.stats.GetRootData().lockedCard;
#pragma warning restore CS8601 // Possible null reference assignment.
                }
            }
        }
        internal static Player GetPlayerWithID(int playerID)
        {
            for (int i = 0; i < PlayerManager.instance.players.Count; i++)
            {
                if (PlayerManager.instance.players[i].playerID == playerID)
                {
                    return PlayerManager.instance.players[i];
                }
            }
            return null;
        }

        [HarmonyPriority(Priority.First)]
        static void Postfix(CardChoice __instance, GameObject __result, AdjustedCards __state)
        {
            if (__state.adjusted)
            {
                RootCards.instance.ExecuteAfterFrames(2, () => { __result.GetComponent<CardInfo>().sourceCard = __state.newCard; });
            }
        }

        private class AdjustedCards
        {
            public bool adjusted = false;
            public CardInfo newCard;
        }
    }
}
