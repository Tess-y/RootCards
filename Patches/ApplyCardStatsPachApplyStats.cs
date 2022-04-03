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
			//RootCards.Debug(__instance);
			Player player = (Player)Traverse.Create(__instance).Field("playerToUpgrade").GetValue();
			Gun gun = player.GetComponent<Holding>().holdable.GetComponent<Gun>();
			if (player.data.stats.GetRootData().ammoCap != -1) {
				GunAmmo ammo = gun.GetComponentInChildren<GunAmmo>();
				ammo.maxAmmo = Mathf.Clamp(ammo.maxAmmo, 1, player.data.stats.GetRootData().ammoCap);
			}
			if (player.data.stats.GetRootData().bulletCap != -1)
			{
				gun.numberOfProjectiles = Mathf.Clamp(gun.numberOfProjectiles, 1, player.data.stats.GetRootData().bulletCap);
			}
		}
	}
}
