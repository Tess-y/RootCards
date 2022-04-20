using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnboundLib;
using UnboundLib.Cards;
using UnityEngine;
using RootCards.Cards.Util.Authors;
using RootCards.MonoBehaviours;
using RootCards.Extensions;
using ModdingUtils.Extensions;
using RootCards.Cards.Util;
using System.Collections;
using ModdingUtils.Utils;

namespace RootCards.Cards
{
    class ReforgeIntegraty : CustomCard
    {
        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
        {
            //Edits values on card itself, which are then applied to the player in `ApplyCardStats`
            cardInfo.categories = new CardCategory[] { Null.NeedsNull };
            cardInfo.GetAdditionalData().canBeReassigned = false;
            RootCards.Debug($"[{RootCards.ModInitials}][Card] {GetTitle()} has been setup.");
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            //Edits values on player when card is selected
            RootCards.instance.ExecuteAfterFrames(2, () =>
                 {
                     List<CardInfo> cards = data.currentCards;
                     List<int> nulls = new List<int>();
                     List<CardInfo> nulleds = new List<CardInfo>();
                     for (int i = 0; i< cards.Count; i++)
                     {
                         if (cards[i].GetComponent<Null>() != null)
                         {
                             nulls.Add(i);
                             nulleds.Add(NullManager.GetNulledForPlayer(player.playerID,i));
                         }
                     }
                     Unbound.Instance.StartCoroutine(ReplaceCards(player, nulls.ToArray(), nulleds.ToArray(), editCardBar: true));
                     characterStats.AjustNulls(-nulls.Count);
                 });
            RootCards.Debug($"[{RootCards.ModInitials}][Card] {GetTitle()} has been added to player {player.playerID}.");
        } 
        public override void OnRemoveCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            //Run when the card is removed from the player
            RootCards.Debug($"[{RootCards.ModInitials}][Card] {GetTitle()} has been removed from player {player.playerID}.");
        }

        protected override string GetTitle()
        {
            return "Reforge Integraty";
        }
        protected override string GetDescription()
        {
            return "Turn nulls you've taken back into the cards they Nulled out.";
        }
        protected override GameObject GetCardArt()
        {
            return null;
        }
        protected override CardInfo.Rarity GetRarity()
        {
            return CardInfo.Rarity.Rare;
        }
        protected override CardInfoStat[] GetStats()
        {
            return new CardInfoStat[]
            {
            };
        }
        protected override CardThemeColor.CardThemeColorType GetTheme()
        {
            return CardThemeColor.CardThemeColorType.EvilPurple;
        }
        public override string GetModName()
        {
            return RootCards.ModInitials;
        }

        internal static void callback(CardInfo card)
        {
            card.gameObject.AddComponent<Tess>();//set the author of the card
        }

        public IEnumerator ReplaceCards(Player player, int[] indeces, CardInfo[] newCards, string[] twoLetterCodes = null, bool editCardBar = true)
        {
            if (twoLetterCodes == null)
            {
                twoLetterCodes = new string[indeces.Length];
                for (int i = 0; i < twoLetterCodes.Length; i++)
                {
                    twoLetterCodes[i] = "";
                }
            }
            List<bool> reassign = new List<bool>();

            List<CardInfo> list = new List<CardInfo>();
            foreach (CardInfo currentCard in player.data.currentCards)
            {
                list.Add(currentCard);
            }

            List<CardInfo> newCardsToAssign = new List<CardInfo>();
            List<string> twoLetterCodesToAssign = new List<string>();
            int num = 0;
            for (int j = 0; j < list.Count; j++)
            {
                if (!indeces.Contains(j))
                {
                    newCardsToAssign.Add(list[j]);
                    twoLetterCodesToAssign.Add("");
                    reassign.Add(true);
                }
                else if (newCards[num] == null)
                {
                    newCardsToAssign.Add(list[j]);
                    twoLetterCodesToAssign.Add("");
                    num++;
                    reassign.Add(true);
                }
                else
                {
                    newCardsToAssign.Add(newCards[num]);
                    twoLetterCodesToAssign.Add(twoLetterCodes[num]);
                    num++;
                    reassign.Add(false);
                }
            }

            ModdingUtils.Utils.Cards.instance.RemoveAllCardsFromPlayer(player, editCardBar);
            yield return new WaitForSecondsRealtime(0.1f);
            if (editCardBar)
            {
                CardBarUtils.instance.ClearCardBar(player);
            }

            ModdingUtils.Utils.Cards.instance.AddCardsToPlayer(player, newCardsToAssign.ToArray(), reassign.ToArray(), twoLetterCodesToAssign.ToArray(), null, null, editCardBar);
        }

    }
}
