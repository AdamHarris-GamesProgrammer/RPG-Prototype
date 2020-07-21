using UnityEngine;
using RPG.Resources;

namespace RPG.Combat 
{
    [CreateAssetMenu(menuName = "RPG/Make New Weapon")]
    public class Weapon : ScriptableObject
    {
        [Header("Animator Settings")]
        public AnimatorOverrideController weaponOverride = null;

        [Header("Prefab Settings")]
        public GameObject weaponPrefab = null;
        [SerializeField] private Projectile projectile = null;

        [Header("Damage Settings")]
        [SerializeField] private float mininumDamage = 8.0f;
        [SerializeField] private float maxinumDamage = 12.0f;
        [SerializeField] private float percentageBonus = 0f;


        [Header("Critical Settings")]
        [SerializeField] float criticalChance = 0.05f;
        [Tooltip("This is a percentage boost to damage, 80 means there will be an additional 80% damage when theres a crit")]
        [SerializeField] float criticalDamage = 80.0f;

        [Header("General Settings")]
        [SerializeField] private float attackRange = 2.5f;
        [SerializeField] private float timeBetweenAttacks = 1.0f;

        [SerializeField] bool isRightHanded = true;

        const string weaponName = "Weapon";

        public float AttackRange { get { return attackRange; } }
        public float AttackTime { get { return timeBetweenAttacks; } }

        public float PercentageBonus { get { return percentageBonus; } }

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

        public void LaunchProjectile(Transform rightHand, Transform leftHand, Health target, GameObject instigator)
        {
            //Creates a projectile instance and sets the target
            Projectile projectileInstance = Instantiate(projectile, GetTransform(rightHand, leftHand).position, Quaternion.identity);
            projectileInstance.SetTarget(target, instigator, CalculateDamage());
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

            //Destroys the old weapon's game object
            Destroy(oldWeapon.gameObject);
        }



        public float CalculateDamage()
        {
            float damage = UnityEngine.Random.Range(mininumDamage, maxinumDamage);

            bool isCritHit = UnityEngine.Random.Range(0.0f, 1.0f) < criticalChance;

            if (isCritHit)
            {
                damage += (damage / 100) * criticalDamage;
            }

            return damage;
        }
    }

}

