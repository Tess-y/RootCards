﻿using HarmonyLib;
using RootCards.Cards;
using RootCards.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnboundLib;
using UnboundLib.Networking;
using UnityEngine;

namespace RootCards.Patches
{

    [Serializable]
    [HarmonyPatch(typeof(CardChoice), "IDoEndPick")]
    internal class CardChoicePatchIDoEndPick
    {
        public static void Prefix(CardChoice __instance, ref List<GameObject> ___spawnedCards, ref GameObject pickedCard, ref float ___speed)
        {
            if (pickedCard == null || pickedCard.GetComponent<CardInfo>().cardName.ToLower() != "Distill Acquisition".ToLower()) return;
            for (int i = 0; i < ___spawnedCards.Count; i++)
            {
                if ((bool)___spawnedCards[i])
                {
                    if (___spawnedCards[i].gameObject != pickedCard)
                    {
                        __instance.StartCoroutine(GrabCard(___spawnedCards[i], ___speed, __instance));
                    }
                }
            }
            NetworkingManager.RPC(typeof(CardChoicePatchIDoEndPick), nameof(GiveNulls), __instance.pickrID, (___spawnedCards.Count + 1) / 2);
            ___spawnedCards.Clear();
            ___spawnedCards.Add(pickedCard);
        }

        private static IEnumerator GrabCard(GameObject card, float speed, CardChoice __instance)
        {
            card.GetComponentInChildren<ApplyCardStats>().Pick(__instance.pickrID, forcePick: false, (PickerType)__instance.GetFieldValue("pickerType"));
            Vector3 startPos = card.transform.position;
            Vector3 endPos = CardChoiceVisuals.instance.transform.position;
            float c2 = 0f;
            while (c2 < 1f)
            {
                CardChoiceVisuals.instance.framesToSnap = 1;
                Vector3 position = Vector3.LerpUnclamped(startPos, endPos, __instance.curve.Evaluate(c2));
                card.transform.position = position;
                c2 += Time.deltaTime * speed;
                yield return null;
            }

            GamefeelManager.GameFeel((startPos - endPos).normalized * 2f);
            card.GetComponentInChildren<CardVisuals>().Leave();
            card.GetComponentInChildren<CardVisuals>().Pick();
        }

        [UnboundRPC]
        public static void GiveNulls(int playerID,int amount)
        {
            PlayerManager.instance.players.Find(p => p.playerID == playerID).data.stats.AjustNulls(amount);
        }
    }
}