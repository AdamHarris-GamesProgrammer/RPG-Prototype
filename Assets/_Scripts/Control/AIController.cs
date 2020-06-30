using RPG.Combat;
using RPG.Core;
using RPG.Movement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Control
{
    public class AIController : MonoBehaviour
    {
        [SerializeField] float chaseDistance = 5.0f;
        [SerializeField] float attackRange = 1.5f;

        private NavMeshAgent agent;
        private GameObject player;
        private Fighter fighter;

        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();

            player = GameObject.FindGameObjectWithTag("Player");

            fighter = GetComponent<Fighter>();
        }

        private void Update()
        {
            if (GetComponent<Health>().isDead)
            {
                agent.isStopped = true;
                GetComponent<Fighter>().Cancel();
                return;
            }

            if (IsPlayerInRange(chaseDistance) && fighter.CanAttack(player))
            {
                fighter.Attack(player);
            }
            else
            {
                fighter.Cancel();
            }


        }

        private bool IsPlayerInRange(float distance)
        {
            return Vector3.Distance(player.transform.position, transform.position) < distance;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, chaseDistance);
        }

    }
}

