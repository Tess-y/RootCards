using ModdingUtils.MonoBehaviours;
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
            int potatoTCount = _player.data.currentCards.FindAll(c => c == Cards.ToxicPotato.cardInfo).Count;
            if (potatoTCount > 0 && _player.data.view.IsMine) _player.data.healthHandler.CallTakeDamage(Vector2.down * _player.data.maxHealth * TimeHandler.deltaTime * 0.05f * potatoTCount, transform.position);
            int potatoFCount = _player.data.currentCards.FindAll(c => c == Cards.FrozenPotato.cardInfo).Count;
            if (potatoFCount == oldPotatoCount) return;
            oldPotatoCount = potatoFCount;
            _player.data.stats.movementSpeed -= _movespeedDelta;

            _movespeedDelta = (_player.data.stats.movementSpeed / Mathf.Pow(2, potatoFCount)) - _player.data.stats.movementSpeed;

            _player.data.stats.movementSpeed += _movespeedDelta;
        }
    }
}
