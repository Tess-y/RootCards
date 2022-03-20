using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace RootCards.Cards.Util.Authors
{
    // destroy object once its no longer a child
    public class DestroyOnUnparent : MonoBehaviour
    {
        void LateUpdate()
        {
            if (this.gameObject.transform.parent == null) { Destroy(this.gameObject); } 
        }
    }
}
