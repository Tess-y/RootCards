using System;
using System.Runtime.CompilerServices;
using HarmonyLib;
using Photon.Pun;
using System.Collections.Generic;
using System.Text;
using RootCards.Extensions;
using UnityEngine;

namespace RootCards.Extensions
{
    public static class DevConsoleExtention
    {

        [PunRPC]
        public static void RPCA_WishCard(this DevConsole devConsole,String wish, Player wisher)
        {

            CardInfo[] cards = CardChoice.instance.cards;
            int num = -1;
            float num2 = 0f;
            for (int i = 0; i < cards.Length; i++)
            {
                string text = cards[i].GetComponent<CardInfo>().cardName.ToUpper();
                text = text.Replace(" ", "");
                string text2 = wish.ToUpper();
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
                ModdingUtils.Utils.Cards.instance.AddCardToPlayer(wisher, cards[num], addToCardBar: true);
                wisher.data.stats.GetRootData().freeCards -= 1;
            }
        }

    }
}