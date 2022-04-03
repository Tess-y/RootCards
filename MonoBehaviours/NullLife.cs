using RootCards.Cards;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using ModdingUtils;
using System.Collections;
using UnboundLib.GameModes;

namespace RootCards.MonoBehaviours
{
    internal class NullLife : MonoBehaviour
    {
        private Player player;
        bool triggered = false;
        public int nulllive = 0;
        bool active = false;

        public void Start()
        {
            player = base.gameObject.GetComponent<Player>();
            GameModeManager.AddHook(GameModeHooks.HookPointEnd, pointEnd);
            GameModeManager.AddHook(GameModeHooks.HookPointStart, pointStart);
            active = false;
        }
        public void Update()
        {
            if(player.data.stats.remainingRespawns == 0 && nulllive < player.data.currentCards.FindAll(c => c == Null.NULLCARD).Count)
            {
                nulllive++;
                player.data.stats.remainingRespawns += 1;
            }
        }

        public void OnDestroy()
        {
            GameModeManager.RemoveHook(GameModeHooks.HookPickEnd, pointEnd);
        }
        public IEnumerator pointStart(IGameModeHandler gm)
        {
            active = true;
            yield break;
        }
        public IEnumerator pointEnd(IGameModeHandler gm)
        {
            active = false;
            for (; nulllive > 0; nulllive--)
                ModdingUtils.Utils.Cards.instance.RemoveCardFromPlayer(player, Null.NULLCARD, ModdingUtils.Utils.Cards.SelectionType.Oldest);
            yield break;
        }
    }
}
