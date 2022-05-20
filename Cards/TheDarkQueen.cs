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

namespace RootCards.Cards
{
    class TheDarkQueen : CustomCard
    {
        public static CardInfo card;
        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
        {
            //Edits values on card itself, which are then applied to the player in `ApplyCardStats`
            cardInfo.allowMultiple = false;
            RootCards.Debug($"[{RootCards.ModInitials}][Card] {GetTitle()} has been setup.");
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            //Edits values on player when card is selected
            GameObject forceField = Instantiate(RootCards.ArtAssets.LoadAsset<GameObject>("DarkQueen"), player.transform);
            player.GetComponent<Gravity>().enabled = false;
            player.GetComponent<PlayerVelocity>().enabled = false;
            RootCards.Debug($"[{RootCards.ModInitials}][Card] {GetTitle()} has been added to player {player.playerID}.");
        } 
        public override void OnRemoveCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            //Run when the card is removed from the player
            player.GetComponent<Gravity>().enabled = true;
            player.GetComponent<PlayerVelocity>().enabled = true;
            Destroy(player.transform.Find("DarkQueen").gameObject);
            RootCards.Debug($"[{RootCards.ModInitials}][Card] {GetTitle()} has been removed from player {player.playerID}.");
        }

        protected override string GetTitle()
        {
            return "The Dark Queen";
        }
        protected override string GetDescription()
        {
            return "";
        }
        protected override GameObject GetCardArt()
        {
            return RootCards.ArtAssets.LoadAsset<GameObject>("C_THE_DARK_QUEEN");
        }
        protected override CardInfo.Rarity GetRarity()
        {
            return CardInfo.Rarity.Uncommon;
        }
        protected override CardInfoStat[] GetStats()
        {
            return new CardInfoStat[]
            {
            };
        }
        protected override CardThemeColor.CardThemeColorType GetTheme()
        {
            return CardThemeLib.CardThemeLib.instance.CreateOrGetType("DarknessBlack");
        }
        public override string GetModName()
        {
            return RootCards.ModInitials;
        }

        public override void Callback()
        {
            gameObject.AddComponent<Lilith>();//set the author of the card
            var m = gameObject.AddComponent<ClassesManagerReborn.Util.ClassNameMono>();
            m.className = "";
            m.color1 = new Color(0.9f, 0.0765f, 0.3567f);
            m.color2 = m.color1;
        }
    }
}
