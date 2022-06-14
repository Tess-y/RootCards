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
using UnboundLib.Utils;
using System.Reflection;
using System.Collections.ObjectModel;
using ModdingUtils.Extensions;
using System.Collections;

namespace RootCards.Cards
{
    class DistillPower : CustomCard
    {
        public static CardCategory[] noLotteryCategories = new CardCategory[] { CardChoiceSpawnUniqueCardPatch.CustomCategories.CustomCardCategories.instance.CardCategory("CardManipulation"), CardChoiceSpawnUniqueCardPatch.CustomCategories.CustomCardCategories.instance.CardCategory("NoRandom") };
        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
        {
            //Edits values on card itself, which are then applied to the player in `ApplyCardStats`
            cardInfo.categories = new CardCategory[] { CardChoiceSpawnUniqueCardPatch.CustomCategories.CustomCardCategories.instance.CardCategory("CardManipulation") };
            cardInfo.GetAdditionalData().canBeReassigned = false;
            RootCards.Debug($"[{RootCards.ModInitials}][Card] {GetTitle()} has been setup.");
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            //Edits values on player when card is selected

            RootCards.instance.ExecuteAfterFrames(2, ()=> { RootCards.instance.StartCoroutine(addRandomCards(player, gun, gunAmmo, data, health, gravity, block, characterStats)); });
            Extensions.CharacterStatModifiersExtension.AjustNulls(characterStats, 3);
            RootCards.Debug($"[{RootCards.ModInitials}][Card] {GetTitle()} has been added to player {player.playerID}.");
        }

        public IEnumerator addRandomCards(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            for (int i = 0; i < 2; i++)
            {
                CardInfo randomCard = ModdingUtils.Utils.Cards.instance.NORARITY_GetRandomCardWithCondition(player, gun, gunAmmo, data, health, gravity, block, characterStats, this.condition);
                if (randomCard == null)
                {
                    // if there is no valid card, then try drawing from the list of all cards (inactive + active) but still make sure it is compatible
                    CardInfo[] allCards = ((ObservableCollection<CardInfo>)typeof(CardManager).GetField("activeCards", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null)).ToList().Concat((List<CardInfo>)typeof(CardManager).GetField("inactiveCards", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null)).ToArray();
                    randomCard = ModdingUtils.Utils.Cards.instance.DrawRandomCardWithCondition(allCards, player, null, null, null, null, null, null, null, this.condition);

                }

                //ModdingUtils.Utils.Cards.instance.AddCardToPlayer(player, randomCard, false, "", 2f);
                int cardCount = player.data.currentCards.Count();
                ModdingUtils.Utils.Cards.instance.AddCardToPlayer(player, randomCard, addToCardBar: true);
                var time = 0f;
                yield return new WaitUntil(() =>
                {
                    time += Time.deltaTime;
                    return ((player.data.currentCards.Count > cardCount) || (player.data.currentCards[player.data.currentCards.Count - 1] == randomCard) || (time > 5f));
                });
                ModdingUtils.Utils.CardBarUtils.instance.ShowAtEndOfPhase(player, randomCard);
            }
            yield break;
        }
        public override void OnRemoveCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            //Run when the card is removed from the player
            RootCards.Debug($"[{RootCards.ModInitials}][Card] {GetTitle()} has been removed from player {player.playerID}.");
        }

        protected override string GetTitle()
        {
            return "Distill Power";
        }
        protected override string GetDescription()
        {
            return "Gain two random <color=#ff00ffff>rares</color>\n\n\n <i>(Does not work right if it is your first card, we don't know why)</i>";
        }
        protected override GameObject GetCardArt()
        {
            return RootCards.ArtAssets.LoadAsset<GameObject>("C_DISTILL_POWER");
        }
        protected override CardInfo.Rarity GetRarity()
        {
            return CardInfo.Rarity.Common;
        }
        protected override CardInfoStat[] GetStats()
        {
            return new CardInfoStat[]
            {
                new CardInfoStat()
                {
                    positive = false,
                    stat = "Nulls",
                    amount = "+3",
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

        public override void Callback()
        {
            gameObject.AddComponent<Tess>();//set the author of the card
        }

        public bool condition(CardInfo card, Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            // do not allow duplicates of cards with allowMultiple == false (handled by moddingutils)
            // card rarity must be as desired
            // card cannot be another cardmanipulation card
            // card cannot be from a blacklisted catagory of any other card (handled by moddingutils)

            return (card.rarity == CardInfo.Rarity.Rare) && !card.categories.Intersect(DistillPower.noLotteryCategories).Any();

        }
    }
}
