using BepInEx;
using UnboundLib;
using UnboundLib.Cards;
using RootCards.Cards;
using HarmonyLib;
using CardChoiceSpawnUniqueCardPatch.CustomCategories;

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
        private const string ModId = "com.Root.Cards";
        private const string ModName = "RootCards";
        public const string Version = "0.1.2"; // What version are we On (major.minor.patch)?
        public const string ModInitials = "Root";
        public static RootCards instance { get; private set; }

        void Awake()
        {
            // Use this to call any harmony patch files your Mod may have
            var harmony = new Harmony(ModId);
            harmony.PatchAll();
        }
        void Start()
        {
            instance = this;
            CustomCard.BuildCard<BloodBullets>(BloodBullets.callback);
            CustomCard.BuildCard<DownUpHere>(DownUpHere.callback);
            CustomCard.BuildCard<JohsonsIngenuity>(JohsonsIngenuity.callback);
            CustomCard.BuildCard<UsedAmmo>(UsedAmmo.callback);
            CustomCard.BuildCard<LilithsDeal>(LilithsDeal.callback);
            //CustomCard.BuildCard<FrozenPotato>(FrozenPotato.callback); card is currently bugged.
            CustomCard.BuildCard<OneHitWonder>(OneHitWonder.callback); 
            CustomCard.BuildCard<BattleRage>(BattleRage.callback); 
            CustomCard.BuildCard<TimeLoop>(TimeLoop.callback);  // I plan to go back and add a timer visual at some point --Tess
            CustomCard.BuildCard<DropGrenade>(DropGrenade.callback);
        }
    }
}