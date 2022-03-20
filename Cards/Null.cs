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
using System.Collections;

namespace RootCards.Cards
{
    class Null : CustomCard
    {
        public static CardInfo NULLCARD;
        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
        {
            //Edits values on card itself, which are then applied to the player in `ApplyCardStats`
          /*  System.Random random = new System.Random();
            cardInfo.cardDestription = Guid.NewGuid().ToString();
            cardInfo.cardColor = new Color(random.Next(255), random.Next(255), random.Next(255));*/
            cardInfo.categories = new CardCategory[] { CustomCardCategories.instance.CardCategory("NoRandom") };
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
            return "NULL";
        }
        protected override string GetDescription()
        {
            return "";
        }
        protected override GameObject GetCardArt()
        {
            return RootCards.ArtAssets.LoadAsset<GameObject>("C_NULL");
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
            return CardThemeColor.CardThemeColorType.TechWhite;
        }
        public override string GetModName()
        {
            return "";
        }

        public override bool GetEnabled()
        {
            return false;  
        }

        internal static void callback(CardInfo card)
        {
            card.gameObject.AddComponent<NullCard>();
            ModdingUtils.Utils.Cards.instance.AddHiddenCard(card);
            NULLCARD = card;
        }

        internal static IEnumerator clearNulls()
        {
            foreach (Player player in PlayerManager.instance.players.ToArray())
            {
                Extensions.CharacterStatModifiersExtension.GetRootData(player.data.stats).nulls = 0;
            }
            yield break;
        }
    }

    internal class NullCard : MonoBehaviour
    {
        public void Update()
        {
            if(gameObject.GetComponent<CardInfo>().sourceCard != Null.NULLCARD)
            {
                gameObject.GetComponent<CardInfo>().sourceCard = Null.NULLCARD;
            }
        }
    }
}
