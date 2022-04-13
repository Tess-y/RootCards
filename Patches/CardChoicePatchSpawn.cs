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
using Photon.Pun;
using System.Collections;

namespace RootCards.Patches
{
    [Serializable]
    [HarmonyPatch(typeof(CardChoice), "Spawn")]
    internal class CardChoicePatchSpawn
    {
        private static int nulls = 0;
        [HarmonyPriority(int.MinValue)] 
        private static bool Prefix(CardChoice __instance, ref GameObject objToSpawn, ref AdjustedCards __state, int ___pickrID, List<GameObject> ___spawnedCards, Transform[] ___children, ref GameObject __result, Vector3 pos, Quaternion rot)
        {
            __state = new AdjustedCards();
            if (__instance.IsPicking)
            {
                var player = GetPlayerWithID(___pickrID);
                
                CardInfo[] spawnedCards = ___spawnedCards.Select(obj => obj.GetComponent<CardInfo>().sourceCard).ToArray();
               /* int nulls = 0;
                foreach(CardInfo card in spawnedCards)
                {
                    if(card.gameObject.GetComponent<Cards.NullCard>() != null)
                    {
                        nulls++;
                    }
                }
                RootCards.Debug(nulls);*/
                if (player.data.stats.GetRootData().lockedCard != null)
                {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                    objToSpawn = player.data.stats.GetRootData().lockedCard.gameObject;
#pragma warning restore CS8602 // Dereference of a possibly null reference.

                    __state.adjusted = true;
#pragma warning disable CS8601 // Possible null reference assignment.
                    __state.newCard = player.data.stats.GetRootData().lockedCard;
#pragma warning restore CS8601 // Possible null reference assignment.
                }
                else if (spawnedCards.Length < player.data.stats.GetRootData().nulls)
                {
                    __state.adjusted = true;
                    __state.newCard = Cards.Null.NULLCARD;
                    __state.nulledCard = objToSpawn.GetComponent<CardInfo>();
                    objToSpawn = __state.newCard.gameObject;
                    objToSpawn.GetComponent<CardInfo>().cardName = __state.newCard.cardName;
                    __result = PhotonNetwork.Instantiate(objToSpawn.name, pos, rot, 0, new object[] { __state.nulledCard.cardName, player.playerID });
                    return false;
                }
            }
            return true;
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
        private static void Postfix(CardChoice __instance, ref GameObject __result, ref AdjustedCards __state)
        {
            if (__state.adjusted)
            {
                __result.GetComponent<CardInfo>().sourceCard = __state.newCard; 
                if(__state.nulledCard != null)
                {
                    __result.GetComponent<CardInfo>().cardName = __state.nulledCard.cardName;

                }
            }
        }

        private class AdjustedCards
        {
            public bool adjusted = false;
            public CardInfo newCard;
            public CardInfo nulledCard;
        }

        public static IEnumerator resetNull()
        {
            nulls = 0;
            yield break;
        }
    }
}
