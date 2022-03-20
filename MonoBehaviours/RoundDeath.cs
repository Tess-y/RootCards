using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Photon.Pun;
using UnboundLib;
using UnboundLib.GameModes;
using UnityEngine;

namespace RootCards.MonoBehaviours
{
    internal class RoundDeath : MonoBehaviourPun
    {
        public void Start()
        {
            GameModeManager.AddHook(GameModeHooks.HookBattleStart, PointStart);
        }

        private IEnumerator PointStart(IGameModeHandler gm)
        {
            List<TeamScore> currentScore = PlayerManager.instance.players.Select(p => p.teamID).Distinct().Select(ID => gm.GetTeamScore(ID)).ToList();

            int totalPointsEarned = 0;

            foreach (var score in currentScore)
            {
                totalPointsEarned += score.points;
            }

            if (totalPointsEarned == 0)
            {
                base.gameObject.GetComponent<Player>().data.view.RPC("RPCA_Die", RpcTarget.All, new object[]
                    {
                                    new Vector2(0, 1)
                    });
            }
            return null;
        }
    }
}
