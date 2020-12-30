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
        //STATE
        const string _weaponName = "Weapon";

        //EDITOR PROPERTIES
        [Header("Animator Settings")]
        [Tooltip("Overrides the default animation controller, to be specific for this weapon.")]
        [SerializeField] private AnimatorOverrideController _weaponOverride = null;

        [Header("Prefab Settings")]
        [Tooltip("Sets the prefab for the weapon, this is the model that the player will hold.")]
        [SerializeField] private Weapon _weaponPrefab = null;
        [Tooltip("Projectile prefab for the weapon. If this is set then the weapon is assumed to be a ranged weapon, if not then it is assumed to be a melee weapon.")]
        [SerializeField] private Projectile _projectile = null;

        [Header("Damage Settings")]
        [Tooltip("The minimum amount of damage that the weapon can do.")]
        [Min(0f)][SerializeField] private float _mininumDamage = 8.0f;
        [Tooltip("The maximum amount of damage that the weapon can do")]
        [Min(1f)][SerializeField] private float _maxinumDamage = 12.0f;
        [Tooltip("The damage bonus that the weapon can do.")]
        [Min(0f)][SerializeField] private float _percentageBonus = 0f;


        [Header("Critical Settings")]
        [Tooltip("This is the chance to cause a critical hit, 0.05 would mean that a critical hit can happen 5% of the time.")]
        [Range(0f, 1f)][SerializeField] float _criticalChance = 0.05f;
        [Tooltip("This is a percentage boost to damage, 80 means there will be an additional 80% damage when theres a crit.")]
        [Min(0f)][SerializeField] float _criticalDamage = 80.0f;

        [Header("General Settings")]
        [Tooltip("The range that the weapon can attack from.")]
        [Min(0f)][SerializeField] private float _attackRange = 2.5f;
        [Tooltip("The time between attacks, stops the player from attacking every frame.")]
        [Min(0.1f)][SerializeField] private float _timeBetweenAttacks = 1.0f;
        [Tooltip("If the weapon is right or left handed. All bows are left handed by default.")]
        [SerializeField] bool _isRightHanded = true;
        [Tooltip("Does the weapon have a heavy attack available to use.")]
        [SerializeField] bool _hasHeavyAttack = true;

        //PUBLIC PROPERTIES
        public float AttackRange { get { return _attackRange; } }

        public float AttackTime { get { return _timeBetweenAttacks; } }

        public float PercentageBonus { get { return _percentageBonus; } }

        public bool HasHeavyAttack { get { return _hasHeavyAttack; } }



        //OVERRIDE METHODS

        /// <summary>
        /// Gets the Items description. Used in item tooltip system.
        /// </summary>
        /// <returns>Returns the string that contains the items description and stats</returns>
        public override string GetDescription()
        {
            //Gets the base item description
            string desc = base.GetDescription();

            //Adds the weapons stats to the description
            desc += "\n" + GetStatDescription();

            return desc;
        }

        //PUBLIC METHODS

        /// <summary>
        /// Spawns the current weapon into the actors hands.
        /// </summary>
        /// <param name="rightHand">The actors right hand transform.</param>
        /// <param name="leftHand">The actors left hand transform.</param>
        /// <param name="animator">The actors animator.</param>
        /// <returns>Returns a Weapon object that is used in attacking.</returns>
        public Weapon Spawn(Transform rightHand, Transform leftHand, Animator animator)
        {
            //Destroys the old weapon that the player has equipped
            DestroyOldWeapon(rightHand, leftHand);

            //Weapon that is being spawned
            Weapon weapon = null;

            //Safety Check
            if(_weaponPrefab != null)
            {
                //Gets the transform that the weapon needs to go into
                Transform weaponPlacement = GetHandTransform(rightHand, leftHand);

                //Instantiates the weapon in the respective 
                weapon = Instantiate(_weaponPrefab, weaponPlacement);

                //Sets the name equal to the name of the weapon
                weapon.gameObject.name = _weaponName;
            }

            //Safety check for the weapon animator override
            if (_weaponOverride != null)
            {
                //Safety check for the passed in animator
                if(animator != null)
                {
                    //override the animator controller with the animation controller for the weapon
                    animator.runtimeAnimatorController = _weaponOverride;
                }
                else
                {
                    var overrideController = animator.runtimeAnimatorController as AnimatorOverrideController;

                    if (overrideController != null) animator.runtimeAnimatorController = _weaponOverride.runtimeAnimatorController;
                }
            }

            //Returns the created weapon object
            return weapon;
        }

 
        /// <summary>
        /// Launches a Projectile instance from the weapons equipped hand.
        /// </summary>
        /// <param name="rightHand">The actors right hand transform.</param>
        /// <param name="leftHand">The actors left hand transform.</param>
        /// <param name="target">The target that the actor is attacking.</param>
        /// <param name="instigator">The actor themselves.</param>
        public void LaunchProjectile(Transform rightHand, Transform leftHand, Health target, GameObject instigator)
        {
            //Creates a projectile instance and sets the target
            Projectile projectileInstance = Instantiate(_projectile, GetHandTransform(rightHand, leftHand).position, Quaternion.identity);
            projectileInstance.SetTarget(target, instigator, CalculateDamage());
        }

        /// <summary>
        /// Checks to see if the weapon has a projectile.
        /// </summary>
        /// <returns>Returns true if a projectile is available, returns false if not.</returns>
        public bool HasProjectile()
        {
            return _projectile != null;
        }


        /// <summary>
        /// Calculates the damage a attack will do based off its minimum and maximum damage
        /// </summary>
        /// <returns></returns>
        public float CalculateDamage()
        {
            //Calculates the damage between min and max values
            float damage = Random.Range(_mininumDamage, _maxinumDamage);

            //Checks if it is a critical hit
            if (Random.Range(0.0f, 1.0f) < _criticalChance)
            {
                //Add the crit damage as percentage
                damage += (damage / 100) * _criticalDamage;
            }

            //Returns damage 
            return damage;
        }

        //PRIVATE METHODS

        /// <summary>
        /// Gets the hand that the weapon will go in.
        /// </summary>
        /// <param name="rightHand">The actors right hand transform.</param>
        /// <param name="leftHand">The actors left hand transform.</param>
        /// <returns>Returns the transform of the hand the weapon will spawn in.</returns>
        private Transform GetHandTransform(Transform rightHand, Transform leftHand)
        {
            Transform weaponPlacement = leftHand;
            if (_isRightHanded) weaponPlacement = rightHand;
            return weaponPlacement;
        }

        /// <summary>
        /// Destroys the old weapon if one was equipped.
        /// </summary>
        /// <param name="rightHand">The actors right hand transform.</param>
        /// <param name="leftHand">The actors left hand transform.</param>
        private void DestroyOldWeapon(Transform rightHand, Transform leftHand)
        {
            //Finds the old weapon game objects
            Transform oldWeapon = rightHand.Find(_weaponName);
            if (oldWeapon == null)
            {
                oldWeapon = leftHand.Find(_weaponName);
                if (oldWeapon == null) return;
            }

            //renames the old weapon for debugging purposes
            oldWeapon.name = "DESTROYING WEAPON";

            //Destroys the old weapon's game object
            Destroy(oldWeapon.gameObject);
        }
        
        /// <summary>
        /// Gets the item stats in the form of a description.
        /// </summary>
        /// <returns>Returns the items stats as a string.</returns>
        private string GetStatDescription()
        {
            //Base result
            string result = "";

            //Damage
            result += "Damage: " + _mininumDamage + "-" + _maxinumDamage + "\n";

            if (_percentageBonus != 0.0f)
            {
                //Attack Bonus damage, if applicable
                result += "Attack Bonus: " + _percentageBonus;
            }

            //Attack range and time between attacks
            result += "Attack Range: " + _attackRange + "\n";
            result += "Time between Attacks: " + _timeBetweenAttacks + "\n";


            if (_hasHeavyAttack)
            {
                //Heavy attack bonus damage if applicable
                result += "Heavy Attack Bonus: " + "50%" + "\n";
            }

            //Crit chance and damage 
            result += "Critical Chance: " + _criticalChance + "\n";
            result += "Critical Damage: " + _criticalDamage + "\n";

            //Returns the full stat description
            return result;
        }


        //Implements IModifierProvider Interface
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
                yield return _percentageBonus;
            }
        }
    }
}

