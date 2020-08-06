using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.UI
{
    public class DamageText : MonoBehaviour
    {
        [HideInInspector] public Text damageText;

        private void Awake()
        {
            damageText = GetComponentInChildren<Text>();
            Destroy(gameObject, 1.5f);
        }
    }
}

