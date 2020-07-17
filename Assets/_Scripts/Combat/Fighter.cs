using RPG.Core;
using RPG.Movement;
using RPG.Saving;
using RPG.Resources;
using UnityEngine;

namespace RPG.Combat
{
    public class Fighter : MonoBehaviour, IAction, ISaveable
    {
        [SerializeField] private float timeBetweenAttacks = 1.0f;

        [SerializeField] private Transform leftHandTransform = null;
        [SerializeField] private Transform rightHandTransform = null;
        [SerializeField] private Weapon defaultWeapon = null;
        [SerializeField] string defaultWeaponName = "Unarmed";
        private Weapon equippedWeapon = null;


        [HideInInspector] public Transform target;

        float timeSinceLastAttack = 5.0f;

        private Mover fighterMover;

        private void Awake()
        {
            fighterMover = GetComponent<Mover>();

            if (equippedWeapon == null)
            {
                EquipWeapon(defaultWeapon);

            }
        }

        void Start()
        {

        }


        private void Update()
        {
            if (GetComponent<Health>().isDead) return;

            timeSinceLastAttack += Time.deltaTime;

            //if the target has not been assigned then return
            if (target == null) return;

            //checks if the target is in range
            bool inRange = IsInRange();

            //if the target is not in range
            if (!inRange)
            {
                //then move to the target
                //TODO: enemy runs if player is further away
                fighterMover.MoveTo(target.position, 1.0f, false);
            }
            else
            {
                //cancel the mover
                fighterMover.Cancel();

                //if the time since last attacked is greater than the cooldown
                if (timeSinceLastAttack >= timeBetweenAttacks)
                {
                    //then attack
                    AttackBehaviour();
                    timeSinceLastAttack = 0.0f;
                }
            }
        }

        private bool IsInRange()
        {
            return Vector3.Distance(transform.position, target.position) < equippedWeapon.AttackRange;
        }

        private void AttackBehaviour()
        {
            GetComponent<Animator>().SetTrigger("attack");
            transform.LookAt(target);
        }

        public void Attack(GameObject combatTarget)
        {
            GetComponent<ActionScheduler>().StartAction(this);

            //Debug.Log("Take that, Tarquin!");
            target = combatTarget.transform;
        }

        public void Cancel()
        {
            target = null;
            GetComponent<Mover>().Cancel();
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
            equippedWeapon = weapon;
            equippedWeapon.Spawn(rightHandTransform, leftHandTransform, GetComponent<Animator>());
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
                        //Deals damage
                        enemyHealthComponent.TakeDamage(gameObject, equippedWeapon.WeaponDamage);
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

        public object CaptureState()
        {
            return equippedWeapon.name;
        }

        public void RestoreState(object state)
        {
            string weaponName = (string)state;
            Weapon weapon = UnityEngine.Resources.Load<Weapon>(weaponName);
            EquipWeapon(weapon);
        }
    }
}

