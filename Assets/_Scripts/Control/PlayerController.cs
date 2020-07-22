using UnityEngine;
using UnityEngine.UI;

using RPG.Movement;
using RPG.Combat;
using RPG.Resources;
using System;
using System.Collections.Generic;

namespace RPG.Control
{
    public class PlayerController : MonoBehaviour
    {
        private Mover playerMover;

        private Health playerHealth;

        public float interactionRange = 1.5f;

        bool isSprinting = false;

        List<AIController> enemiesInImmediateCombatArea;
        float triggerRadius;

        void Awake()
        {
            playerMover = GetComponent<Mover>();
            playerHealth = GetComponent<Health>();

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

        void MoveWithKeyboard()
        {
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

            Vector3 movement = Camera.main.transform.forward * input.y + Camera.main.transform.right * input.x;

            movement += transform.position;

            playerMover.StartMoveAction(movement, 1.0f, isSprinting);
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
    }
}
