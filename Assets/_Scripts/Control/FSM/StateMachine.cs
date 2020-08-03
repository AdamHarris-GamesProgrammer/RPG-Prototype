using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Control
{
    public enum Transition
    {
        None = 0,
        WaitTimeOver,
        AtWaypoint,
        PlayerInChaseDistance,
        PlayerLeavesChaseDistance,
        PlayerInAttackRange,
        PlayerLeavesAttackRange,
        SuspicionTimeUp
    }

    public enum StateID
    {
        None = 0,
        Guard,
        Patrol,
        Chase,
        Attack,
        Suspicion,
        Dead
    }

    public class StateMachine : MonoBehaviour
    {
        protected Transform playerTransform;

        protected Vector3 destinationPos;

        protected GameObject[] pointList;

        [SerializeField] private List<State> states;

        private StateID currentStateID;
        public StateID CurrentStateID { get { return currentStateID; } }

        private State currentState;
        public State CurrentState { get { return currentState; } }

        public StateMachine()
        {
            states = new List<State>();
        }

        //Adds the passed in state
        public void AddState(State state)
        {
            if (state == null)
            {
                Debug.LogError("Add State state is null");
                return;
            }

            if (states.Count == 0)
            {
                states.Add(state);
                currentState = state;
                currentState.OnEntry();
                currentStateID = state.ID;
                return;
            }

            foreach (State loopState in states)
            {
                if (loopState.ID == state.ID)
                {
                    Debug.LogError("[Error: StateMachine.cs]: Trying to add a state that is already in states");
                    return;
                }
            }

            states.Add(state);
        }

        //Deletes the passed in state to the states list
        public void DeleteState(StateID stateID)
        {
            if (stateID == StateID.None)
            {
                Debug.LogError("[Error: StateMachine.cs]: StateID is set to None");
                return;
            }

            foreach (State state in states)
            {
                if (state.ID == stateID)
                {
                    states.Remove(state);
                    return;
                }
            }

            Debug.LogError("[Error: StateMachine.cs]: The state passed in was not on the list. Cannot delete state");
        }

        protected void PerformTransition(Transition transition)
        {
            if (transition == Transition.None)
            {
                Debug.LogError("[Error: StateMachine.cs]: The passed in transition is set to none");
                return;
            }

            StateID id = currentState.GetOutputState(transition);
            if (id == StateID.None)
            {
                Debug.LogError("[Error: StateMachine.cs]: Current state does not have a target state for this transition");
                return;
            }

            currentStateID = id;
            foreach (State state in states)
            {
                if (state.ID == currentStateID)
                {
                    currentState.OnExit();
                    currentState = state;
                    currentState.OnEntry();
                    return;
                }
            }

            Debug.LogError("[Error: StateMachine.cs]: " + id + " is not in state map");
        }

        protected virtual void Initialize() { }

        protected virtual void StateUpdate() { }

        protected virtual void StateFixedUpdate() { }

        private void Start()
        {
            Initialize();
        }

        private void Update()
        {
            StateUpdate();
        }

        private void FixedUpdate()
        {
            StateFixedUpdate();
        }
    }
}

