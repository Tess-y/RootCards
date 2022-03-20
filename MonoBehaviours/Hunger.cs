using ModdingUtils.MonoBehaviours;
using System;
using System.Collections.Generic;
using System.Text;
using UnboundLib;
using UnityEngine;

namespace RootCards.MonoBehaviours
{
    internal class Hunger : ReversibleEffect
    {
        internal int hungerLevel = 1;
        internal int hungerGrowth = 0;
        internal int hungerMax = 30;
        public void AttackAction() { hungerLevel += hungerGrowth; }

        public override void OnStart()
        {
            base.OnStart();
            gunStatModifier.projectileColor = Color.white;
            SetLivesToEffect(int.MaxValue);
            gun.AddAttackAction(AttackAction);
        }
        public override void OnOnDestroy()
        {
            base.OnOnDestroy();
            gun.InvokeMethod("RemoveAttackAction", (Action)AttackAction);
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            if (hungerLevel != gunStatModifier.damage_mult)
            {
                hungerLevel = Mathf.Clamp(hungerLevel, 1, hungerMax);
                float dilute = 1 -  Mathf.Clamp(hungerLevel/(float)hungerMax, 0f,1f);
                gunStatModifier.projectileColor = new Color(1f, dilute, dilute); 
                ClearModifiers();
                gunStatModifier.damage_mult = hungerLevel;
                ApplyModifiers();
            }
        }

        public override void OnOnDisable()
        {
            base.OnOnDisable();
            hungerLevel = 1;
        }
    }

    internal class Devourer : DealtDamageEffect
    {
        public Hunger hunger;
        public override void DealtDamage(Vector2 damage, bool selfDamage, Player damagedPlayer = null)
        {
            RootCards.Debug(hunger);
            hunger.gunStatModifier.projectileColor = Color.white;
            hunger.hungerLevel = 1;
        }
    }
}
