using RPG.Core;
using RPG.Movement;
using RPG.Saving;
using RPG.Resources;

using UnityEngine;
using RPG.Stats;
using System.Collections.Generic;

namespace RPG.Combat
{
    public class Fighter : MonoBehaviour, IAction, ISaveable, IModifierProvider
    {
        protected float timeBetweenAttacks = 1.0f;

        [SerializeField] private Transform leftHandTransform = null;
        [SerializeField] private Transform rightHandTransform = null;
        [SerializeField] private Weapon defaultWeapon = null;

        private Weapon equippedWeapon = null;

        protected Transform target;

        protected float timeSinceLastAttack = 5.0f;

        protected bool heavyAttack;

        protected Mover fighterMover;
        protected Health fighterHealth;
        protected BaseStats fighterStats;
        protected Experience fighterExperience;
        protected Animator fighterAnimator;

        private void Awake()
        {
            fighterMover = GetComponent<Mover>();
            fighterHealth = GetComponent<Health>();
            fighterStats = GetComponent<BaseStats>();
            fighterExperience = GetComponent<Experience>();
            fighterAnimator = GetComponent<Animator>();
        }

        void Start()
        {
            if (equippedWeapon == null)
            {
                EquipWeapon(defaultWeapon);
            }
        }

        protected virtual void Update()
        {
            UpdateTimers();
        }

        protected virtual void UpdateTimers()
        {
            timeSinceLastAttack += Time.deltaTime;
        }

        public bool IsInRangeOfWeapon(Vector3 target)
        {
            return Vector3.Distance(transform.position, target) < equippedWeapon.AttackRange;
        }

        protected void AttackBehaviour()
        {
            if (heavyAttack && equippedWeapon.HasHeavyAttack)
            {
                fighterAnimator.SetTrigger("heavyAttack");
            }
            else
            {
                fighterAnimator.SetTrigger("lightAttack");
            }
            transform.LookAt(target);
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
        
        public void EquipWeapon(Weapon weapon)
        {
            if(!weapon) Debug.LogError("[Error]: Fighter.cs weapon is null");

            equippedWeapon = weapon;
            equippedWeapon.Spawn(rightHandTransform, leftHandTransform, fighterAnimator);
            timeBetweenAttacks = equippedWeapon.AttackTime;
        }

        //Animation Event
        void Hit()
        {
            //if the target is null then return
            if (target == null) return;
            //if the target is not in range of the weapon then return
            if (!IsInRangeOfWeapon(target.position)) return;

            //Gets the health component from the target
            Health enemyHealthComponent = target.GetComponent<Health>();

            //if the target does not have a health component
            if (!enemyHealthComponent) return;

            //if the enemy is dead cancel the fighter state
            if (enemyHealthComponent.isDead)
            {
                Cancel();
            }

            //if the equipped weapon is a projectile based weapon
            if (equippedWeapon.HasProjectile())
            {
                //Launch the projectile
                equippedWeapon.LaunchProjectile(rightHandTransform, leftHandTransform, enemyHealthComponent, gameObject);
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
            return equippedWeapon.name;
        }

        public void RestoreState(object state)
        {
            string weaponName = (string)state;

            if (gameObject.name == "Player")
            {
                Debug.Log("Weapon name upon loading is: " + weaponName);
            }

            Weapon weapon = UnityEngine.Resources.Load<Weapon>(weaponName);
            EquipWeapon(weapon);
        }
        #endregion

        #region IModifierProvider
        //Implements IModifierProvider interface
        public IEnumerable<float> GetAdditiveModifiers(Stat stat)
        {
            if(stat == Stat.Damage)
            {
                yield return equippedWeapon.CalculateDamage();
            }
        }

        public IEnumerable<float> GetPercentageModifiers(Stat stat)
        {
            if(stat == Stat.Damage)
            {
                yield return equippedWeapon.PercentageBonus;
            }
        }
        #endregion

    }
}