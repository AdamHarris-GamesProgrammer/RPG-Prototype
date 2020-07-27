using RPG.Core;
using RPG.Resources;
using RPG.Saving;
using RPG.Stats;
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
        [Tooltip("Amount of time between using stamina and it starting to regenerate")]
        [SerializeField] float sprintCooldown = 3.5f;

        float stamina = 0.0f;
        float maxStamina = 0.0f;
        bool staminaRegenerating = false;
        float timeSinceLastUsedStamina = Mathf.Infinity;
        bool sprinting = false;

        StaminaBar staminaBar;

        void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            animator = GetComponent<Animator>();

            actionScheduler = GetComponent<ActionScheduler>();

            health = GetComponent<Health>();

            maxStamina = GetComponent<BaseStats>().GetStat(Stat.Stamina);
            stamina = maxStamina;

            if (TryGetComponent(out StaminaBar bar))
            {
                staminaBar = bar;
            }
        }

        private void Update()
        {
            agent.enabled = !health.isDead;

            if (staminaRegenerating)
            {
                timeSinceLastUsedStamina += Time.deltaTime;
                if (timeSinceLastUsedStamina >= sprintCooldown)
                {
                    stamina += sprintEnergy * Time.deltaTime;

                    if (staminaBar != null) staminaBar.UpdateBar(stamina, maxStamina);
                }
            }

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
            sprinting = isSprinting;
            agent.updateRotation = freeRotation;

            if (stamina <= 0.0f)
            {
                sprinting = false;
                staminaRegenerating = true;
            }

            if (sprinting)
            {
                timeSinceLastUsedStamina = 0.0f;
                staminaRegenerating = false;
                speedFraction = sprintingFactor;
                stamina -= sprintEnergy * Time.deltaTime;
            }
            else
            {
                staminaRegenerating = true;
            }

            stamina = Mathf.Clamp(stamina, 0f, maxStamina);

            if (staminaBar != null) staminaBar.UpdateBar(stamina, maxStamina);

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