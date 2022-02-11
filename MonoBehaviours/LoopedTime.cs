using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace RootCards.MonoBehaviours
{
    internal class LoopedTime : MonoBehaviour
    {
        private float timer = 0;
        private bool stored = false;
        private float playerX;
        private float playerY;
        private float playerHealth;
        private int playerAmmo;
        public Player player;
        public Gun gun;

        private void Awake()
        {
            player = base.gameObject.GetComponent<Player>();
            gun = base.gameObject.GetComponent<Gun>();
        }

        private void Update()
        {
            timer += Time.deltaTime;
            if (!stored && timer >= 3)
            {
                playerX = player.transform.position.x;
                playerY = player.transform.position.y;
                playerHealth = player.data.health;
                playerAmmo = gun.ammo;
                stored = true; 
            }
            else if(timer >= 5)
            {
                timer = 0;
                stored = false;
                player.GetComponentInParent<PlayerCollision>().IgnoreWallForFrames(2);
                player.transform.position = new Vector3(playerX, playerY, player.transform.position.y);
                player.data.healthHandler.Heal(playerHealth - player.data.health);
                gun.ammo = playerAmmo;
            }
        }
    }
}
