﻿using RPG.Combat;
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

        [Header("Strafing Settings")]
        [SerializeField] float staminaUsedWhileStrafing = 25.0f;
        [Tooltip("This a percentage from 0% to 100% chance to strafe")]
        [Range(0f,1f)]
        [SerializeField] float strafingChance = 0.3f;

        int currentWaypoint = 0;

        private GameObject player;
        private PlayerController playerController;

        private Fighter fighter;

        

        private Health playerHealth;

        Vector3 guardPosition;
        float timeSinceLastSeenPlayer = Mathf.Infinity;
        float timeSinceAggrevated = Mathf.Infinity;
        float timeAtCurrentWaypoint = Mathf.Infinity;

        [SerializeField] private List<AIController> enemiesInScene = null;

        bool aggrevated = false;

        protected override void Awake()
        {
            base.Awake();

            player = GameObject.FindGameObjectWithTag("Player");
            playerController = player.GetComponent<PlayerController>();
            playerHealth = player.GetComponent<Health>();

            fighter = GetComponent<Fighter>();

            stamina = GetComponent<Stamina>();

            health.OnDeath += RemoveAIFromGameSpace;

            guardPosition = transform.position;

            GetComponent<Health>().OnHealthChanged += Aggrevate;
        }

        private void RemoveAIFromGameSpace()
        {
            player.GetComponent<PlayerController>().RemoveAI(this);
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

            base.Update();

            if (IsAggrevated() && fighter.CanAttack(player) && !playerHealth.isDead)
            {
                AttackState();
            }
            else if (IsPlayerInRange(warningRadius))
            {
                DeAggrevate();
                WarningState();
            }
            else if (timeSinceLastSeenPlayer < suspiscionDuration)
            {
                DeAggrevate();
                SuspicionState();
            }
            else
            {
                DeAggrevate();
                PatrolState();
            }
            timeSinceLastSeenPlayer += Time.deltaTime;

            if (Vector3.Distance(transform.position, player.transform.position) > warningRadius)
            {
                timeSinceAggrevated += Time.deltaTime;
            }
            else
            {
                timeSinceAggrevated = 0.0f;
            }
        }

        void DeAggrevate()
        {
            if (!aggrevated) return;

            aggrevated = false;
            if (playerController.aggrevatedEnemies.Contains(this))
            {
                playerController.RemoveAI(this);
            }
        }

        private void AttackState()
        {
            Aggrevate();
            timeSinceLastSeenPlayer = 0.0f;

            if (fighter.IsInRange(player.transform.position))
            {
                fighter.Attack(player);
            }
            else
            {
                GetComponent<Mover>().MoveTo(player.transform.position, 1.0f, false);
            }
        }

        public void Strafe()
        {
            if (strafing) return;

            if (UnityEngine.Random.value > strafingChance) return;

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



        private static void WarningState()
        {
            //Debug.Log("Im warning you, back off!");
        }
        private void SuspicionState()
        {
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }

        private void PatrolState()
        {

            Vector3 nextPosition = guardPosition;

            if(patrolPath != null)
            {
                if (AtWaypoint())
                {
                    timeAtCurrentWaypoint += Time.deltaTime;

                    if(timeAtCurrentWaypoint >= dwellTime)
                    {
                        timeAtCurrentWaypoint = 0.0f;

                        currentWaypoint++;
                        currentWaypoint = patrolPath.CycleWaypoint(currentWaypoint);
                    }
                }
                //Set next position to current Waypoint
                nextPosition = patrolPath.GetWaypointPosition(currentWaypoint);
            }

            GetComponent<Mover>().StartMoveAction(nextPosition, patrollingSpeedFraction, false);
            
        }

        public void Aggrevate()
        {
            if (aggrevated) return;

            timeSinceAggrevated = 0.0f;

            aggrevated = true;

            playerController.aggrevatedEnemies.Add(this);

            foreach(AIController controller in enemiesInScene)
            {
                if (controller.aggrevated) continue;

                if (Vector3.Distance(controller.transform.position, transform.position) > warningRadius) continue;

                controller.Aggrevate();
            }
        }

        bool IsAggrevated()
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

            return distanceToPlayer < chaseDistance || timeSinceAggrevated < aggrevatedDuration;
        }

        private bool IsPlayerInRange(float distance)
        {
            return Vector3.Distance(player.transform.position, transform.position) < distance;
        }

        private bool AtWaypoint()
        {
            float distanceToWaypoint = Vector3.Distance(transform.position, patrolPath.GetWaypointPosition(currentWaypoint));

            if (distanceToWaypoint <= waypointTolerance)
            {
                return true;
            }

            return false;
        }

        //Called by Unity Editor
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, chaseDistance);

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, warningRadius);
        }
    }
}

