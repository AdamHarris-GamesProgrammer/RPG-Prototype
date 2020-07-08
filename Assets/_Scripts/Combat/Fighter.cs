using RPG.Core;
using RPG.Movement;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Combat
{
    public class Fighter : MonoBehaviour, IAction
    {
        [SerializeField] private float attackRange = 2.5f;
        [SerializeField] private float timeBetweenAttacks = 1.0f;
        [SerializeField] private float weaponDamage = 5.0f;
        [SerializeField] private GameObject weaponPrefab = null;
        [SerializeField] private Transform handTransform = null;

        [SerializeField] private AnimatorOverrideController weaponOverride = null;


        public Transform target;

        float timeSinceLastAttack = 5.0f;

        private Mover fighterMover;
        private NavMeshAgent fighterAgent;

        private void Awake()
        {
            fighterMover = GetComponent<Mover>();
            fighterAgent = GetComponent<NavMeshAgent>();
        }

        void Start()
        {
            SpawnWeapon();
        }


        private void Update()
        {
            if (GetComponent<Health>().isDead) return;

            timeSinceLastAttack += Time.deltaTime;

            if (target == null) return;
            bool inRange = IsInRange(target);

            if (!inRange)
            {
                fighterMover.MoveTo(target.position, 1.0f);
            }
            else
            {
                fighterMover.Cancel();

                if (timeSinceLastAttack >= timeBetweenAttacks)
                {
                    AttackBehaviour();
                    timeSinceLastAttack = 0.0f;
                }
            }
        }
        void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, attackRange);
        }

        private bool IsInRange(Transform target)
        {
            return Vector3.Distance(transform.position, target.position) < attackRange;
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
            print(gameObject.name + " fighter canceling");
        }

        //Animation Event
        void Hit()
        {
            if(target != null )
            {
                Health enemyHealthComponent = target.GetComponent<Health>();
                if (enemyHealthComponent)
                {
                    enemyHealthComponent.TakeDamage(weaponDamage);

                    if (enemyHealthComponent.isDead)
                    {
                        Cancel();
                    }
                }
            }
        }

        public bool CanAttack(GameObject target)
        {
            if(target != null && !target.GetComponent<Health>().isDead)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private void SpawnWeapon()
        {
            if (weaponPrefab != null)
            {
                Instantiate(weaponPrefab, handTransform);
                GetComponent<Animator>().runtimeAnimatorController = weaponOverride;
            }
        }
    }
}

