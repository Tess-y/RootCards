using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace RootCards.Patches
{
    [Serializable]
    [HarmonyPatch(typeof(Block), "Update")]
    internal class BlockPatchUpdate
    {
        private static bool Prefix() {  return true; }
    }
}
