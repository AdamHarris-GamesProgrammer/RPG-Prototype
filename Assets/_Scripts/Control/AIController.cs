using RPG.Combat;
using RPG.Core;
using RPG.Movement;
using UnityEngine;
using RPG.Resources;
using System.Collections.Generic;
using System;

namespace RPG.Control
{
    public class AIController : Controller
    {
        [Header("Distance Settings")]
        [SerializeField] float chaseDistance = 5.0f;
        [SerializeField] float warningRadius = 12.0f;

        [Header("Timer Settings")]
        [SerializeField] float suspiscionDuration = 5.0f;
        [SerializeField] float aggrevatedDuration = 15.0f;
        [SerializeField] float dwellTime = 3.5f;

        [Header("Patrolling Settings")]
        [SerializeField] PatrolPath patrolPath = null;
        [SerializeField] float waypointTolerance = 1.0f;

        [Header("Speed Settings")]
        [Range(0f,1f)]
        [SerializeField] float patrollingSpeedFraction = 0.2f;

        [Header("Dodge/Block Settings")]
        [SerializeField] float staminaUsedWhileStrafing = 25.0f;
        [Tooltip("This is a percentage from 0 to 1 for chance to dodge")]
        [Range(0f,1f)]
        [SerializeField] float strafingChance = 0.3f;
        [SerializeField] float staminaUsedWhileBlocking = 12.5f;
        [Tooltip("This is a percentage from 0 to 1 for chance to block attack")]
        [Range(0f, 1f)]
        [SerializeField] float chanceToBlock = 0.3f;

        

        private GameObject player;

        private Fighter fighter;

        

        private Health playerHealth;

        float timeSinceLastSeenPlayer = Mathf.Infinity;
        float timeSinceAggrevated = Mathf.Infinity;

        [SerializeField] private List<AIController> enemiesInScene = null;

        bool aggrevated = false;

        protected override void Awake()
        {
            base.Awake();

            player = GameObject.FindGameObjectWithTag("Player");
            playerHealth = player.GetComponent<Health>();

            fighter = GetComponent<Fighter>();

            stamina = GetComponent<Stamina>();
        }

        private void RemoveAIFromGameSpace()
        {
            //player.GetComponent<PlayerController>().RemoveAI(this);
        }

        private void Start()
        {
            if (health.isDead) return;

            foreach(AIController aiController in FindObjectsOfType<AIController>())
            {
                if (!aiController.GetComponent<Health>().isDead)
                {
                    enemiesInScene.Add(aiController);
                }
            }
        }

        protected override void Update()
        {
            if (health.isDead) return;

            timeSinceLastSeenPlayer += Time.deltaTime;

        }

        public void DecideDefence()
        {
            float value = UnityEngine.Random.value;

            float rangeToDodge = 1 - strafingChance;
            float rangeToBlock = rangeToDodge - chanceToBlock;

            Debug.Log("Random Value: " + value + " Range to dodge: " + rangeToDodge + " Range to block: " + rangeToBlock);

            if(value < rangeToBlock)
            {
                Debug.Log("Enemy Block");
                blocking = true;
            }
            else if(value < rangeToDodge)
            {
                Debug.Log("Enemy Dodge");
                Strafe();
                blocking = false;
            }
            else
            {
                Debug.Log("Enemy does nothing");
                blocking = false;
            }
        }

        private void Strafe()
        {
            if (strafing) return;

            if (stamina.CurrentStamina - staminaUsedWhileStrafing < 0) return;

            int direction = UnityEngine.Random.Range(1, 3);

            Vector2 dodgeMovement = new Vector2();
            string animTrigger = "";

            switch (direction)
            {
                case 1:
                    //Strafe Left
                    animTrigger = "strafeLeft";
                    dodgeMovement.x = -1.5f;
                    break;
                case 2:
                    //Strafe Right
                    animTrigger = "strafeRight";
                    dodgeMovement.x = 1.5f;
                    break;
                case 3:
                    //Strafe Backward
                    animTrigger = "strafeBackward";
                    dodgeMovement.y = -1.5f;
                    break;
            }

            StrafeAction(animTrigger);

            //Calculate strafing movement
            Vector3 newPosition = transform.forward * dodgeMovement.y + transform.right * dodgeMovement.x;

            newPosition += transform.position;

            GetComponent<Mover>().MoveTo(newPosition, 1.0f, false, false);
            stamina.CurrentStamina -= staminaUsedWhileStrafing;
            stamina.StaminaUsed(true);
        }

    }
}

