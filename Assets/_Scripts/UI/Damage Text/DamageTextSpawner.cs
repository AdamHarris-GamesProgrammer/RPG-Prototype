using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.UI
{
    public class DamageTextSpawner : MonoBehaviour
    {
        [Tooltip("The Damage Text Prefab.")]
        [SerializeField] DamageText _damageText = null;

        public void Spawn(float damageIn)
        {
            //Spawns a damage text instance with this game object as the parent
            DamageText textInstance = Instantiate(_damageText, transform);
            textInstance.damageText.text = (damageIn).ToString("0.00");
        }
    }
}
