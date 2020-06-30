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


        public Transform target;

        float timeSinceLastAttack = 0.0f;

        private Mover fighterMover;
        private NavMeshAgent fighterAgent;

        private void Awake()
        {
            fighterMover = GetComponent<Mover>();
            fighterAgent = GetComponent<NavMeshAgent>();
        }

        private void Update()
        {
            timeSinceLastAttack += Time.deltaTime;

            if (target == null) return;

            bool inRange = Vector3.Distance(transform.position, target.position) < attackRange;

            if (target != null && !inRange)
            {
                fighterMover.MoveTo(target.position);
            }
            else
            {
                fighterMover.Cancel();

                if(timeSinceLastAttack >= timeBetweenAttacks)
                {
                    AttackBehaviour();
                    timeSinceLastAttack = 0.0f;
                }
            }
        }

        private void AttackBehaviour()
        {
            GetComponent<Animator>().SetTrigger("attack");
            transform.LookAt(target);
        }

        public void Attack(CombatTarget combatTarget)
        {
            GetComponent<ActionScheduler>().StartAction(this);

            //Debug.Log("Take that, Tarquin!");
            target = combatTarget.transform;
        }

        public void Cancel()
        {
            target = null;
            GetComponent<Animator>().SetTrigger("stopAttack");
            //print("Canceling Fighter");
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

        public bool CanAttack()
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
    }
}

