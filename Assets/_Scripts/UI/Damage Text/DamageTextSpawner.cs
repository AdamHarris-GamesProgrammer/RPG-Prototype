using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.UI
{
    public class DamageTextSpawner : MonoBehaviour
    {
        [SerializeField] DamageText damageText = null;

        public void Spawn(float damageIn)
        {
            DamageText textInstance = Instantiate<DamageText>(damageText, transform);
            textInstance.damageText.text = ((int)damageIn).ToString();
            Debug.Log(damageIn);
        }
    }
}
