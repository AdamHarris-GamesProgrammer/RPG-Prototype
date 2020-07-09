using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Combat 
{
    [CreateAssetMenu(menuName = "RPG/Make New Weapon")]
    public class Weapon : ScriptableObject
    {
        public AnimatorOverrideController weaponOverride = null;
        public GameObject weaponPrefab = null;
        [SerializeField] private float weaponDamage = 5.0f;
        [SerializeField] private float attackRange = 2.5f;

        public float WeaponDamage { get { return weaponDamage; } }
        public float AttackRange { get { return attackRange; } }

        public void Spawn(Transform handTransform, Animator animator)
        {
            if(weaponPrefab != null)
            {
                //spawn the weapon prefab
                Instantiate(weaponPrefab, handTransform);
            }

            if(weaponOverride != null)
            {
                if(animator != null)
                {
                    //override the animator controller with the animation controller for the weapon
                    animator.runtimeAnimatorController = weaponOverride;
                }
            }
        }

    }

}

