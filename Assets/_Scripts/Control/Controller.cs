using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.AI;

using RPG.Movement;
using RPG.Combat;
using RPG.Resources;

namespace RPG.Control
{
    public class Controller : MonoBehaviour
    {
        protected bool strafing;
        protected bool blocking;

        public bool IsStrafing { get { return strafing; } }
        public bool IsBlocking { get { return blocking; } }

        [SerializeField] float strafeDuration = 0.8f;
        protected float timeSinceStrafeStarted = Mathf.Infinity;

        protected Mover mover;
        protected NavMeshAgent agent;
        protected Health health;

        protected virtual void Awake()
        {
            mover = GetComponent<Mover>();
            agent = GetComponent<NavMeshAgent>();
            health = GetComponent<Health>();
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

        //Animation events
        protected void FootL()
        {

        }

        protected void FootR()
        {

        }
    }
}

