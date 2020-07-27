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

        [HideInInspector] public Transform target;

        protected float timeSinceLastAttack = 5.0f;

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

            if (fighterHealth.isDead || target == null) return;

            //checks if the target is in range
            bool inRange = IsInRange(target.position);

            //if the target is not in range
            if (inRange)
            {
                //if the time since last attacked is greater than the cooldown
                if (timeSinceLastAttack >= timeBetweenAttacks)
                {
                    //then attack
                    AttackBehaviour();
                    timeSinceLastAttack = 0.0f;
                }
            }
        }

        protected virtual void UpdateTimers()
        {
            timeSinceLastAttack += Time.deltaTime;
        }

        protected bool SafetyChecks()
        {
            bool safe = false;

            if (fighterHealth.isDead || target == null) safe = false;

            return safe;
        }

        public bool IsInRange(Vector3 target)
        {
            return Vector3.Distance(transform.position, target) < equippedWeapon.AttackRange;
        }

        private void AttackBehaviour()
        {
            fighterAnimator.SetTrigger("attack");
            transform.LookAt(target);
        }

        public virtual void Attack(GameObject combatTarget)
        {
            GetComponent<ActionScheduler>().StartAction(this);

            //Debug.Log("Take that, Tarquin!");
            target = combatTarget.transform;
        }

        public void Cancel()
        {
            target = null;

            if (gameObject.name == "Player") return;

            fighterMover.Cancel();
            //print(gameObject.name + " fighter canceling");
        }

        public bool CanAttack(GameObject target)
        {
            //if the target does not exist and target is dead then return false
            if (target == null && target.GetComponent<Health>().isDead) return false;

            return true;
        }
        
        public void EquipWeapon(Weapon weapon)
        {
            if(weapon == null)
            {
                Debug.LogError("[Error]: Fighter.cs weapon is null");
            }

            equippedWeapon = weapon;
            equippedWeapon.Spawn(rightHandTransform, leftHandTransform, fighterAnimator);
            timeBetweenAttacks = equippedWeapon.AttackTime;
            //equippedWeaponName = equippedWeapon.name;
        }

        //Animation Event
        void Hit()
        {
            if (target != null)
            {
                //Gets the health component from the target
                Health enemyHealthComponent = target.GetComponent<Health>();

                if (enemyHealthComponent) //if health component found
                {
                    if (equippedWeapon.HasProjectile())
                    {
                        equippedWeapon.LaunchProjectile(rightHandTransform, leftHandTransform, enemyHealthComponent, gameObject);
                    }
                    else
                    {
                        float damage = fighterStats.GetStat(Stat.Damage, fighterExperience.GetLevel());
                        //Debug.Log(gameObject.name + " deals " + damage + " damage");
                        //Deals damage
                        enemyHealthComponent.TakeDamage(gameObject, damage);
                    }

                    //if the enemy is dead cancel the fighter state
                    if (enemyHealthComponent.isDead)
                    {
                        Cancel();
                    }
                }
            }
        }

        //Animation Event
        void Shoot()
        {
            Hit();
        }

        //Implements ISaveable interface
        public object CaptureState()
        {
            if(gameObject.name == "Player")
            {
                Debug.Log("Weapon name upon saving is: " + equippedWeapon.name);
            }
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
    }
}

