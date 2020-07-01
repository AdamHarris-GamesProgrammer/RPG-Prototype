using RPG.Combat;
using RPG.Core;
using RPG.Movement;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Control
{
    public class AIController : MonoBehaviour
    {
        [SerializeField] float chaseDistance = 5.0f;
        [SerializeField] float suspiscionDuration = 5.0f;
        [SerializeField] PatrolPath patrolPath;
        [SerializeField] float waypointTolerance = 1.0f;
        [SerializeField] float dwellTime = 3.5f;
        [SerializeField] float warningRadius = 12.0f;
        [SerializeField] float maxDistanceFromPoint = 25.0f;
        int currentWaypoint = 0;

        private GameObject player;
        private Fighter fighter;

        Vector3 guardPosition;
        float timeSinceLastSeenPlayer = Mathf.Infinity;
        float timeAtCurrentWaypoint = Mathf.Infinity;


        private void Awake()
        {
            player = GameObject.FindGameObjectWithTag("Player");

            fighter = GetComponent<Fighter>();

            guardPosition = transform.position;
        }

        private void Update()
        {
            if (GetComponent<Health>().isDead) return;

            if (IsPlayerInRange(chaseDistance) && fighter.CanAttack(player))
            {
                AttackState();
            }
            else if (IsPlayerInRange(warningRadius))
            {
                WarningState();
            }
            else if (timeSinceLastSeenPlayer < suspiscionDuration)
            {
                SuspicionState();
            }
            else
            {
                PatrolState();
            }

            timeSinceLastSeenPlayer += Time.deltaTime;
        }

        private static void WarningState()
        {
            Debug.Log("Im warning you, back off!");
        }

        private void PatrolState()
        {
            Vector3 nextPosition = guardPosition;

            if(patrolPath != null)
            {
                if (AtWaypoint())
                {
                    timeAtCurrentWaypoint += Time.deltaTime;

                    if(timeAtCurrentWaypoint >= dwellTime)
                    {
                        timeAtCurrentWaypoint = 0.0f;

                        currentWaypoint++;
                        currentWaypoint = patrolPath.CycleWaypoint(currentWaypoint);
                    }

                    
                }
                //Set next position to current waypoint
                nextPosition = patrolPath.GetWaypointPosition(currentWaypoint);
            }

            GetComponent<Mover>().StartMoveAction(nextPosition);
            
        }

        private void SuspicionState()
        {
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }

        private void AttackState()
        {
            timeSinceLastSeenPlayer = 0.0f;
            fighter.Attack(player);
        }

        private bool IsPlayerInRange(float distance)
        {
            return Vector3.Distance(player.transform.position, transform.position) < distance;
        }

        //Called by Unity Editor
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, chaseDistance);

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, warningRadius);
        }

        private bool AtWaypoint()
        {
            float distanceToWaypoint = Vector3.Distance(transform.position, patrolPath.GetWaypointPosition(currentWaypoint));

            if(distanceToWaypoint <= waypointTolerance)
            {
                return true;
            }

            return false;
        }


    }
}

