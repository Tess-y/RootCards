using BepInEx;
using UnboundLib;
using UnboundLib.Cards;
using RootCards.Cards;
using HarmonyLib;
using CardChoiceSpawnUniqueCardPatch.CustomCategories;
using UnityEngine;
using UnboundLib.GameModes;
using System;
using RootCards.Patches;

namespace RootCards
{
    // These are the mods required for our Mod to work
    [BepInDependency("com.willis.rounds.unbound", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("pykess.rounds.plugins.moddingutils", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("pykess.rounds.plugins.cardchoicespawnuniquecardpatch", BepInDependency.DependencyFlags.HardDependency)]
    // Declares our Mod to Bepin
    [BepInPlugin(ModId, ModName, Version)]
    // The game our Mod Is associated with
    [BepInProcess("Rounds.exe")]
    public class RootCards : BaseUnityPlugin
    {
        public static  Type UniqueCardPatchType = typeof(CardChoiceSpawnUniqueCardPatch.NullCard).Assembly.GetType("CardChoicePatchSpawnUniqueCard");
        public const bool DEBUG = true;
        private const string ModId = "com.Root.Cards";
        private const string ModName = "RootCards";
        public const string Version = "0.3.0"; // What version are we On (major.minor.patch)?
        internal static AssetBundle ArtAssets;
        //private static readonly AssetBundle Bundle = Jotunn.Utils.AssetUtils.LoadAssetBundleFromResources("rootassets", typeof(RootCards).Assembly);
        public const string ModInitials = "Root";
        public static RootCards instance { get; private set; }

        void Awake()
        {
            // Use this to call any harmony patch files your Mod may have
            var harmony = new Harmony(ModId);
            harmony.PatchAll();/*
            var prefix = typeof(CardChoiceSpawnUniqueCardPatchPatchGetCondition).GetMethod("Prefix");
            harmony.Patch(UniqueCardPatchType.GetMethod("GetCondition"), new HarmonyMethod(prefix)
            {
                priority = 9999,
            });*/
        }
        void Start()
        {
            instance = this;
            
            ArtAssets =  Jotunn.Utils.AssetUtils.LoadAssetBundleFromResources("rootassets", typeof(RootCards).Assembly);

            CustomCard.BuildCard<BloodBullets>(BloodBullets.callback);
            CustomCard.BuildCard<DownUpHere>(DownUpHere.callback);
            CustomCard.BuildCard<JohsonsIngenuity>(JohsonsIngenuity.callback);
            CustomCard.BuildCard<UsedAmmo>(UsedAmmo.callback);
            CustomCard.BuildCard<LilithsDeal>(LilithsDeal.callback);
            //CustomCard.BuildCard<FrozenPotato>(FrozenPotato.callback); card is currently bugged.
            CustomCard.BuildCard<OneHitWonder>(OneHitWonder.callback); 
            CustomCard.BuildCard<BattleRage>(BattleRage.callback); 
            CustomCard.BuildCard<TimeLoop>(TimeLoop.callback); 
            CustomCard.BuildCard<DropGrenade>(DropGrenade.callback);
            CustomCard.BuildCard<QuickShield>(QuickShield.callback);
            CustomCard.BuildCard<Genie>(Genie.callback);
            CustomCard.BuildCard<Null>(Null.callback);
            //CustomCard.BuildCard<DistillPower>(DistillPower.callback); //work in progress 

            ///Genie Outcomes
            CustomCard.BuildCard<GenieDeath>(GenieDeath.callback);
            CustomCard.BuildCard<GenieFee>(GenieFee.callback);
            CustomCard.BuildCard<GenieGranted>(GenieGranted.callback);
            CustomCard.BuildCard<GenieGreed>(GenieGreed.callback);
            CustomCard.BuildCard<GenieSmiles>(GenieSmiles.callback);
            CustomCard.BuildCard<GenieEternity>(GenieEternity.callback);
            ///End Outcomes

            GameModeManager.AddHook(GameModeHooks.HookPickEnd, (gm) => Genie.Wish());
            GameModeManager.AddHook(GameModeHooks.HookGameStart, (gm) => Genie.RestCardLock());
            GameModeManager.AddHook(GameModeHooks.HookPickStart, (gm) => Null.clearTempNulls());
        }
        public static void Debug(object message)
        {
            if (DEBUG)
            {
                UnityEngine.Debug.Log(message);
            }
        }
    }
}