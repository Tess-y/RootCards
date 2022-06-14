using System;
using System.Collections.Generic;
using System.Text;
using UnboundLib.Cards;
using UnityEngine;

namespace RootCards.Cards
{
    internal class EmptyLamp : CustomCard
    {
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
        }


        protected override GameObject GetCardArt()
        {
            return RootCards.ArtAssets.LoadAsset<GameObject>("C_OUTCOME");
        }
        protected override CardInfo.Rarity GetRarity()
        {
            return CardInfo.Rarity.Common;
        }

        protected override string GetDescription()
        {
            return "There might have been a Genie here once, but now it is just a lamp.";
        }

        protected override CardInfoStat[] GetStats()
        {
            return new CardInfoStat[0];
        }

        protected override CardThemeColor.CardThemeColorType GetTheme()
        {
            return CardThemeLib.CardThemeLib.instance.CreateOrGetType("DarknessBlack");
        }

        protected override string GetTitle()
        {
            return "Empty Lamp";
        }

        internal static void callback(CardInfo card)
        {
            ModdingUtils.Utils.Cards.instance.AddHiddenCard(card);
        }

        public override bool GetEnabled()
        {
            return false;
        }
        public override string GetModName()
        {
            return RootCards.ModInitials;
        }
    }
}
