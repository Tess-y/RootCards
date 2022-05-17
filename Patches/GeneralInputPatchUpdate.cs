using HarmonyLib;
using RootCards.Cards;
using System;
using System.Collections.Generic;
using System.Text;

namespace RootCards.Patches
{
    [HarmonyPatch(typeof(GeneralInput), "Update")]
    internal class GeneralInputPatchUpdate
    {
        public static float vel = 10;
        private static void Postfix(GeneralInput __instance)
        {
            try
            {
                CharacterData data = __instance.GetComponent<CharacterData>();
                if (!data.currentCards.Contains(TheDarkQueen.card)) return;
                if (data.playerActions.Up.IsPressed)
                {
                    UnityEngine.Debug.Log("up");
                        __instance.transform.position += UnityEngine.Vector3.up * vel * data.stats.movementSpeed * TimeHandler.deltaTime;
                }
                if (data.playerActions.Down.IsPressed)
                {
                    UnityEngine.Debug.Log("down");
                    __instance.transform.position += UnityEngine.Vector3.down * vel * data.stats.movementSpeed * TimeHandler.deltaTime;
                }
                if (data.playerActions.Left.IsPressed)
                {
                    UnityEngine.Debug.Log("left");
                    __instance.transform.position += UnityEngine.Vector3.left * vel * data.stats.movementSpeed * TimeHandler.deltaTime;
                }
                if (data.playerActions.Right.IsPressed)
                {
                    UnityEngine.Debug.Log("right");
                    __instance.transform.position += UnityEngine.Vector3.right * vel * data.stats.movementSpeed * TimeHandler.deltaTime;
                }
            }
            catch { }
        }
    }
}
