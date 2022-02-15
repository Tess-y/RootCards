using ModdingUtils.MonoBehaviours;
using System;
using System.Collections.Generic;
using System.Text;

namespace RootCards.MonoBehaviours
{
    internal class RageBuff : ReversibleEffect
    {
        public override void OnStart()
        {
            characterStatModifiers.movementSpeed = 1.2f;
            gun.damage = 1.4f;
            this.ApplyModifiers();
        }
    }
}
