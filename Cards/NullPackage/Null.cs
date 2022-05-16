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
using CardChoiceSpawnUniqueCardPatch.CustomCategories;
using System.Collections;
using RootCards.Extensions;
using TMPro;
using UnityEngine.UI;
using HarmonyLib;
using Photon.Pun;
using UnboundLib.Utils;
using System.Collections.ObjectModel;
using RootCards.Cards.Util;

namespace RootCards.Cards
{

	//File recovered via dnSpy
	//TODO: Clean up code for readableility 
	internal class Null : CustomCard
	{
		public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
		{
			cardInfo.categories = new CardCategory[]
			{
				CustomCardCategories.instance.CardCategory("NoRandom"), CustomCardCategories.instance.CardCategory("nullCard")
			};
			RootCards.Debug("[Root][Card] " + this.GetTitle() + " has been setup.");
		}

		public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
		{
			data.maxHealth *= characterStats.GetRootData().nullData.Health_multiplier;
			characterStats.movementSpeed *= characterStats.GetRootData().nullData.MovmentSpeed_multiplier;
			characterStats.lifeSteal += characterStats.GetRootData().nullData.Lifesteal;
			block.cdMultiplier *= characterStats.GetRootData().nullData.block_cdMultiplier;
			gun.damage *= characterStats.GetRootData().nullData.Damage_multiplier;
			gun.reflects += characterStats.GetRootData().nullData.gun_Reflects;
			gunAmmo.maxAmmo += characterStats.GetRootData().nullData.gun_Ammo;
			characterStats.AjustNulls(-1);
			RootCards.Debug(string.Format("[{0}][Card] {1} has been added to player {2}.", "Root", this.GetTitle(), player.playerID));
		 	RootCards.instance.ExecuteAfterFrames(1, ()=> NullManager.RegisterNull(player.playerID, NulledCard));
		}

		public override void OnRemoveCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
		{
			characterStats.AjustNulls(1);
			NullManager.RemoveNull(player.playerID, NulledCard);
			RootCards.Debug(string.Format("[{0}][Card] {1} has been removed from player {2}.", "Root", this.GetTitle(), player.playerID));
		}

		protected override string GetTitle()
		{
			return "[]NULL";
		}

		protected override string GetDescription()
		{
			return "";
		}
		protected override GameObject GetCardArt()
		{
			return RootCards.ArtAssets.LoadAsset<GameObject>("C_NULL");
		}

		protected override CardInfo.Rarity GetRarity()
		{
			return CardInfo.Rarity.Common;
		}

		protected override CardInfoStat[] GetStats()
		{
			return new CardInfoStat[]
			{
				new CardInfoStat
				{
					positive = true,
					stat = "Null",
					amount = "-1",
					simepleAmount = CardInfoStat.SimpleAmount.notAssigned
				}
			};
		}

		protected override CardThemeColor.CardThemeColorType GetTheme()
		{
			return CardThemeColor.CardThemeColorType.TechWhite;
		}

		public override string GetModName()
		{
			return "";
		}

		public override bool GetEnabled()
		{
			return false;
		}

		public override void Callback()
		{
			gameObject.AddComponent<NullCard>();
		}
		internal static void callback(CardInfo card)
		{
			ModdingUtils.Utils.Cards.instance.AddHiddenCard(card);
			Null.NULLCARD = card;
		}

		internal static IEnumerator clearNulls()
		{
			foreach (Player player in PlayerManager.instance.players.ToArray())
			{
				player.data.stats.GetRootData().nulls = 0;
				ModdingUtils.Extensions.CharacterStatModifiersExtension.GetAdditionalData(player.data.stats).blacklistedCategories.Add(Null.NeedsNull);
			}
			Player[] array = null;
			yield break;
		}

		public static CardInfoStat[] GetStatsForPlayer(int playerID)
		{
			if(playerID < 0) return new CardInfoStat[0];
			NullData nullData = PlayerManager.instance.players.Find(p => p.playerID == playerID).data.stats.GetRootData().nullData;
			List<CardInfoStat> stats = new List<CardInfoStat>();
			stats.Add(new CardInfoStat()
			{
				positive = true,
				stat = "Null",
				amount = "-1",
				simepleAmount = CardInfoStat.SimpleAmount.notAssigned
			});
			if (nullData.Health_multiplier > 1)
				stats.Add(new CardInfoStat()
				{
					positive = true,
					stat = "Health",
					amount = $"+{(int)((nullData.Health_multiplier - 1) * 100)}%",
					simepleAmount = CardInfoStat.SimpleAmount.notAssigned
				});

			if (nullData.MovmentSpeed_multiplier > 1)
				stats.Add(new CardInfoStat()
				{
					positive = true,
					stat = "Movemet Speed",
					amount = $"+{(int)((nullData.MovmentSpeed_multiplier - 1) * 100)}%",
					simepleAmount = CardInfoStat.SimpleAmount.notAssigned
				});

			if (nullData.Lifesteal > 0)
				stats.Add(new CardInfoStat()
				{
					positive = true,
					stat = "Lifesteal",
					amount = $"+{(int)((nullData.Lifesteal) * 100)}%",
					simepleAmount = CardInfoStat.SimpleAmount.notAssigned
				});

			if (nullData.block_cdMultiplier < 1)
				stats.Add(new CardInfoStat()
				{
					positive = true,
					stat = "Block Cooldown",
					amount = $"-{(int)((1 - nullData.block_cdMultiplier) * 100)}%",
					simepleAmount = CardInfoStat.SimpleAmount.notAssigned
				});

			if (nullData.Damage_multiplier > 1)
				stats.Add(new CardInfoStat()
				{
					positive = true,
					stat = "Damage",
					amount = $"+{(int)((nullData.Damage_multiplier - 1) * 100)}%",
					simepleAmount = CardInfoStat.SimpleAmount.notAssigned
				});

			if (nullData.gun_Reflects > 0)
				stats.Add(new CardInfoStat()
				{
					positive = true,
					stat = "Bounces",
					amount = $"+{nullData.gun_Reflects}",
					simepleAmount = CardInfoStat.SimpleAmount.notAssigned
				});

			if (nullData.gun_Ammo > 0)
				stats.Add(new CardInfoStat()
				{
					positive = true,
					stat = "Ammo",
					amount = $"+{nullData.gun_Ammo}",
					simepleAmount = CardInfoStat.SimpleAmount.notAssigned
				});
			return stats.ToArray();
		}

		public static CardInfo NULLCARD;

		public static CardCategory NeedsNull = CustomCardCategories.instance.CardCategory("NeedsNull");

		public static List<CardInfo> nulled_Cards = new List<CardInfo>();

		public CardInfo NulledCard = null;
	}

	internal class NullCard : MonoBehaviour
	{
		private void Start()
		{
			TextMeshProUGUI[] componentsInChildren = base.gameObject.GetComponentsInChildren<TextMeshProUGUI>();
			GameObject gameObject = (from obj in componentsInChildren
									 where obj.gameObject.name == "Text_Name"
									 select obj).FirstOrDefault<TextMeshProUGUI>().gameObject;
			this.card = base.GetComponent<CardInfo>();
			this.statHolder = (from obj in base.gameObject.GetComponentsInChildren<LayoutElement>()
							   where obj.gameObject.name == "StatObject(Clone)"
							   select obj).First<LayoutElement>();
			this.cardName = gameObject.GetComponent<TextMeshProUGUI>();
			try
			{
				this.NulledCard = ModdingUtils.Utils.Cards.instance.GetCardWithName((string)base.gameObject.GetPhotonViewsInChildren()[0].InstantiationData[0]);
				this.playerID = (int)base.gameObject.GetPhotonViewsInChildren()[0].InstantiationData[1];
				this.card.sourceCard = Null.NULLCARD;
			}
			catch
			{
			}
		}

		public void Update()
		{
			if (this.updated)
            {
                if (this.NulledCard != null)
                {
                    this.title = this.NulledCard.cardName;
					this.card.GetComponent<Null>().NulledCard = this.NulledCard;
				}
                LayoutElement[] array = (from obj in base.gameObject.GetComponentsInChildren<LayoutElement>()
										 where obj.gameObject.name == "StatObject(Clone)"
										 select obj).ToArray<LayoutElement>();
				for (int i = 1; i < array.Length; i++)
				{
					Destroy(array[i].gameObject);
				}
				this.cardName.text = this.title.ToUpper();
				this.card.cardStats = Null.GetStatsForPlayer(playerID);
				for (int j = 1; j < this.card.cardStats.Length; j++)
				{
					LayoutElement layoutElement = Instantiate<LayoutElement>(this.statHolder);
					layoutElement.name = "StatObject(Clone)";
					layoutElement.transform.SetParent(this.statHolder.transform.parent, false);
					(from obj in layoutElement.gameObject.GetComponentsInChildren<TextMeshProUGUI>()
					 where obj.gameObject.name == "Stat"
					 select obj).FirstOrDefault<TextMeshProUGUI>().text = this.card.cardStats[j].stat;
					(from obj in layoutElement.gameObject.GetComponentsInChildren<TextMeshProUGUI>()
					 where obj.gameObject.name == "Value"
					 select obj).FirstOrDefault<TextMeshProUGUI>().text = this.card.cardStats[j].amount;
				}
				this.updated = false;
			}
		}

		private TextMeshProUGUI description;

		private TextMeshProUGUI cardName;

		public CardInfo NulledCard;

		public CardInfo card;

		public int playerID = -1;

		public bool updated = true;

		public string title = "NULL";

		private LayoutElement statHolder;

		private NullCard.StatHolder[] stats = new NullCard.StatHolder[0];

		private class StatHolder
		{
			public TextMeshProUGUI stat;

			public TextMeshProUGUI value;
		}
	}

	[HarmonyPatch(typeof(CardBarButton), "OnPointerEnter")]
	[Serializable]
	internal class CardBarButtonPatchOnPointerEnter
	{
		// Token: 0x060000B7 RID: 183 RVA: 0x000047C8 File Offset: 0x000029C8
		private static void Postfix(CardBarButton __instance)
		{
			if (((CardInfo)__instance.GetFieldValue("card")).cardName.ToLower() == Null.NULLCARD.cardName.ToLower())
			{
				int num = __instance.gameObject.transform.GetSiblingIndex() - 1;
				int num2 = Array.IndexOf<CardBar>((CardBar[])CardBarHandler.instance.GetFieldValue("cardBars"), __instance.gameObject.transform.parent.GetComponent<CardBar>());
				Player player = null;
				foreach (Player player2 in PlayerManager.instance.players)
				{
					if (player2.playerID == num2)
					{
						player = player2;
						break;
					}
				}
				if (player != null)
				{
					if ((GameObject)__instance.GetComponentInParent<CardBar>().GetFieldValue("currentCard") != null)
					{
						NullCard component = ((GameObject)__instance.GetComponentInParent<CardBar>().GetFieldValue("currentCard")).GetOrAddComponent<NullCard>();
						if (component != null)
						{
							component.playerID = player.playerID;
							component.NulledCard = NullManager.GetNulledForPlayer(player.playerID,num);
						}
					}
				}
			}
		}
	}

}
