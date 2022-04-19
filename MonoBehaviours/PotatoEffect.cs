using ModdingUtils.MonoBehaviours;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnboundLib.GameModes;
using UnityEngine;

namespace RootCards.MonoBehaviours
{
    internal class PotatoEffect : ReversibleEffect
    {

        float movespeed = 1;
        bool enabled = false;
        public override void OnStart()
        {

            GameModeManager.AddHook(GameModeHooks.HookBattleStart, stats);
            movespeed = Mathf.Pow(0.5f, player.data.currentCards.FindAll(c => c == Cards.FrozenPotato.cardInfo).Count);
            characterStatModifiersModifier.movementSpeed_mult = movespeed;
            SetLivesToEffect(int.MaxValue);
            enabled = true;
        }

        public override void OnUpdate()
        {
            if (!enabled) return;
            movespeed = Mathf.Pow(0.5f, player.data.currentCards.FindAll(c => c == Cards.FrozenPotato.cardInfo).Count);
            if(characterStatModifiersModifier.movementSpeed_mult != movespeed)
            {
                UnityEngine.Debug.Log(movespeed);
                ClearModifiers();
                characterStatModifiersModifier.movementSpeed_mult = movespeed;
                ApplyModifiers();
            }
        }

        public IEnumerator stats(IGameModeHandler gm)
        {
            ClearModifiers();
            ApplyModifiers();
            yield break;
        }
    }
}
