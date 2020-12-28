using UnityEngine;
using RPG.Resources;
using RPG.Inventories;
using RPG.Stats;
using System.Collections.Generic;

namespace RPG.Combat 
{
    [CreateAssetMenu(menuName = "RPG/Make New Weapon")]
    public class WeaponConfig : EquipableItem, IModifierProvider
    {
        [Header("Animator Settings")]
        public AnimatorOverrideController weaponOverride = null;

        [Header("Prefab Settings")]
        public Weapon weaponPrefab = null;
        [SerializeField] private Projectile projectile = null;

        [Header("Damage Settings")]
        [Min(0f)][SerializeField] private float mininumDamage = 8.0f;
        [Min(1f)][SerializeField] private float maxinumDamage = 12.0f;
        [Min(0f)][SerializeField] private float percentageBonus = 0f;


        [Header("Critical Settings")]
        [Min(0f)][SerializeField] float criticalChance = 0.05f;
        [Tooltip("This is a percentage boost to damage, 80 means there will be an additional 80% damage when theres a crit")]
        [Min(0f)][SerializeField] float criticalDamage = 80.0f;

        [Header("General Settings")]
        [Min(0f)][SerializeField] private float attackRange = 2.5f;
        [Min(0f)][SerializeField] private float timeBetweenAttacks = 1.0f;

        [SerializeField] bool isRightHanded = true;
        [SerializeField] bool hasHeavyAttack = true;

        const string weaponName = "Weapon";

        public float AttackRange { get { return attackRange; } }
        public float AttackTime { get { return timeBetweenAttacks; } }

        public float PercentageBonus { get { return percentageBonus; } }

        public bool HasHeavyAttack { get { return hasHeavyAttack; } }

        public Weapon Spawn(Transform rightHand, Transform leftHand, Animator animator)
        {
            DestroyOldWeapon(rightHand, leftHand);

            Weapon weapon = null;

            if(weaponPrefab != null)
            {
                Transform weaponPlacement = GetTransform(rightHand, leftHand);

                weapon = Instantiate(weaponPrefab, weaponPlacement);
                weapon.gameObject.name = weaponName;
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

            return weapon;
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
            //Finds the old weapon game objects
            Transform oldWeapon = rightHand.Find(weaponName);
            if(oldWeapon == null)
            {
                oldWeapon = leftHand.Find(weaponName);
                if (oldWeapon == null) return;
            }

            //renames the old weapon for debugging purposes
            oldWeapon.name = "DESTROYING WEAPON";

            //Destroys the old weapon's game object
            Destroy(oldWeapon.gameObject);
        }

        public override string GetDescription()
        {
            //Gets the base item description
            string desc = base.GetDescription();

            //Adds the weapons stats to the description
            desc += "\n" + GetStatDescription();

            return desc;
        }

        private string GetStatDescription()
        {
            //Base result
            string result = "";

            //Damage
            result += "Damage: " + mininumDamage + "-" + maxinumDamage + "\n";

            if(percentageBonus != 0.0f)
            {
                //Attack Bonus damage, if applicable
                result += "Attack Bonus: " + percentageBonus;
            }

            //Attack range and time between attacks
            result += "Attack Range: " + attackRange + "\n";
            result += "Time between Attacks: " + timeBetweenAttacks + "\n";


            if (hasHeavyAttack)
            {
                //Heavy attack bonus damage if applicable
                result += "Heavy Attack Bonus: " + "50%" + "\n";
            }

            //Crit chance and damage 
            result += "Critical Chance: " + criticalChance + "\n";
            result += "Critical Damage: " + criticalDamage + "\n";

            //Returns the full stat description
            return result;
        }

        public float CalculateDamage()
        {
            //Calculates the damage between min and max values
            float damage = UnityEngine.Random.Range(mininumDamage, maxinumDamage);

            //Is it a critical hit
            bool isCritHit = UnityEngine.Random.Range(0.0f, 1.0f) < criticalChance;

            if (isCritHit)
            {
                //Add the crit damage as percentage
                damage += (damage / 100) * criticalDamage;
            }

            return damage;
        }

        public IEnumerable<float> GetAdditiveModifiers(Stat stat)
        {
            if(stat == Stat.Damage)
            {
                yield return CalculateDamage();
            }
        }

        public IEnumerable<float> GetPercentageModifiers(Stat stat)
        {
            if(stat == Stat.Damage)
            {
                yield return percentageBonus;
            }
        }
    }

}

