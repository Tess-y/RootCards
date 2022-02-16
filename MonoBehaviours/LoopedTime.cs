using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI.ProceduralImage;

namespace RootCards.MonoBehaviours
{
    internal class LoopedTime : MonoBehaviour
    {
        private float timer = 0;
        private bool stored = false;
        private Vector3 playerPos;
        private float playerHealth;
        private int playerAmmo;
        public Player player;
        public Gun gun;



        [Range(0f, 1f)]
        public float counter;

        public float timeToFill = 5f;

        public float timeToEmpty = 0f;

        public float duration = 1;

        public ProceduralImage outerRing;

        public ProceduralImage fill;

        public Transform rotator;

        public Transform still;



        public void Awake()
        {
            try
            {
                player = base.gameObject.GetComponent<Player>();
                gun = player.data.weaponHandler.gun;
            }
            catch (Exception e) { }
        }

        public void Start()
        {

        }

        public void Update()
        {
            if (player.data.view.IsMine)
            {

                timer += Time.deltaTime;
                counter = timer / 5;
                this.outerRing.fillAmount = this.counter;
                this.fill.fillAmount = this.counter;
                this.rotator.transform.localEulerAngles = new Vector3(0f, 0f, -Mathf.Lerp(0f, 360f, this.counter));

                if (timer > 2.5 && timer < 3)
                {
                    if (outerRing.color.r == 0)
                    {
                        outerRing.color = new Color32(255, 150, 0, 255);
                        rotator.gameObject.GetComponentInChildren<ProceduralImage>().color = outerRing.color;
                    }
                    else
                    {
                        outerRing.color = new Color32(150, 255, 0, 255);
                        rotator.gameObject.GetComponentInChildren<ProceduralImage>().color = outerRing.color;
                    }
                }
                else if (!stored && timer >= 3)
                {
                    outerRing.color = new Color32(255, 0, 0, 255);
                    rotator.gameObject.GetComponentInChildren<ProceduralImage>().color = outerRing.color;
                    playerPos = player.transform.position;
                    playerHealth = player.data.health;
                    playerAmmo = gun.ammo;
                    stored = true;
                }
                else if (timer >= 5.1)
                {
                    outerRing.color = new Color32(0, 255, 0, 255);
                    rotator.gameObject.GetComponentInChildren<ProceduralImage>().color = outerRing.color;
                    timer = 0;
                    stored = false;
                    player.GetComponentInParent<PlayerCollision>().IgnoreWallForFrames(2);
                    player.transform.position = playerPos;
                    player.data.healthHandler.Heal(playerHealth - player.data.health);
                    gun.ammo = playerAmmo;
                }
            }
        }
    }
}
