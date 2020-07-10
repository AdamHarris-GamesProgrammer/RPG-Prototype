using RPG.Core;
using RPG.Movement;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Combat
{
    public class Fighter : MonoBehaviour, IAction
    {
        [SerializeField] private float timeBetweenAttacks = 1.0f;

        [SerializeField] private Transform leftHandTransform = null;
        [SerializeField] private Transform rightHandTransform = null;
        [SerializeField] private Weapon defaultWeapon = null;
        private Weapon equippedWeapon = null;


        [HideInInspector] public Transform target;

        float timeSinceLastAttack = 5.0f;

        private Mover fighterMover;

        private void Awake()
        {
            fighterMover = GetComponent<Mover>();
        }

        void Start()
        {
            EquipWeapon(defaultWeapon);
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
                fighterMover.MoveTo(target.position, 1.0f);
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

        //Animation Event
        void Hit()
        {
            if(target != null )
            {
                //Gets the health component from the target
                Health enemyHealthComponent = target.GetComponent<Health>();
                if (enemyHealthComponent) //if health component found
                {
                    //Deals damage
                    enemyHealthComponent.TakeDamage(equippedWeapon.WeaponDamage);


                    //if the enemy is dead cancel the fighter state
                    if (enemyHealthComponent.isDead)
                    {
                        Cancel();
                    }
                }
            }
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
    }
}

