using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Control
{
    public class NPCController : StateMachine
    {
        [Header("Patrolling Settings")]
        [SerializeField] PatrolPath patrolPath = null;
        [SerializeField] float waypointTolerance = 1.0f;
        [SerializeField] float waypointDwellTime = 3.5f;

        [Header("Suspicion Settings")]
        [SerializeField] float suspicionDuration = 10.0f;

        protected override void Initialize()
        {
            GameObject playerGO = GameObject.FindGameObjectWithTag("Player");
            playerTransform = playerGO.transform;

            ConstructStateMachine();
        }

        protected override void StateUpdate()
        {

        }

        protected override void StateFixedUpdate()
        {
            if(playerTransform == null)
            {
                Debug.LogError("Player Transform is null");
                return;
            }

            if(CurrentState == null)
            {
                Debug.LogError("Current state is null");
                return;
            }

            CurrentState.Reason(playerTransform, transform);
            CurrentState.Act(playerTransform, transform);
        }

        public void SetTransition(Transition t)
        {
            PerformTransition(t);
        }

        private void ConstructStateMachine()
        {
            Patrol patrol = new Patrol(this, patrolPath);
            patrol.AddTransition(Transition.AtWaypoint, StateID.Guard);
            patrol.AddTransition(Transition.PlayerInChaseDistance, StateID.Chase);

            Guard guard = new Guard(this, waypointDwellTime);
            guard.AddTransition(Transition.WaitTimeOver, StateID.Patrol);
            guard.AddTransition(Transition.PlayerInChaseDistance, StateID.Chase);

            Chase chase = new Chase(this);
            chase.AddTransition(Transition.PlayerInAttackRange, StateID.Attack);
            chase.AddTransition(Transition.PlayerLeavesChaseDistance, StateID.Suspicion);

            Suspicion suspicion = new Suspicion(this, suspicionDuration);
            suspicion.AddTransition(Transition.SuspicionTimeUp, StateID.Patrol);
            suspicion.AddTransition(Transition.PlayerInChaseDistance, StateID.Chase);

            Attack attack = new Attack(this);
            attack.AddTransition(Transition.PlayerLeavesAttackRange, StateID.Chase);

            Dead dead = new Dead(this);

            AddState(patrol);
            AddState(guard);
            AddState(chase);
            AddState(attack);
            AddState(dead);
        }

    }
}

