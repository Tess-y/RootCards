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

namespace RootCards.Cards
{
    class GenieFee : CustomCard
    {
        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
        {
            //Edits values on card itself, which are then applied to the player in `ApplyCardStats`
            statModifiers.health = 0.9f;
            statModifiers.movementSpeed = 0.9f;
            statModifiers.jump = 0.9f;
            gun.reloadTime = 1.1f;
            cardInfo.categories = new CardCategory[] { CustomCardCategories.instance.CardCategory("NoRandom"), Genie.GenieOutcomeCategory };
            RootCards.Debug($"[{RootCards.ModInitials}][Card] {GetTitle()} has been setup.");
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            //Edits values on player when card is selected
            RootCards.Debug($"[{RootCards.ModInitials}][Card] {GetTitle()} has been added to player {player.playerID}.");
        }
        public override void OnRemoveCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            //Run when the card is removed from the player
            RootCards.Debug($"[{RootCards.ModInitials}][Card] {GetTitle()} has been removed from player {player.playerID}.");
        }

        protected override string GetTitle()
        {
            return "Genie: Fee";
        }
        protected override string GetDescription()
        {
            return "Your wish is granted, but my powers are not free. You should know by now that everything comes with a fee. A piece of your soul shall suffice.";
        }
        protected override GameObject GetCardArt()
        {
            return RootCards.ArtAssets.LoadAsset<GameObject>("C_OUTCOME");
        }
        protected override CardInfo.Rarity GetRarity()
        {
            return CardInfo.Rarity.Common;
        }
        protected override CardInfoStat[] GetStats()
        {
            return new CardInfoStat[] {
                new CardInfoStat()
                {
                    positive = false,
                    stat = "Health",
                    amount = "-10%",
                    simepleAmount = CardInfoStat.SimpleAmount.slightlyLower
                },new CardInfoStat()
                {
                    positive = false,
                    stat = "Movment Speed",
                    amount = "-10%",
                    simepleAmount = CardInfoStat.SimpleAmount.slightlyLower
                },new CardInfoStat()
                {
                    positive = false,
                    stat = "Jump Hight",
                    amount = "-10%",
                    simepleAmount = CardInfoStat.SimpleAmount.slightlyLower
                },new CardInfoStat()
                {
                    positive = false,
                    stat = "Reload Time",
                    amount = "+10%",
                    simepleAmount = CardInfoStat.SimpleAmount.aLittleBitOf
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

        public override bool GetEnabled()
        {
            return false;
        }

        public override void Callback()
        {
            gameObject.AddComponent<Lilith>();//set the author of the card
        }
        internal static void callback(CardInfo card)
        {
            ModdingUtils.Utils.Cards.instance.AddHiddenCard(card);
        }
    }
}
