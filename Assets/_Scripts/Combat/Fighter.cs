using RPG.Core;
using RPG.Movement;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Combat
{
    public class Fighter : MonoBehaviour, IAction
    {
        [SerializeField] private float attackRange = 2.5f;

        public Transform target;

        private Mover fighterMover;
        private NavMeshAgent fighterAgent;

        private void Awake()
        {
            fighterMover = GetComponent<Mover>();
            fighterAgent = GetComponent<NavMeshAgent>();
        }

        private void Update()
        {
            if (target == null) return;

            bool inRange = Vector3.Distance(transform.position, target.position) < attackRange;

            if (target != null && !inRange)
            {
                fighterMover.MoveTo(target.position);
            }
            else
            {
                fighterMover.Cancel();
            }
        }

        public void Attack(CombatTarget combatTarget)
        {
            GetComponent<ActionScheduler>().StartAction(this);

            Debug.Log("Take that, Tarquin!");
            target = combatTarget.transform;
        }

        public void Cancel()
        {
            target = null;
        }
    }
}

