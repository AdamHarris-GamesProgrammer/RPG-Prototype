using RPG.Combat;
using RPG.Core;
using RPG.Movement;
using UnityEngine;
using RPG.Resources;
using System.Collections.Generic;

namespace RPG.Control
{
    public class AIController : MonoBehaviour
    {
        [Header("Distance Settings")]
        [SerializeField] float chaseDistance = 5.0f;
        [SerializeField] float warningRadius = 12.0f;

        [Header("Timer Settings")]
        [SerializeField] float suspiscionDuration = 5.0f;
        [SerializeField] float aggrevatedDuration = 15.0f;
        [SerializeField] float dwellTime = 3.5f;

        [Header("Patrolling Settings")]
        [SerializeField] PatrolPath patrolPath = null;
        [SerializeField] float waypointTolerance = 1.0f;

        [Header("Speed Settings")]
        [Range(0f,1f)]
        [SerializeField] float patrollingSpeedFraction = 0.2f;

        int currentWaypoint = 0;

        private GameObject player;
        private Fighter fighter;
        private Health health;

        private Health playerHealth;


        Vector3 guardPosition;
        float timeSinceLastSeenPlayer = Mathf.Infinity;
        float timeSinceAggrevated = Mathf.Infinity;
        float timeAtCurrentWaypoint = Mathf.Infinity;

        [SerializeField] private List<AIController> enemiesInScene;

        bool aggrevated = false;

        private void Awake()
        {
            player = GameObject.FindGameObjectWithTag("Player");
            playerHealth = player.GetComponent<Health>();

            fighter = GetComponent<Fighter>();

            health = GetComponent<Health>();

            guardPosition = transform.position;

            GetComponent<Health>().onHealthChanged += Aggrevate;
        }

        private void Start()
        {
            if (health.isDead) return;

            foreach(AIController aiController in FindObjectsOfType<AIController>())
            {
                if (!aiController.GetComponent<Health>().isDead)
                {
                    enemiesInScene.Add(aiController);
                }
            }
        }

        private void Update()
        {
            if (health.isDead) return;


            if (IsAggrevated() && fighter.CanAttack(player) && !playerHealth.isDead)
            {
                AttackState();
            }
            else if (IsPlayerInRange(warningRadius))
            {
                aggrevated = false;
                WarningState();
            }
            else if (timeSinceLastSeenPlayer < suspiscionDuration)
            {
                aggrevated = false;
                SuspicionState();
            }
            else
            {
                aggrevated = false;
                PatrolState();
            }

            timeSinceLastSeenPlayer += Time.deltaTime;
            timeSinceAggrevated += Time.deltaTime;
        }

        bool IsAggrevated()
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
            return distanceToPlayer < chaseDistance || timeSinceAggrevated < aggrevatedDuration ;
        }

        private static void WarningState()
        {
            //Debug.Log("Im warning you, back off!");
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
                //Set next position to current Waypoint
                nextPosition = patrolPath.GetWaypointPosition(currentWaypoint);
            }

            GetComponent<Mover>().StartMoveAction(nextPosition, patrollingSpeedFraction, false);
            
        }

        private void SuspicionState()
        {
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }

        private void AttackState()
        {
            Aggrevate();
            timeSinceLastSeenPlayer = 0.0f;
            fighter.Attack(player);
        }

        public void Aggrevate()
        {
            if (aggrevated) return;

            timeSinceAggrevated = 0.0f;

            aggrevated = true;

            foreach(AIController controller in enemiesInScene)
            {
                if (controller.aggrevated) continue;

                if (Vector3.Distance(controller.transform.position, transform.position) > warningRadius) continue;

                controller.Aggrevate();
            }
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

