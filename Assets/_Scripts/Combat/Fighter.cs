using RPG.Core;
using RPG.Movement;
using RPG.Saving;
using RPG.Resources;

using UnityEngine;
using RPG.Stats;
using System.Collections.Generic;
using RPG.Inventories;

namespace RPG.Combat
{
    public class Fighter : MonoBehaviour, IAction, ISaveable
    {
        protected float timeBetweenAttacks = 1.0f;
        
        [SerializeField] private Transform leftHandTransform = null;
        [SerializeField] private Transform rightHandTransform = null;
        [SerializeField] private WeaponConfig defaultWeapon = null;

        protected WeaponConfig equippedWeaponConfig = null;
        private Weapon currentWeapon;

        protected Transform target;

        protected float timeSinceLastAttack = 5.0f;

        protected bool heavyAttack;

        protected Mover fighterMover;
        protected Health fighterHealth;
        protected BaseStats fighterStats;
        protected Experience fighterExperience;
        protected Animator fighterAnimator;
        protected Equipment fighterEquipment;

        private WeaponConfig unarmedConfig;

        private void Awake()
        {
            fighterMover = GetComponent<Mover>();
            fighterHealth = GetComponent<Health>();
            fighterStats = GetComponent<BaseStats>();
            fighterExperience = GetComponent<Experience>();
            fighterAnimator = GetComponent<Animator>();

            fighterEquipment = GetComponent<Equipment>();

            if (fighterEquipment)
            {
                fighterEquipment.EquipmentUpdated += UpdateWeapon;
            }

            unarmedConfig = UnityEngine.Resources.Load<WeaponConfig>("Unarmed");

            if(equippedWeaponConfig == null)
            {
                equippedWeaponConfig = unarmedConfig;
            }
        }

        public WeaponConfig GetWeaponConfig()
        {
            return equippedWeaponConfig;
        }

        void Start()
        {
            if (equippedWeaponConfig == null)
            {
                currentWeapon = EquipWeapon(defaultWeapon);
            }
        }

        protected virtual void Update()
        {
            UpdateTimers();

            Debug.DrawRay(transform.position, transform.forward * 1.5f, Color.red);
        }

        protected virtual void UpdateTimers()
        {
            timeSinceLastAttack += Time.deltaTime;
        }

        public bool IsInRangeOfWeapon(Vector3 target)
        {
            return Vector3.Distance(transform.position, target) < equippedWeaponConfig.AttackRange;
        }

        public virtual void Attack(GameObject combatTarget, bool isHeavyAttack)
        {
            GetComponent<ActionScheduler>().StartAction(this);

            target = combatTarget.transform;
            heavyAttack = isHeavyAttack;
        }

        public virtual void Attack(GameObject combatTarget)
        {
            GetComponent<ActionScheduler>().StartAction(this);

            target = combatTarget.transform;
        }

        public bool CanAttack(GameObject target)
        {
            //if the target does not exist and target is dead then return false
            if (target == null && target.GetComponent<Health>().isDead) return false;

            return true;
        }

        public Weapon EquipWeapon(WeaponConfig weapon)
        {
            if (!weapon) Debug.LogError("[Error]: Fighter.cs weapon is null");

            equippedWeaponConfig = weapon;
            timeBetweenAttacks = equippedWeaponConfig.AttackTime;
            Weapon spawnedWeapon = equippedWeaponConfig.Spawn(rightHandTransform, leftHandTransform, fighterAnimator);

            currentWeapon = spawnedWeapon;

            return spawnedWeapon;
        }

        private void UpdateWeapon()
        {
            //Gets the weapon in the weapon slot
            WeaponConfig weapon = fighterEquipment.GetItemInSlot(EquipLocation.Weapon) as WeaponConfig;
            
            //If a weapon is in the slot
            if(weapon)
            {
                //Equip the weapon
                EquipWeapon(weapon);
            }
            else
            {
                //if there is no weapon then equip unarmed.
                EquipWeapon(unarmedConfig);
                
            }
        }


        //Animation Event
        void Hit()
        {
            //if the target is null then return
            if (target == null) return;
            //if the target is not in range of the weapon then return
            if (!IsInRangeOfWeapon(target.position)) return;

            //Checks to see if the target is within 45 degrees of the forward.
            Vector3 rayDirection = target.position - transform.position;
            if (Vector3.Angle(rayDirection, transform.forward) > 45) return;

            //Gets the health component from the target
            Health enemyHealthComponent = target.GetComponent<Health>();

            //if the target does not have a health component
            if (!enemyHealthComponent) return;

            //if the enemy is dead cancel the fighter state
            if (enemyHealthComponent.isDead)
            {
                Cancel();
            }

            if(currentWeapon != null)
            {
                currentWeapon.OnHit();
            }

            //if the equipped weapon is a projectile based weapon
            if (equippedWeaponConfig.HasProjectile())
            {
                //Launch the projectile
                equippedWeaponConfig.LaunchProjectile(rightHandTransform, leftHandTransform, enemyHealthComponent, gameObject);
            }
            else
            {
                float damage = fighterStats.GetStat(Stat.Damage, fighterExperience.GetLevel());
                //Debug.Log(gameObject.name + " deals " + damage + " damage");

                if (heavyAttack) damage *= 1.5f;

                enemyHealthComponent.TakeDamage(gameObject, damage, heavyAttack);
            }
        }

        #region IAction
        public void Cancel()
        {
            target = null;

            if (gameObject.name == "Player") return;

            fighterMover.Cancel();
            //print(gameObject.name + " fighter canceling");
        }
        #endregion

        #region ISaveable
        //Implements ISaveable interface
        public object CaptureState()
        {
            return equippedWeaponConfig.name;
        }

        public void RestoreState(object state)
        {
            string weaponName = (string)state;

            WeaponConfig weapon = UnityEngine.Resources.Load<WeaponConfig>(weaponName);
            currentWeapon = EquipWeapon(weapon);
        }
        #endregion
    }
}