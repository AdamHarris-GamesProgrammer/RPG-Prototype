using UnityEngine;
using UnityEngine.AI;

using RPG.Movement;
using RPG.Resources;

namespace RPG.Control
{
    public class Controller : MonoBehaviour
    {
        protected bool strafing;
        protected bool blocking;

        public bool IsStrafing { get { return strafing; } }
        public bool IsBlocking { get { return blocking; } }

        [Header("Strafe Settings")]
        [SerializeField] float strafeDuration = 0.8f;
        protected float timeSinceStrafeStarted = Mathf.Infinity;

        [Header("Block Settings")]
        [Range(0f,1f)]
        [SerializeField] float blockDamageReduction = 0.2f;
        [SerializeField] protected float staminaReductionFromBlocking = 12.0f;


        public float BlockReduction { get { return blockDamageReduction; } }

        protected Mover mover;
        protected NavMeshAgent agent;
        protected Health health; 
        protected Stamina stamina;

        protected virtual void Awake()
        {
            mover = GetComponent<Mover>();
            agent = GetComponent<NavMeshAgent>();
            health = GetComponent<Health>();
            stamina = GetComponent<Stamina>();
        }

        protected void StrafeAction(string animTrigger)
        {
            strafing = true;
            timeSinceStrafeStarted = 0.0f;
            agent.updateRotation = false;
            GetComponent<Animator>().SetTrigger(animTrigger);
        }

        protected virtual void Update()
        {
            if (strafing)
            {
                timeSinceStrafeStarted += Time.deltaTime;

                if (timeSinceStrafeStarted > strafeDuration)
                {
                    timeSinceStrafeStarted = 0.0f;
                    strafing = false;
                    agent.updateRotation = true;
                }

                return;
            }
        }

        public virtual void BlockDamage()
        {
            stamina.StaminaUsed(true);
            stamina.CurrentStamina -= staminaReductionFromBlocking;
        }

        //Animation events
        protected void FootL()
        {

        }

        protected void FootR()
        {

        }


    }
}

