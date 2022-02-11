using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using TMPro;
using System.Linq;

namespace RootCards.Cards.Util.Authors
{
    internal class Tess : MonoBehaviour
    {
        private static string AuthorName = "Tess";
        private static GameObject _extraTextObj = null;
        internal static GameObject extraTextObj
        {
            get
            {
                if (_extraTextObj != null) { return _extraTextObj; }

                _extraTextObj = new GameObject("ExtraCardText", typeof(TextMeshProUGUI), typeof(DestroyOnUnparent));
                DontDestroyOnLoad(_extraTextObj);
                return _extraTextObj;


            }
            private set { }
        }

        private void Start()
        {
            // add extra text to bottom right
            // create blank object for text, and attach it to the canvas
            // find bottom right edge object
            RectTransform[] allChildrenRecursive = this.gameObject.GetComponentsInChildren<RectTransform>();
            GameObject BottomLeftCorner = allChildrenRecursive.Where(obj => obj.gameObject.name == "EdgePart (1)").FirstOrDefault().gameObject;
            GameObject modNameObj = UnityEngine.GameObject.Instantiate(extraTextObj, BottomLeftCorner.transform.position, BottomLeftCorner.transform.rotation, BottomLeftCorner.transform);
            TextMeshProUGUI modText = modNameObj.gameObject.GetComponent<TextMeshProUGUI>();
            modText.text = AuthorName;
            modText.enableWordWrapping = false;
            modNameObj.transform.Rotate(0f, 0f, 135f);
            modNameObj.transform.localScale = new Vector3(1f, 1f, 1f);
            modNameObj.transform.localPosition = new Vector3(-50f, -50f, 0f);
            modText.alignment = TextAlignmentOptions.Bottom;
            modText.alpha = 0.1f;
            modText.fontSize = 50;
        }
    }

}
