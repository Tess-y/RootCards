using System;
using System.Runtime.CompilerServices;
using HarmonyLib;


namespace RootCards.Extensions
{
    // ADD FIELDS TO CHARACTERSTATMODIFIERS
    [Serializable]
    public class CharacterStatModifiersRootData
    {
        public float shieldEfectiveness;
        public int freeCards;
        public CharacterStatModifiersRootData()
        {
            shieldEfectiveness = 1;
            freeCards = 0;
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
        }
    }
}
