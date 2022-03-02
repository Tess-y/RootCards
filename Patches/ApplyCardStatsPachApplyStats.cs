using System;
using System.Collections.Generic;
using System.Text;
using HarmonyLib;
using RootCards.Extensions;
using UnityEngine;

namespace RootCards.Patches
{

	[Serializable]
	[HarmonyPatch(typeof(ApplyCardStats), "ApplyStats")]
	public class ApplyCardStatsPachApplyStats
	{
		private static void Postfix(ApplyCardStats __instance)
        {
			RootCards.Debug(__instance);
			GunAmmo ammo = ((Player) Traverse.Create(__instance).Field("playerToUpgrade").GetValue()).GetComponent<Holding>().holdable.GetComponent<Gun>().GetComponentInChildren<GunAmmo>();
			ammo.maxAmmo = Mathf.Clamp(ammo.maxAmmo, 1, ((Player) Traverse.Create(__instance).Field("playerToUpgrade").GetValue()).data.stats.GetRootData().ammoCap);
		}
	}
}
