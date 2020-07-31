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
            Patrol patrol = new Patrol(patrolPath);
            patrol.AddTransition(Transition.AtWaypoint, StateID.Guard);

            Guard guard = new Guard(waypointDwellTime);
            guard.AddTransition(Transition.WaitTimeOver, StateID.Patrol);


            AddState(patrol);
            AddState(guard);
        }

    }
}

