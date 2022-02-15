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
using UnityEngine.UI.ProceduralImage;

namespace RootCards.Cards
{
    class TimeLoop : CustomCard
    {
        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
        {
            //Edits values on card itself, which are then applied to the player in `ApplyCardStats`
            cardInfo.allowMultiple = false;
            UnityEngine.Debug.Log($"[{RootCards.ModInitials}][Card] {GetTitle()} has been setup.");
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            //Edits values on player when card is selected

            var abyssalCard = CardChoice.instance.cards.First(c => c.name.Equals("AbyssalCountdown"));
            var statMods = abyssalCard.gameObject.GetComponentInChildren<CharacterStatModifiers>();
            var abyssalObj = statMods.AddObjectToPlayer;

            UnityEngine.Debug.Log("making loop");

            var loopObject = Instantiate(abyssalObj, player.transform);
            loopObject.name = "LoopedTime";
            loopObject.transform.localPosition = Vector3.zero;
            var abyssal = loopObject.GetComponent<AbyssalCountdown>();

            UnityEngine.Debug.Log("Abyssal made");

            LoopedTime loopedTime = loopObject.AddComponent<LoopedTime>();
            loopedTime.player = player;
            loopedTime.gun = gun;

            loopedTime.timeToFill = 5f;
            loopedTime.timeToEmpty = 0f;
            loopedTime.outerRing = abyssal.outerRing;
            loopedTime.fill = abyssal.fill;
            loopedTime.rotator = abyssal.rotator;
            loopedTime.still = abyssal.still;
            loopedTime.player = player;



            RootCards.instance.ExecuteAfterFrames(5, () =>
            {
                try
                {
                    UnityEngine.GameObject.Destroy(abyssal);

                    var COs = loopObject.GetComponentsInChildren<Transform>().Where(child => child.parent == loopObject.transform).Select(child => child.gameObject).ToArray();

                    foreach (var CO in COs)
                    {
                        if (CO.transform.gameObject != loopObject.transform.Find("Canvas").gameObject)
                        {
                            UnityEngine.GameObject.Destroy(CO);
                        }
                    }
                }
                catch (Exception e)
                {
                    UnityEngine.Debug.Log("First Catch");
                    UnityEngine.Debug.LogException(e);
                }
                try
                {
                    loopedTime.outerRing.color = new Color32(0, 255, 0, 255);
                    loopedTime.fill.color = new Color32(10, 255, 10, 0);
                    loopedTime.rotator.gameObject.GetComponentInChildren<ProceduralImage>().color = loopedTime.outerRing.color;
                    loopedTime.still.gameObject.GetComponentInChildren<ProceduralImage>().color = loopedTime.outerRing.color;
                    loopObject.transform.Find("Canvas/Size/BackRing").GetComponent<ProceduralImage>().color = new Color32(33, 33, 33, 29);
                }
                catch (Exception e)
                {
                    UnityEngine.Debug.Log("Last Catch");
                    UnityEngine.Debug.LogException(e);
                }
            });

            UnityEngine.Debug.Log($"[{RootCards.ModInitials}][Card] {GetTitle()} has been added to player {player.playerID}.");
        }
        public override void OnRemoveCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            //Run when the card is removed from the player
            UnityEngine.Debug.Log($"[{RootCards.ModInitials}][Card] {GetTitle()} has been removed from player {player.playerID}.");
        }

        protected override string GetTitle()
        {
            return "Time Loop";
        }
        protected override string GetDescription()
        {
            return "Every five seconds, travel two seconds back in time.";
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
                    positive = false,
                    stat = "Time",
                    amount = "Endless Looped",
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                },
                new CardInfoStat()
                {
                    positive = true,
                    stat = "Bargain",
                    amount = "I've Come to",
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                }
            };
        }
        protected override CardThemeColor.CardThemeColorType GetTheme()
        {
            return CardThemeColor.CardThemeColorType.ColdBlue;
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
