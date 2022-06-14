using System;
using System.Runtime.CompilerServices;
using HarmonyLib;
using UnityEngine;
using UnboundLib;
using RootCards.Cards;
using System.Collections.Generic;

namespace RootCards.Extensions
{
    // ADD FIELDS TO CHARACTERSTATMODIFIERS
    [Serializable]
    public class CharacterStatModifiersRootData
    {
        public float shieldEfectiveness;
        public int freeCards;
        public int ammoCap;
        public int bulletCap;
        public int trueMaxAmmo;
        public CardInfo? lockedCard;
        public int nulls;
        public int roundNulls;
        public float witchTimeDuration;
        public bool stillShoping;
        public bool knowledge;
        public NullData nullData;
        public CharacterStatModifiersRootData()
        {
            shieldEfectiveness = 1;
            freeCards = 0;
            ammoCap = -1;
            bulletCap = -1;
            trueMaxAmmo = 3;
            lockedCard = null;
            nulls = 0;
            roundNulls = 0;
            witchTimeDuration = 0;
            stillShoping = false;
            knowledge = false;
            nullData = new NullData();
        }
    }

    public class NullData
    {
        public float Health_multiplier;
        public float MovmentSpeed_multiplier;
        public float Damage_multiplier;
        public int gun_Reflects;
        public int gun_Ammo;
        internal float Lifesteal;
        internal float block_cdMultiplier;

        public NullData()
        {
            Health_multiplier = 1;
            MovmentSpeed_multiplier = 1;
            Damage_multiplier = 1;
            Lifesteal = 0;
            block_cdMultiplier = 1;
            gun_Reflects = 0;
            gun_Ammo = 0;
        }

    }

    public static class CharacterStatModifiersExtension
    {
        public static readonly ConditionalWeakTable<CharacterStatModifiers, CharacterStatModifiersRootData> data =
          new ConditionalWeakTable<CharacterStatModifiers, CharacterStatModifiersRootData>();


        public static CharacterStatModifiersRootData GetRootData(this CharacterStatModifiers characterstats)
        {
            return data.GetOrCreateValue(characterstats);
        }

        public static void AjustNulls(this CharacterStatModifiers characterstats, int value)
        {
            characterstats.GetRootData().nulls = Mathf.Clamp(characterstats.GetRootData().nulls+value,0,100);
        }


        public static void AddData(this CharacterStatModifiers characterstats, CharacterStatModifiersRootData value)
        {
            try
            {
                data.Add(characterstats, value);
            }
            catch (Exception) { }
        }

    }
    // reset additional CharacterStatModifiers when ResetStats is called
    [HarmonyPatch(typeof(CharacterStatModifiers), "ResetStats")]
    class CharacterStatModifiersPatchResetStats
    {
        private static void Prefix(CharacterStatModifiers __instance)
        {
            __instance.GetRootData().shieldEfectiveness = 1;
            __instance.GetRootData().freeCards = 0;
            __instance.GetRootData().ammoCap = -1;
            __instance.GetRootData().ammoCap = -1;
            __instance.GetRootData().trueMaxAmmo = 3;
            __instance.GetRootData().witchTimeDuration = 0;
            __instance.GetRootData().stillShoping = false;
            __instance.GetRootData().knowledge = false;
            __instance.GetRootData().nullData = new NullData();
        }
    }
}
