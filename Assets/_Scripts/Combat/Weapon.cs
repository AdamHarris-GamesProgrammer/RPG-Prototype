using RPG.Core;
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
        [SerializeField] private Projectile projectile = null;

        [SerializeField] bool isRightHanded = true;

        const string weaponName = "Weapon";

        public float WeaponDamage { get { return weaponDamage; } }
        public float AttackRange { get { return attackRange; } }

        public void Spawn(Transform rightHand, Transform leftHand, Animator animator)
        {
            DestroyOldWeapon(rightHand, leftHand);

            if(weaponPrefab != null)
            {
                Transform weaponPlacement = GetTransform(rightHand, leftHand);

                GameObject weapon = Instantiate(weaponPrefab, weaponPlacement);

                weapon.name = weaponName;
            }

            if (weaponOverride != null)
            {
                if(animator != null)
                {
                    //override the animator controller with the animation controller for the weapon
                    animator.runtimeAnimatorController = weaponOverride;
                }
                else
                {
                    var overrideController = animator.runtimeAnimatorController as AnimatorOverrideController;

                    if (overrideController != null) animator.runtimeAnimatorController = weaponOverride.runtimeAnimatorController;
                }
            }
        }

        private Transform GetTransform(Transform rightHand, Transform leftHand)
        {
            Transform weaponPlacement = leftHand;
            if (isRightHanded) weaponPlacement = rightHand;
            return weaponPlacement;
        }

        public void LaunchProjectile(Transform rightHand, Transform leftHand, Health target)
        {
            //Creates a projectile instance and sets the target
            Projectile projectileInstance = Instantiate(projectile, GetTransform(rightHand, leftHand).position, Quaternion.identity);
            projectileInstance.SetTarget(target, weaponDamage);
        }

        public bool HasProjectile()
        {
            return projectile != null;
        }

        private void DestroyOldWeapon(Transform rightHand, Transform leftHand)
        {
            Transform oldWeapon = rightHand.Find(weaponName);
            if(oldWeapon == null)
            {
                oldWeapon = leftHand.Find(weaponName);
            }
            if (oldWeapon == null) return;

            //renames the old weapon for debugging purposes
            oldWeapon.name = "DESTROYING WEAPON";

            //Destrys the old weapon's game object
            Destroy(oldWeapon.gameObject);
        }
    }

}

