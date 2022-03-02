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
using CardChoiceSpawnUniqueCardPatch.CustomCategories;
using System.Collections;
using RootCards.Util;
using UnboundLib.GameModes;

namespace RootCards.Cards
{
    class Genie : CustomCard
    {
        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
        {
            //Edits values on card itself, which are then applied to the player in `ApplyCardStats`
            cardInfo.categories = new CardCategory[] { CustomCardCategories.instance.CardCategory("NoRandom") };
            RootCards.Debug($"[{RootCards.ModInitials}][Card] {GetTitle()} has been setup.");
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            //Edits values on player when card is selected
            Extensions.CharacterStatModifiersExtension.GetRootData(characterStats).freeCards += 1;
            
            RootCards.Debug($"[{RootCards.ModInitials}][Card] {GetTitle()} has been added to player {player.playerID}.");
        }
        public override void OnRemoveCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            //Run when the card is removed from the player
            RootCards.Debug($"[{RootCards.ModInitials}][Card] {GetTitle()} has been removed from player {player.playerID}.");
        }

        protected override string GetTitle()
        {
            return "Genie";
        }
        protected override string GetDescription()
        {
            return "Ask and ye shall receive."; // (When you are ready to use your wish, type in chat and the Genie will try to give you the closest named card to what you said.)
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
                new CardInfoStat()
                {
                    positive = true,
                    stat = "Wish",
                    amount = "+1",
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                },
                new CardInfoStat()
                {
                    positive = false,
                    stat = ".........Yet",
                    amount = "-Nothing",
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
            card.gameObject.AddComponent<Lilith>();//set the author of the card
        }


        internal static IEnumerator Wish()
        {
            foreach (Player player in PlayerManager.instance.players.ToArray())
            {
                RootCards.Debug(player + ":" + Extensions.CharacterStatModifiersExtension.GetRootData(player.data.stats).freeCards);
                if (Extensions.CharacterStatModifiersExtension.GetRootData(player.data.stats).freeCards > 0)
                {
                    CardChoice.instance.IsPicking = true;
                    yield return GameModeManager.TriggerHook(GameModeHooks.HookPlayerPickStart);
                    yield return CardPickMenu.PickCard(player);
                    yield return new WaitForSecondsRealtime(0.1f);
                    yield return GameModeManager.TriggerHook(GameModeHooks.HookPlayerPickEnd);
                    yield return new WaitForSecondsRealtime(0.1f);
                    CardChoice.instance.IsPicking = false;
                    yield return new WaitForSecondsRealtime(0.2f);
                    ModdingUtils.Utils.Cards.instance.AddCardToPlayer(player, ModdingUtils.Utils.Cards.instance.GetCardWithName(" "), addToCardBar:false);
                    yield return new WaitForSecondsRealtime(0.2f);
                    ModdingUtils.Utils.Cards.instance.RemoveCardFromPlayer(player, ModdingUtils.Utils.Cards.instance.GetCardWithName(" "), ModdingUtils.Utils.Cards.SelectionType.Newest);
                }
            }
            yield break;
        }

        internal static IEnumerator RestCardLock()
        {
            foreach (Player player in PlayerManager.instance.players.ToArray())
            {
                Extensions.CharacterStatModifiersExtension.GetRootData(player.data.stats).lockedCard = null;
            }
            yield break;
        }
    }
}
