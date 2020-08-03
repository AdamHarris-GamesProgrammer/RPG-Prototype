using RPG.Core;
using RPG.Resources;
using RPG.Saving;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Movement
{
    public class Mover : MonoBehaviour, IAction, ISaveable
    {
        private NavMeshAgent agent;
        private Animator animator;
        private Health health;
        private ActionScheduler actionScheduler;

        [SerializeField] private float maxSpeed = 4.0f;

        [Header("Sprint Settings")]
        [SerializeField] float sprintingFactor = 1.2f;
        [Tooltip("Amount of stamina used per second while sprinting")]
        [SerializeField] float sprintEnergy = 12.5f;

        Stamina stamina;

        bool sprinting = false;



        void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            animator = GetComponent<Animator>();

            actionScheduler = GetComponent<ActionScheduler>();

            health = GetComponent<Health>();

            stamina = GetComponent<Stamina>();
        }

        private void Update()
        {
            agent.enabled = !health.isDead;

            UpdateAnimator();
        }

        public void StartMoveAction(Vector3 location, float speedFraction, bool isSprinting, bool freeRotation = true)
        {
            agent.updateRotation = freeRotation;
            actionScheduler.StartAction(this);
            MoveTo(location, speedFraction, isSprinting);
        }

        public void MoveTo(Vector3 location, float speedFraction, bool isSprinting, bool freeRotation = true)
        {
            if (health.isDead) return;

            sprinting = isSprinting;
            agent.updateRotation = freeRotation;

            if (stamina.CurrentStamina <= 0.0f)
            {
                sprinting = false;
            }

            if (sprinting)
            {
                speedFraction = sprintingFactor;
                stamina.CurrentStamina -= sprintEnergy * Time.deltaTime;
            }

            stamina.StaminaUsed(sprinting);

            agent.speed = maxSpeed * speedFraction;
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
            //print(gameObject.name + " Canceling Movement");
        }

        public object CaptureState()
        {
            return new SerializableVector3(transform.position);
        }

        public void RestoreState(object state)
        {
            SerializableVector3 positon = (SerializableVector3)state;

            agent.enabled = false;

            transform.position = positon.ToVector();

            agent.enabled = true;
        }
    }
}