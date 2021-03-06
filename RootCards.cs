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
using System.Collections;
using BepInEx.Configuration;
using TMPro;
using UnboundLib.Utils.UI;
using ItemShops.Utils;
using UnityEngine.UI;
using RootCards.Cards.Util;
using WillsWackyManagers.Utils;
using RarityLib.Utils;

namespace RootCards
{
    // These are the mods required for our Mod to work
    [BepInDependency("com.willis.rounds.unbound", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("pykess.rounds.plugins.moddingutils", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("pykess.rounds.plugins.cardchoicespawnuniquecardpatch", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("com.willuwontu.rounds.itemshops", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("com.willuwontu.rounds.managers", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("root.classes.manager.reborn")]
    [BepInDependency("root.cardtheme.lib")]
    [BepInDependency("root.rarity.lib")]
    // Declares our Mod to Bepin
    [BepInPlugin(ModId, ModName, Version)]
    // The game our Mod Is associated with
    [BepInProcess("Rounds.exe")]
    public class RootCards : BaseUnityPlugin
    {
        public static  Type UniqueCardPatchType = typeof(CardChoiceSpawnUniqueCardPatch.NullCard).Assembly.GetType("CardChoicePatchSpawnUniqueCard");
        public static ConfigEntry<bool> DEBUG;
        private const string ModId = "com.Root.Cards";
        private const string ModName = "RootCards";
        public const string Version = "0.9.2"; // What version are we On (major.minor.patch)?
        internal static AssetBundle ArtAssets;
        //private static readonly AssetBundle Bundle = Jotunn.Utils.AssetUtils.LoadAssetBundleFromResources("rootassets", typeof(RootCards).Assembly);
        public const string ModInitials = "Root"; 
        public static RootCards instance { get; private set; }
        public static CardCategory PotatoCategory; 


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

            DEBUG = base.Config.Bind<bool>(ModInitials, "Debug", false, "Enable to turn on concole spam from our mod");
            CardThemeLib.CardThemeLib.instance.CreateOrGetType("DarknessBlack", new CardThemeColor() { bgColor = new Color(0.1978f, 0.1088f, 0.1321f), targetColor = new Color(0.0978f, 0.1088f, 0.1321f) });
            RarityUtils.AddRarity("Trinket", 3, new Color(0.38f, 0.38f, 0.38f), new Color(0.0978f, 0.1088f, 0.1321f));
        }
        void Start()
        {
            instance = this;
            Unbound.RegisterMenu("Root Settings", delegate () { }, new Action<GameObject>(this.NewGUI), null, true);

            ArtAssets =  Jotunn.Utils.AssetUtils.LoadAssetBundleFromResources("rootassets", typeof(RootCards).Assembly);
            PotatoCategory = CustomCardCategories.instance.CardCategory("PotatoCard");


            CustomCard.BuildCard<BloodBullets>();
            CustomCard.BuildCard<DownUpHere>();
            CustomCard.BuildCard<JohsonsIngenuity>();
            CustomCard.BuildCard<UsedAmmo>();
            CustomCard.BuildCard<LilithsDeal>(card => LilithsDeal.card = card);
            CustomCard.BuildCard<ContractOfSouls>(card => ContractOfSouls.card = card);
            CustomCard.BuildCard<TheDarkQueen>(card => TheDarkQueen.card = card);
            CustomCard.BuildCard<FrozenPotato>(card => FrozenPotato.cardInfo = card); 
            CustomCard.BuildCard<ToxicPotato>(card => ToxicPotato.cardInfo = card); 
            CustomCard.BuildCard<OneHitWonder>(); 
            //CustomCard.BuildCard<BattleRage>(BattleRage.callback); 
            CustomCard.BuildCard<TimeLoop>(); 
            CustomCard.BuildCard<DropGrenade>();
            CustomCard.BuildCard<QuickShield>();
            CustomCard.BuildCard<Genie>();
            CustomCard.BuildCard<WitchTime>();
            CustomCard.BuildCard<StayHungry>();
            CustomCard.BuildCard<TacticalInversion>();
            //CustomCard.BuildCard<Drilling_Ammo_Card>(Drilling_Ammo_Card.callback);


            CustomCard.BuildCard<Null>(Null.callback);
            CustomCard.BuildCard<DistillKnowledge>();
            CustomCard.BuildCard<DistillAcquisition>();
            CustomCard.BuildCard<DistillPower>();
            //CustomCard.BuildCard<ReforgeSoul>(ReforgeSoul.callback);
            CustomCard.BuildCard<ReforgeVitality>();
            CustomCard.BuildCard<ReforgeOffense>();
            CustomCard.BuildCard<ReforgeDefence>();
            CustomCard.BuildCard<ReforgeReflection>();
            CustomCard.BuildCard<ReforgeIntegraty>();

            ///Genie Outcomes/

            CustomCard.BuildCard<GenieDeath>(GenieDeath.callback);
            CustomCard.BuildCard<GenieFee>(GenieFee.callback);
            CustomCard.BuildCard<GenieGranted>(GenieGranted.callback);
            CustomCard.BuildCard<GenieGreed>(GenieGreed.callback);
            CustomCard.BuildCard<GenieSmiles>(GenieSmiles.callback);
            CustomCard.BuildCard<GenieEternity>(GenieEternity.callback);
            CustomCard.BuildCard<EmptyLamp>(EmptyLamp.callback);
            ///End Outcomes

            GameModeManager.AddHook(GameModeHooks.HookGameStart, (gm) => Genie.Wish());
            GameModeManager.AddHook(GameModeHooks.HookPickEnd, (gm) => Genie.WaitTillShopDone());
            GameModeManager.AddHook(GameModeHooks.HookGameStart, (gm) => Genie.RestCardLock());
            GameModeManager.AddHook(GameModeHooks.HookGameStart, (gm) => Null.clearNulls());
            GameModeManager.AddHook(GameModeHooks.HookPlayerPickEnd, (gm) => DistillKnowledge.ExtraPicks());
            GameModeManager.AddHook(GameModeHooks.HookPlayerPickStart, (gm) => CardChoicePatchSpawn.resetNull());
            GameModeManager.AddHook(GameModeHooks.HookGameStart, (gm) => NullManager.ResetLibrary());
            GameModeManager.AddHook(GameModeHooks.HookRoundEnd, (gm) => NullManager.CleanupRemovedNulls());
            GameModeManager.AddHook(GameModeHooks.HookPlayerPickStart, (gm) => NullManager.CalculateNulls());


            CurrencyManager.instance.RegisterCurrencyIcon("Wish",(image) =>
            {
                image.sprite = ArtAssets.LoadAsset<Sprite>("WISH");
                image.color = new Color32(118, 117, 35, 255);
            });


            RerollManager.instance.RegisterRerollAction(rerollAction);
            //Unbound.Instance.ExecuteAfterFrames(15, () => StartCoroutine(NullCard.genNulls()));

        }

        public void rerollAction(Player player, CardInfo[] _)
        {
            RootCards.Debug("RerollAction Called");
            StartCoroutine(NullManager.DoHandleReroll(player.playerID));
        }

        public static void Debug(object message)
        {
            if (DEBUG.Value)
            {
                UnityEngine.Debug.Log("ROOT=>" + message);
            }
        }

        private void NewGUI(GameObject menu)
        {
            TextMeshProUGUI textMeshProUGUI;
            MenuHandler.CreateText("Root Settings", menu, out textMeshProUGUI, 60, false, null, null, null, null);
            GameObject toggle = MenuHandler.CreateToggle(RootCards.DEBUG.Value, "Debug Mode", menu, delegate (bool value)
            {
                RootCards.DEBUG.Value = value;
            }, 50, false, Color.red, null, null, null);
        }
    }
}