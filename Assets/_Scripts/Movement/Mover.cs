using RPG.Core;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Movement
{
    public class Mover : MonoBehaviour, IAction
    {
        private NavMeshAgent agent;
        private Animator animator;

        [SerializeField] private float maxSpeed = 4.0f;

        void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            animator = GetComponent<Animator>();
        }

        private void Update()
        {
            agent.enabled = !GetComponent<Health>().isDead;

            UpdateAnimator();
        }

        public void StartMoveAction(Vector3 location, float speedFraction)
        {
            
            GetComponent<ActionScheduler>().StartAction(this);
            MoveTo(location, speedFraction);
        }

        public void MoveTo(Vector3 location, float speedFraction)
        {
            agent.speed = maxSpeed * Mathf.Clamp01(speedFraction);
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
            //print("Canceling Movement");
        }
    }
}

