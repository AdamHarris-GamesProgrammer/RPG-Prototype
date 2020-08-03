using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using RPG.Resources;

namespace RPG.Control
{
    public class NPCController : StateMachine
    {
        [Header("Patrolling Settings")]
        [SerializeField] PatrolPath patrolPath = null;
        [Range(0f, 1f)]
        [SerializeField] float patrollingSpeedFraction = 0.3f;
        [SerializeField] float waypointTolerance = 1.0f;
        [SerializeField] float waypointDwellTime = 3.5f;

        [Header("Suspicion Settings")]
        [SerializeField] float suspicionDuration = 10.0f;
        [SerializeField] float aggrevationDuration = 10.0f;

        [Header("Distance Settings")]
        [SerializeField] float chaseDistance = 15.0f;
        [SerializeField] float attackDistance = 5.0f;

        bool aggrevated = false;
        public bool Aggrevated { get { return aggrevated; } set { aggrevated = value; } }

        float aggrevationTimer = 0.0f;
        public float AggrevatedTimer { get { return aggrevationTimer; } set { aggrevationTimer = value; } }
        public float AggrevationDuration { get { return aggrevationDuration; } }

        private PlayerController player;

        [SerializeField] private List<NPCController> enemiesInScene = null;

        private void Awake()
        {
            GetComponent<Health>().OnDeath += RemoveAIFromGameSpace;
            GetComponent<Health>().OnHealthChanged += Aggrevate;
        }

        protected override void Initialize()
        {
            if (GetComponent<Health>().isDead) return;

            GameObject playerGO = GameObject.FindGameObjectWithTag("Player");
            playerTransform = playerGO.transform;
            player = playerGO.GetComponent<PlayerController>();

            

            foreach (NPCController aiController in FindObjectsOfType<NPCController>())
            {
                if (!aiController.GetComponent<Health>().isDead)
                {
                    enemiesInScene.Add(aiController);
                }
            }

            ConstructStateMachine();
        }

        private void RemoveAIFromGameSpace()
        {
            player.GetComponent<PlayerController>().RemoveAI(this);
        }

        private void Aggrevate()
        {
            if (aggrevated) return;

            aggrevated = true;

            foreach (NPCController controller in enemiesInScene)
            {
                if (controller.aggrevated) continue;

                if (Vector3.Distance(controller.transform.position, transform.position) > chaseDistance) continue;

                controller.Aggrevate();
            }
        }

        protected override void StateUpdate()
        {

        }

        protected override void StateFixedUpdate()
        {
            CurrentState.Reason(playerTransform, transform);
            CurrentState.Act(playerTransform, transform);
        }

        public void SetTransition(Transition t)
        {
            PerformTransition(t);
        }

        private void ConstructStateMachine()
        {
            Patrol patrol;

            if(patrolPath != null)
            {
                patrol = new Patrol(this, patrolPath, chaseDistance, patrollingSpeedFraction, waypointTolerance);
            }
            else
            {
                patrol = new Patrol(this, false, chaseDistance, transform.position);
            }
            
            patrol.AddTransition(Transition.AtWaypoint, StateID.Guard);
            patrol.AddTransition(Transition.PlayerInChaseDistance, StateID.Chase);
            patrol.AddTransition(Transition.Aggrevated, StateID.Chase);

            Guard guard = new Guard(this, waypointDwellTime, chaseDistance);
            guard.AddTransition(Transition.WaitTimeOver, StateID.Patrol);
            guard.AddTransition(Transition.PlayerInChaseDistance, StateID.Chase);
            guard.AddTransition(Transition.Aggrevated, StateID.Chase);

            Chase chase = new Chase(this, chaseDistance, attackDistance);
            chase.AddTransition(Transition.PlayerInAttackRange, StateID.Attack);
            chase.AddTransition(Transition.PlayerLeavesChaseDistance, StateID.Suspicion);
            chase.AddTransition(Transition.Deaggrevated, StateID.Patrol);

            Suspicion suspicion = new Suspicion(this, suspicionDuration, chaseDistance);
            suspicion.AddTransition(Transition.SuspicionTimeUp, StateID.Patrol);
            suspicion.AddTransition(Transition.PlayerInChaseDistance, StateID.Chase);

            Attack attack = new Attack(this, attackDistance);
            attack.AddTransition(Transition.PlayerLeavesAttackRange, StateID.Chase);

            Dead dead = new Dead(this);

            AddState(patrol);
            AddState(guard);
            AddState(chase);
            AddState(suspicion);
            AddState(attack);
            AddState(dead);
        }

        //Called by Unity Editor
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackDistance);

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, chaseDistance);
        }

    }
}

