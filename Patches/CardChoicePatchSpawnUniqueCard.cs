using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using RootCards.Extensions;
using UnityEngine;

namespace RootCards.Patches
{
    [Serializable]
    [HarmonyPatch(typeof(CardChoice), "SpawnUniqueCard")]
    internal class CardChoicePatchSpawnUniqueCard
    {
        [HarmonyPriority(int.MinValue)]
        private static bool Prefix(ref GameObject __result, CardChoice __instance, Vector3 pos, Quaternion rot)
        {
            bool flag = (PickerType)Traverse.Create(__instance).Field("pickerType").GetValue() == PickerType.Team;
            Player player;
            if (flag)
            {
                player = PlayerManager.instance.GetPlayersInTeam(__instance.pickrID)[0];
            }
            else
            {
                player = PlayerManager.instance.players[__instance.pickrID];
            }
            RootCards.Debug(player.data.stats.GetRootData().lockedCard);
            RootCards.Debug(player.data.currentCards.Count);
            if (player.data.stats.GetRootData().lockedCard == null)
            {
                if (player.data.stats.GetRootData().roundNulls < player.data.stats.GetRootData().nulls)
                {
                    Vector3 rotV = rot.eulerAngles;

                    GameObject.Destroy(__result);
                    GameObject gameObject = (GameObject)typeof(CardChoice).InvokeMember("Spawn", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.InvokeMethod, null, __instance, new object[]
                    {
                    ModdingUtils.Utils.Cards.instance.GetCardWithName("NULL").gameObject,
                    pos,
                    Quaternion.Euler( new Vector3(rotV.x,rotV.y,rotV.z+180))
                    });
                    gameObject.GetComponent<CardInfo>().sourceCard = ModdingUtils.Utils.Cards.instance.GetCardWithName("NULL");
                    gameObject.GetComponentInChildren<DamagableEvent>().GetComponent<Collider2D>().enabled = false;
                    __result = gameObject;
                    player.data.stats.GetRootData().roundNulls++;
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                GameObject.Destroy(__result);
                GameObject gameObject = (GameObject)typeof(CardChoice).InvokeMember("Spawn", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.InvokeMethod, null, __instance, new object[]
                {
                    player.data.stats.GetRootData().lockedCard.gameObject,
                    pos,
                    rot
                });

                gameObject.GetComponent<CardInfo>().sourceCard = player.data.stats.GetRootData().lockedCard.GetComponent<CardInfo>();
                gameObject.GetComponentInChildren<DamagableEvent>().GetComponent<Collider2D>().enabled = false;
                __result = gameObject;
                return false;
            }
        }

        /*
        private static bool Prefix(ref Func<CardInfo, Player, Gun, GunAmmo, CharacterData, HealthHandler, Gravity, Block, CharacterStatModifiers, bool> __result, CardChoice instance) {
            bool flag = (PickerType)Traverse.Create(instance).Field("pickerType").GetValue() == PickerType.Team;
            Player player;
            if (flag)
            {
                player = PlayerManager.instance.GetPlayersInTeam(instance.pickrID)[0];
            }
            else
            {
                player = PlayerManager.instance.players[instance.pickrID];
            }
            if(player.data.stats.GetRootData().lockedCard == null)
            {
                return true;
            }
            else
            {
                __result = (CardInfo card, Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers stats) => card == player.data.stats.GetRootData().lockedCard;
                return false;
            }
        }*/
    }
}
