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

        [Header("Attack settings")]
        [SerializeField] bool hasRangedAttack = false;

        public float ChaseDistance { get { return chaseDistance; } }
        public float AttackDistance { get { return attackDistance; } }

        public bool isDead = false;

        bool aggrevated = false;
        public bool Aggrevated { get { return aggrevated; } set { aggrevated = value; } }

        float aggrevationTimer = 0.0f;
        public float AggrevatedTimer { get { return aggrevationTimer; } set { aggrevationTimer = value; } }
        public float AggrevationDuration { get { return aggrevationDuration; } }

        bool stunned;
        public bool Stunned { get { return stunned; } set { stunned = value; } }

        private PlayerController player;

        [SerializeField] private List<NPCController> enemiesInScene = null;

        private void Awake()
        {
            GetComponent<Health>().OnDeath.AddListener(RemoveAIFromGameSpace);
            GetComponent<Health>().OnHealthChanged += Aggrevate;

            GameObject playerGO = GameObject.FindGameObjectWithTag("Player");
            playerTransform = playerGO.transform;
            player = playerGO.GetComponent<PlayerController>();
        }

        protected override void Initialize()
        {
            if (GetComponent<Health>().isDead) return;

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
            aggrevated = false;
            player.GetComponent<PlayerController>().RemoveAI(this);
            Destroy(this, 3f);
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
            if (GetComponent<Health>().isDead)
            {
                isDead = true;
                return;
            }
            if (aggrevated)
            {
                SmoothLookAt(playerTransform.gameObject);
            }
        }

        protected override void StateFixedUpdate()
        {
            if (GetComponent<Health>().isDead) return;

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
                patrol = new Patrol(player, this, patrolPath, chaseDistance, patrollingSpeedFraction, waypointTolerance);
                patrol.AddTransition(Transition.AtWaypoint, StateID.Guard);
            }
            else
            {
                patrol = new Patrol(player, this, transform.position, chaseDistance);
            }
            patrol.AddTransition(Transition.Aggrevated, StateID.Chase);
            patrol.AddTransition(Transition.PlayerInChaseDistance, StateID.Chase);
            patrol.AddTransition(Transition.PlayerInAttackRange, StateID.Attack);


            Guard guard = new Guard(player, this, waypointDwellTime, chaseDistance);
            guard.AddTransition(Transition.Aggrevated, StateID.Chase);
            guard.AddTransition(Transition.WaitTimeOver, StateID.Patrol);
            guard.AddTransition(Transition.PlayerInChaseDistance, StateID.Chase);
            guard.AddTransition(Transition.PlayerInAttackRange, StateID.Attack);


            Chase chase = new Chase(player, this, chaseDistance, attackDistance);
            chase.AddTransition(Transition.PlayerInAttackRange, StateID.Attack);
            chase.AddTransition(Transition.PlayerLeavesChaseDistance, StateID.Suspicion);
            chase.AddTransition(Transition.Deaggrevated, StateID.Suspicion);


            Suspicion suspicion = new Suspicion(player, this, suspicionDuration, chaseDistance);
            suspicion.AddTransition(Transition.SuspicionTimeUp, StateID.Patrol);
            suspicion.AddTransition(Transition.PlayerInChaseDistance, StateID.Chase);


            Attack attack = new Attack(player, this, attackDistance, hasRangedAttack);
            attack.AddTransition(Transition.PlayerLeavesAttackRange, StateID.Chase);

            Dead dead = new Dead(player, this);

            AddState(patrol);
            AddState(guard);
            AddState(chase);
            AddState(suspicion);
            AddState(attack);

            AddTransitionToAll(Transition.Stunned, StateID.Stunned);

            Stunned stun = new Stunned(player, this, 1.5f);
            stun.AddTransition(Transition.StunOver, StateID.Chase);
            stun.AddTransition(Transition.Stunned, StateID.Stunned);

            AddState(stun);

            AddTransitionToAll(Transition.Dead, StateID.Dead);

            AddState(dead);

        }

        public void SmoothLookAt(GameObject target)
        {
            float rotSpeed = 15.0f;

            Quaternion targetRotation = Quaternion.LookRotation(target.transform.position - transform.position);

            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotSpeed * Time.deltaTime);
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

