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
using ItemShops;
using ItemShops.Utils;
using TMPro;
using ItemShops.Extensions;
using System.ArrayExtensions;
using ModdingUtils.Extensions;

namespace RootCards.Cards
{
    class Genie : CustomCard
    {

        public static Shop Genie_Shop;
        public static string ShopID = "Root_Genie_Shop";
        public static Dictionary<String, int> wishes = new Dictionary<String, int>();
        public static CardCategory GenieCategory = CustomCardCategories.instance.CardCategory("GenieCard");
        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
        {
            //Edits values on card itself, which are then applied to the player in `ApplyCardStats`
            cardInfo.GetAdditionalData().canBeReassigned = false;
            cardInfo.categories = new CardCategory[] { CustomCardCategories.instance.CardCategory("NoRandom"), GenieCategory };
            RootCards.Debug($"[{RootCards.ModInitials}][Card] {GetTitle()} has been setup.");
        } 
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            //Edits values on player when card is selected
            player.GetAdditionalData().bankAccount.Deposit("Wish",1);


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
            return RootCards.ArtAssets.LoadAsset<GameObject>("C_GENIE");
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
            wishes = new Dictionary<String, int>();
            wishes.Add("Wish", 1);
            if (Genie_Shop != null) ShopManager.instance.RemoveShop(Genie_Shop); 
            Genie_Shop = ShopManager.instance.CreateShop(ShopID);
            Genie_Shop.UpdateMoneyColumnName("Wishes");
            Genie_Shop.UpdateTitle("Be Carful What You Wish For");
            yield return new WaitForSecondsRealtime(0.2f);
            RootCards.instance.StartCoroutine(SetUpShop());
            yield break;
        }

        internal static IEnumerator SetUpShop()
        {
            List<UnboundLib.Utils.Card> allCards = UnboundLib.Utils.CardManager.cards.Values.ToList();
            foreach (UnboundLib.Utils.Card card in allCards)
            {
                if (card != null && card.cardInfo.name.ToLower() != "genie" && card.cardInfo.name.ToLower() != "immovable object" && card.cardInfo.name.ToLower() != "unstoppable force" && UnboundLib.Utils.CardManager.IsCardActive(card.cardInfo)) {
                    Genie_Shop.AddItem(new CardItem(card));
                }
            }

            yield break;
        }

        internal static IEnumerator WaitTillShopDone()
        {
            bool done = true;
            GameObject gameObject = null;
            GameObject timer = null;
            float time = 120;
            PlayerManager.instance.players.ForEach(p =>
            {
                ModdingUtils.Extensions.CharacterStatModifiersExtension.GetAdditionalData(p.data.stats).blacklistedCategories.Add(GenieCategory);
                if (p.GetAdditionalData().bankAccount.HasFunds(wishes)){ Genie_Shop.Show(p); done = false; }
            });

            if (!done)
            {
                gameObject = new GameObject();
                gameObject.AddComponent<Canvas>().sortingLayerName = "MostFront";
                gameObject.AddComponent<TextMeshProUGUI>().text = "Wating For Players In Wish Menu";
                Color c = Color.magenta;
                c.a = .85f;
                gameObject.GetComponent<TextMeshProUGUI>().color = c;
                gameObject.transform.localScale = new Vector3(.2f, .2f);
                gameObject.transform.localPosition = new Vector3(0, 5);
                timer = new GameObject();
                timer.AddComponent<Canvas>().sortingLayerName = "MostFront";
                timer.transform.localScale = new Vector3(.2f, .2f);
                timer.transform.localPosition = new Vector3(0, 16);
                timer.AddComponent<TextMeshProUGUI>().color = c;
                for (int i = 0; i < 5; i++)
                {
                    timer.GetComponent<TextMeshProUGUI>().text = ((int)time).ToString();
                    yield return new WaitForSecondsRealtime(1f);
                    time -= 1;
                }
            }
            while (!done)
            {
                timer.GetComponent<TextMeshProUGUI>().text = ((int)time).ToString();
                done = true;
                yield return new WaitForSecondsRealtime(0.2f);
                time -= 0.2f;
                PlayerManager.instance.players.ForEach(p => 
                {
                    if (ShopManager.instance.PlayerIsInShop(p)) done = false;
                });
                if (time <= 0)
                    ShopManager.instance.HideAllShops();

            }
            Destroy(gameObject);
            Destroy(timer);
            PlayerManager.instance.players.ForEach(p =>
            {
                ModdingUtils.Extensions.CharacterStatModifiersExtension.GetAdditionalData(p.data.stats).blacklistedCategories.Remove(GenieCategory);
                if (p.GetAdditionalData().bankAccount.HasFunds(wishes)) { Genie_Shop.Show(p); done = false; }
            });
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
    internal class CardItem : Purchasable
    {
        private UnboundLib.Utils.Card Card;
        private Dictionary<string, int> cost = new Dictionary<string, int>();
        public CardItem(UnboundLib.Utils.Card card)
        {
            Card = card;
            cost.Add("Wish", 1);
        }
        public override string Name { get { return Card.cardInfo.name; } }

        public override Dictionary<string, int> Cost { get{ return cost; } } 

        public override Tag[] Tags { get{ return new Tag[] { new Tag(Card.cardInfo.rarity.ToString()), new Tag(Card.category) }; } } 

        public override bool CanPurchase(Player player)
        {
            return true;
        }

        public override GameObject CreateItem(GameObject parent)
        {
            GameObject container = null;
            GameObject holder = null;

            try
            {
                container = GameObject.Instantiate(ItemShops.ItemShops.instance.assets.LoadAsset<GameObject>("Card Container"));
            }
            catch (Exception)
            {

                UnityEngine.Debug.Log("Issue with creating the card container");
            }

            try
            {
                holder = container.transform.Find("Card Holder").gameObject;
            }
            catch (Exception)
            {

                UnityEngine.Debug.Log("Issue with getting the Card Holder");
                holder = container.transform.GetChild(0).gameObject;
            }
            holder.transform.localPosition = new Vector3(0f, -100f, 0f);
            holder.transform.localScale = new Vector3(0.125f, 0.125f, 1f);
            holder.transform.Rotate(0f, 180f, 0f);

            GameObject cardObj = null;

            try
            {
                cardObj = GetCardVisuals(Card.cardInfo, holder);
            }
            catch (Exception e)
            {
                UnityEngine.Debug.Log("Issue with getting card visuals");
                UnityEngine.Debug.LogError(e);
            }

            container.transform.SetParent(parent.transform);

            return container;
        }

        public override void OnPurchase(Player player, Purchasable item)
        {
            var card = ((CardItem)item).Card.cardInfo; 
            System.Random r = new System.Random(Util.random.Seed());
            switch (card.rarity)
            {
                case CardInfo.Rarity.Common:
                    if (r.Next(10) == 0)
                    {
                        ModdingUtils.Utils.Cards.instance.AddCardToPlayer(player, ModdingUtils.Utils.Cards.instance.GetCardWithName("Genie: Smiles"), false, "", 2f, 2f);
                        ModdingUtils.Utils.Cards.instance.AddCardToPlayer(player, card, false, "", 2f, 2f);
                    }
                    else
                    {
                        ModdingUtils.Utils.Cards.instance.AddCardToPlayer(player, ModdingUtils.Utils.Cards.instance.GetCardWithName("Genie: Granted"), false, "", 2f, 2f);
                    }
                    break;
                case CardInfo.Rarity.Uncommon:
                    if (r.Next(10) == 0)
                    {
                        ModdingUtils.Utils.Cards.instance.AddCardToPlayer(player, ModdingUtils.Utils.Cards.instance.GetCardWithName("Genie: Eternity"), false, "", 2f, 2f);
                        player.data.stats.GetRootData().lockedCard = card;
                    }
                    else
                    {
                        ModdingUtils.Utils.Cards.instance.AddCardToPlayer(player, ModdingUtils.Utils.Cards.instance.GetCardWithName("Genie: Fee"), false, "", 2f, 2f);
                    }
                    break;
                case CardInfo.Rarity.Rare:
                    if (r.Next(10) == 0)
                    {
                        ModdingUtils.Utils.Cards.instance.RemoveAllCardsFromPlayer(player);
                        ModdingUtils.Utils.Cards.instance.AddCardToPlayer(player, ModdingUtils.Utils.Cards.instance.GetCardWithName("Genie: Greed"), false, "", 2f, 2f);
                    }
                    else
                    {
                        ModdingUtils.Utils.Cards.instance.AddCardToPlayer(player, ModdingUtils.Utils.Cards.instance.GetCardWithName("Genie: Death"), false, "", 2f, 2f);
                    }
                    break;
            }

            ModdingUtils.Utils.Cards.instance.AddCardToPlayer(player, card, false, "", 2f, 2f);
            RootCards.instance.StartCoroutine(ShowCard(player, card));
        }
        public static IEnumerator ShowCard(Player player, CardInfo card)
        {
            yield return ModdingUtils.Utils.CardBarUtils.instance.ShowImmediate(player, card, 2f);

            yield break;
        }


        private GameObject GetCardVisuals(CardInfo card, GameObject parent)
        {
            GameObject cardObj = GameObject.Instantiate<GameObject>(card.gameObject, parent.gameObject.transform);
            cardObj.SetActive(true);
            cardObj.GetComponentInChildren<CardVisuals>().firstValueToSet = true;
            RectTransform rect = cardObj.GetOrAddComponent<RectTransform>();
            rect.localScale = 100f * Vector3.one;
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            rect.pivot = new Vector2(0.5f, 0.5f);

            GameObject back = FindObjectInChildren(cardObj, "Back");
            try
            {
                GameObject.Destroy(back);
            }
            catch { }
            FindObjectInChildren(cardObj, "BlockFront")?.SetActive(false);

            var canvasGroups = cardObj.GetComponentsInChildren<CanvasGroup>();
            foreach (var canvasGroup in canvasGroups)
            {
                canvasGroup.alpha = 1;
            }

            ItemShops.ItemShops.instance.ExecuteAfterSeconds(0.2f, () =>
            {
                //var particles = cardObj.GetComponentsInChildren<GeneralParticleSystem>().Select(system => system.gameObject);
                //foreach (var particle in particles)
                //{
                //    UnityEngine.GameObject.Destroy(particle);
                //}

                var rarities = cardObj.GetComponentsInChildren<CardRarityColor>();

                foreach (var rarity in rarities)
                {
                    try
                    {
                        rarity.Toggle(true);
                    }
                    catch
                    {

                    }
                }

                var titleText = FindObjectInChildren(cardObj, "Text_Name").GetComponent<TextMeshProUGUI>();

                if ((titleText.color.r < 0.18f) && (titleText.color.g < 0.18f) && (titleText.color.b < 0.18f))
                {
                    titleText.color = new Color32(200, 200, 200, 255);
                }
            });

            return cardObj;
        }
        private static GameObject FindObjectInChildren(GameObject gameObject, string gameObjectName)
        {
            Transform[] children = gameObject.GetComponentsInChildren<Transform>(true);
            return (from item in children where item.name == gameObjectName select item.gameObject).FirstOrDefault();
        }
    }
}
