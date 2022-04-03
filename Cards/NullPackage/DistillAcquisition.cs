﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnboundLib;
using UnboundLib.Cards;
using UnityEngine;
using RootCards.Cards.Util.Authors;
using RootCards.MonoBehaviours;
using ModdingUtils.Extensions;

namespace RootCards.Cards
{
    class DistillAcquisition : CustomCard
    {
        private List<GameObject> cards;
        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
        {
            //Edits values on card itself, which are then applied to the player in `ApplyCardStats`
            cards = null;
            try { cards = (List<GameObject>)CardChoice.instance.GetFieldValue("spawnedCards"); } catch { }
            cardInfo.GetAdditionalData().canBeReassigned = false;
            cardInfo.categories = new CardCategory[] { CardChoiceSpawnUniqueCardPatch.CustomCategories.CustomCardCategories.instance.CardCategory("CardManipulation") };
            RootCards.Debug($"[{RootCards.ModInitials}][Card] {GetTitle()} has been setup.");
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            //Edits values on player when card is selected
            if(cards != null)
            {
                foreach(var card in cards)
                {
                    if(card.name.ToLower() != this.GetTitle().ToLower())
                    {
                        CardChoice.instance.Pick(card);
                    }
                }
                Extensions.CharacterStatModifiersExtension.AjustNulls(characterStats, (int)((cards.Count+1) / 2));
            }
            RootCards.Debug($"[{RootCards.ModInitials}][Card] {GetTitle()} has been added to player {player.playerID}.");
        } 
        public override void OnRemoveCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            //Run when the card is removed from the player
            RootCards.Debug($"[{RootCards.ModInitials}][Card] {GetTitle()} has been removed from player {player.playerID}.");
        }

        protected override string GetTitle()
        {
            return "Distill Acquisition";
        }
        protected override string GetDescription()
        {
            return "Take all cards in the current hand";
        }
        protected override GameObject GetCardArt()
        {
            return null;
        }
        protected override CardInfo.Rarity GetRarity()
        {
            return CardInfo.Rarity.Uncommon;
        }
        protected override CardInfoStat[] GetStats()
        {
            return new CardInfoStat[]
            {
                new CardInfoStat()
                {
                    positive = false,
                    stat = "Per 2 cards",
                    amount = "+1 Null",
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                }
            };
        }
        protected override CardThemeColor.CardThemeColorType GetTheme()
        {
            return CardThemeColor.CardThemeColorType.MagicPink;
        }
        public override string GetModName()
        {
            return RootCards.ModInitials;
        }

        internal static void callback(CardInfo card)
        {
            card.gameObject.AddComponent<Tess>();//set the author of the card
        }
    }
}