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
using CardChoiceSpawnUniqueCardPatch.CustomCategories;
using RootCards.Extensions;

namespace RootCards.Cards
{
    class GenieGreed : CustomCard
    {
        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
        {
            //Edits values on card itself, which are then applied to the player in `ApplyCardStats`
            cardInfo.categories = new CardCategory[] { CustomCardCategories.instance.CardCategory("NoRandom"), CustomCardCategories.instance.CardCategory("GenieCard") };
            RootCards.Debug($"[{RootCards.ModInitials}][Card] {GetTitle()} has been setup.");
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            //Edits values on player when card is selected
           /* List<CardInfo> cards = player.data.currentCards;
            foreach (CardInfo card in cards)
            {
                if (!card.categories.Contains(CustomCardCategories.instance.CardCategory("GenieCard")) && cards.IndexOf(card) != cards.Count -1)
                {
                    ModdingUtils.Utils.Cards.instance.RemoveCardFromPlayer(player, card,ModdingUtils.Utils.Cards.SelectionType.Oldest);
                }
            }*/
            RootCards.Debug($"[{RootCards.ModInitials}][Card] {GetTitle()} has been added to player {player.playerID}.");
        }
        public override void OnRemoveCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            //Run when the card is removed from the player
            RootCards.Debug($"[{RootCards.ModInitials}][Card] {GetTitle()} has been removed from player {player.playerID}.");
        }


        protected override string GetTitle()
        {
            return "Genie: Greed";
        }
        protected override string GetDescription()
        {
            return "You ask for a lot, but be that truly what you want? Here, take it, for it is all that you now have.";
        }
        protected override GameObject GetCardArt()
        {
            return null;
        }
        protected override CardInfo.Rarity GetRarity()
        {
            return CardInfo.Rarity.Common;
        }
        protected override CardInfoStat[] GetStats()
        {
            return new CardInfoStat[] { };
        }
        protected override CardThemeColor.CardThemeColorType GetTheme()
        {
            return CardThemeColor.CardThemeColorType.MagicPink;
        }
        public override string GetModName()
        {
            return RootCards.ModInitials;
        }

        public override bool GetEnabled()
        {
            return false;
        }

        internal static void callback(CardInfo card)
        {
            card.gameObject.AddComponent<Lilith>();//set the author of the card
            ModdingUtils.Utils.Cards.instance.AddHiddenCard(card);
        }
    }
}
