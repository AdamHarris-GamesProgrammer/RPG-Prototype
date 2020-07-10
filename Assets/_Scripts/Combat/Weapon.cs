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

        [SerializeField] bool isRightHanded = true;

        public float WeaponDamage { get { return weaponDamage; } }
        public float AttackRange { get { return attackRange; } }

        public void Spawn(Transform rightHand, Transform leftHand, Animator animator)
        {
            if(weaponPrefab != null)
            {
                //spawn the weapon prefab
                if (isRightHanded)
                {
                    Instantiate(weaponPrefab, rightHand);
                }
                else
                {
                    Instantiate(weaponPrefab, leftHand);
                }
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

