using UnityEngine;
using UnityEngine.UI;

using RPG.Movement;
using RPG.Combat;
using RPG.Resources;
using System;
using System.Collections.Generic;
using UnityEngine.AI;

namespace RPG.Control
{
    public class PlayerController : MonoBehaviour
    {
        private Mover playerMover;
        private NavMeshAgent playerAgent;
        private Health playerHealth;

        public float interactionRange = 1.5f;

        bool isSprinting = false;

        [SerializeField] float strafeTime = 0.8f;
        float timeSinceStrafeStarted = Mathf.Infinity;

        List<AIController> enemiesInImmediateCombatArea;
        float triggerRadius;

        void Awake()
        {
            playerMover = GetComponent<Mover>();
            playerHealth = GetComponent<Health>();
            playerAgent = GetComponent<NavMeshAgent>();

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            enemiesInImmediateCombatArea = new List<AIController>();

            triggerRadius = GetComponent<BoxCollider>().size.x / 2.0f;
        }
        void Update()
        {
            //if the player is dead then dont continue 
            if (playerHealth.isDead) return;

            InteractWithCombat();

            MoveWithKeyboard();

        }

        private void InteractWithCombat()
        {
            if (enemiesInImmediateCombatArea.Count == 0) return;

            if (Input.GetMouseButtonDown(0))
            {
                Vector3 rayPoint = GetMouseRay().GetPoint(triggerRadius);

                //Find the closest enemy to the player
                AIController closestEnemy = ClosestEnemyToPoint(rayPoint);

                if (GetComponent<Fighter>().CanAttack(closestEnemy.gameObject))
                {
                    GetComponent<Fighter>().Attack(closestEnemy.gameObject);
                }
            }
        }

        private AIController ClosestEnemyToPoint(Vector3 point)
        {
            AIController closestEnemy = null;
            float closestEnemyDistance = 10.0f;
            foreach (AIController enemy in enemiesInImmediateCombatArea)
            {
                float distanceToEnemy = Vector3.Distance(enemy.transform.position, point);

                if (distanceToEnemy < closestEnemyDistance)
                {
                    closestEnemy = enemy;
                    closestEnemyDistance = distanceToEnemy;
                }
            }
            return closestEnemy;
        }

        bool strafing = false;

        void MoveWithKeyboard()
        {

            if (strafing)
            {
                timeSinceStrafeStarted += Time.deltaTime;

                if(timeSinceStrafeStarted > strafeTime)
                {
                    timeSinceStrafeStarted = 0.0f;
                    strafing = false;
                    playerAgent.updateRotation = true;
                }

                return;
            }
            
            Vector2 input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));


            if (input == Vector2.zero) return;

            if (Input.GetKey(KeyCode.LeftShift))
            {
                isSprinting = true;
            }
            else
            {
                isSprinting = false;
            }



            if (Input.GetKey(KeyCode.C))
            {
                if (Input.GetKey(KeyCode.W))
                {
                    input.y += 1.5f;
                    Strafe("strafeForward");
                    isSprinting = true;

                }
                else if (Input.GetKey(KeyCode.A))
                {
                    input.x -= 1.5f;
                    Strafe("strafeLeft");
                    isSprinting = true;
                }
                else if (Input.GetKey(KeyCode.S))
                {
                    input.y -= 1.5f;
                    Strafe("strafeBackward");
                    isSprinting = true;

                    
                }
                else if (Input.GetKey(KeyCode.D))
                {
                    input.x += 1.5f;
                    Strafe("strafeRight");
                    isSprinting = true;

                }
            }

            Vector3 movement = Camera.main.transform.forward * input.y + Camera.main.transform.right * input.x;

            movement += transform.position;

            playerMover.StartMoveAction(movement, 1.0f, isSprinting);
        }

        private void Strafe(string animTrigger)
        {
            strafing = true;
            playerAgent.updateRotation = false;
            timeSinceStrafeStarted = 0.0f;
            GetComponent<Animator>().SetTrigger(animTrigger);
        }

        private static Ray GetMouseRay()
        {
            return Camera.main.ScreenPointToRay(new Vector2(Screen.width / 2, Screen.height / 2));
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.gameObject.CompareTag("Enemy")) return;

            enemiesInImmediateCombatArea.Add(other.gameObject.GetComponent<AIController>());
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.gameObject.CompareTag("Enemy")) return;

            enemiesInImmediateCombatArea.Remove(other.gameObject.GetComponent<AIController>());
        }

        public void RemoveAI(AIController ai)
        {
            enemiesInImmediateCombatArea.Remove(ai);
        }

        public bool IsStrafing()
        {
            return strafing;
        }

        void FootL()
        {

        }

        void FootR()
        {

        }
    }
}
