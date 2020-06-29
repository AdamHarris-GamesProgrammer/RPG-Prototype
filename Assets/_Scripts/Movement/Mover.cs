using RPG.Combat;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Movement
{
    public class Mover : MonoBehaviour
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
            GetComponent<Fighter>().Cancel();
            MoveTo(location);
        }

        public void MoveTo(Vector3 location)
        {
            agent.destination = location;
            agent.isStopped = false;
        }

        public void Stop()
        {
            agent.isStopped = true;
        }

        private void UpdateAnimator()
        {
            Vector3 velocity = agent.velocity;

            Vector3 localVelocity = transform.InverseTransformDirection(velocity);

            float speed = localVelocity.z;

            animator.SetFloat("ForwardSpeed", speed);
        }

    }
}

