using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnboundLib;
using UnboundLib.Cards;
using UnityEngine;
using RootCards.Cards.Util.Authors;

namespace RootCards.Cards
{
    class Drilling_Ammo_Card : CustomCard
    {
        private static GameObject drill = new GameObject("drill").AddComponent<Dirll>().gameObject;
        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
        {
            gun.objectsToSpawn = new ObjectsToSpawn[] {  new ObjectsToSpawn(){
                effect = drill,
                spawnAsChild = true,
                spawnOn = ObjectsToSpawn.SpawnOn.notPlayer
            }
            };
            gun.attackSpeed = 10;
            gun.projectielSimulatonSpeed = .25f;

        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
        }
        public override void OnRemoveCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
        }


        protected override string GetTitle()
        {
            return "Drilling Ammo";
        }
        protected override string GetDescription()
        {
            return "Your bullets destroy parts of the map on contact";
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
                    stat = "Projectile Speed",
                    amount = "-75%",
                    simepleAmount = CardInfoStat.SimpleAmount.aLotLower
                },
                new CardInfoStat()
                {
                    positive = false,
                    stat = "Attack Speed",
                    amount = "-90%",
                    simepleAmount = CardInfoStat.SimpleAmount.aLotLower
                }
            };
        }
        protected override CardThemeColor.CardThemeColorType GetTheme()
        {
            return CardThemeColor.CardThemeColorType.FirepowerYellow;
        }
        public override string GetModName()
        {
            return RootCards.ModInitials;
        }

        public override void Callback()
        {
            gameObject.AddComponent<Tess>();//set the author of the card
        }
    }

    internal class Dirll : MonoBehaviour
    {
        void Update()
        {
            if (this.gameObject.transform.parent != null)
            {
                this.gameObject.transform.parent.gameObject.GetOrAddComponent<Minning>();
                Destroy(this.gameObject.transform.parent.gameObject, 1.2f);
                Destroy(this.gameObject);
            }
        }
    }

    internal class Minning : MonoBehaviour
    {
        float time;
        Vector3 scale;
        void Start()
        {
            time = 1;
            scale = this.transform.localScale;
        }
        void Update()
        {
            time -= Time.deltaTime;
            if (time < 0) time = 0;
            this.transform.localScale = scale*time;
        }
    }
}
