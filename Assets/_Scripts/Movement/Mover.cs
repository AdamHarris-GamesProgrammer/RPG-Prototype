using RPG.Core;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Movement
{
    public class Mover : MonoBehaviour, IAction
    {
        private NavMeshAgent agent;
        private Animator animator;

        void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            animator = GetComponent<Animator>();
        }

        private void Update()
        {
            UpdateAnimator();
        }

        public void StartMoveAction(Vector3 location)
        {
            GetComponent<ActionScheduler>().StartAction(this);
            MoveTo(location);
        }

        public void MoveTo(Vector3 location)
        {
            agent.destination = location;
            agent.isStopped = false;
        }

        private void UpdateAnimator()
        {
            Vector3 velocity = agent.velocity;

            Vector3 localVelocity = transform.InverseTransformDirection(velocity);

            float speed = localVelocity.z;

            animator.SetFloat("ForwardSpeed", speed);
        }

        public void Cancel()
        {
            agent.isStopped = true;
            print("Canceling Movement");
        }
    }
}

