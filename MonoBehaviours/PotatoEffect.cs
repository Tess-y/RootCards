using ModdingUtils.MonoBehaviours;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnboundLib.GameModes;
using UnityEngine;

namespace RootCards.MonoBehaviours
{
    internal class PotatoEffect : MonoBehaviour
    {

        int oldPotatoCount = 0;
        float _movespeedDelta = 0;
        private Player _player;
        public void Start()
        {
            if (_player == null) _player = gameObject.GetComponent<Player>();
        }

        public void Update()
        {
            if (_player == null) return;
            int potatoCount = _player.data.currentCards.FindAll(c => c == Cards.FrozenPotato.cardInfo).Count;
            if (potatoCount == oldPotatoCount) return;
            oldPotatoCount = potatoCount;
            _player.data.stats.movementSpeed -= _movespeedDelta;

            _movespeedDelta = (_player.data.stats.movementSpeed / MathF.Pow(2,potatoCount)) - _player.data.stats.movementSpeed;

            _player.data.stats.movementSpeed += _movespeedDelta;
        }
    }
}
