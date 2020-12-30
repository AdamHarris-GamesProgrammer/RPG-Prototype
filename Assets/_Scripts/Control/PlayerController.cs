using UnityEngine;

using RPG.Movement;
using RPG.Combat;
using RPG.Resources;
using System;
using System.Collections.Generic;
using UnityEngine.AI;
using RPG.Inventories;

namespace RPG.Control
{
    public class PlayerController : Controller
    {
        public float interactionRange = 1.5f;

        bool isSprinting = false;
        bool inCombat;

        ActionStore playerActionBar;
        Inventory playerInventory;
        Equipment playerEquipment;
        PlayerFighter playerFighter;

        public bool InCombat() { return inCombat; }

        Camera mainCamera;

        List<NPCController> enemiesInImmediateCombatArea;
        List<NPCController> aggrevatedEnemies;

        float triggerRadius;

        public event Action OnCombat;

        protected override void Awake()
        {
            base.Awake();

            mainCamera = Camera.main;

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            enemiesInImmediateCombatArea = new List<NPCController>();
            aggrevatedEnemies = new List<NPCController>();


            triggerRadius = 2.5f;

            OnCombat += CombatBehaviour;

            playerActionBar = GetComponent<ActionStore>();
            playerInventory = GetComponent<Inventory>();
            playerEquipment = GetComponent<Equipment>();
            playerFighter = GetComponent<PlayerFighter>();
        }



        protected override void Update()
        {
            //if the player is dead then dont continue 
            if (health.isDead) return;

            base.Update();

            InteractWithCombat();

            MoveWithKeyboard();

            InteractWithActionBar();

            InteractWithInventory();
            InteractWithEquipment();
        }

        private void InteractWithCombat()
        {
            if (Input.GetMouseButtonDown(0))
            {

                WeaponConfig weapon = playerFighter.GetWeaponConfig();

                Vector3 rayPoint = GetMouseRay().GetPoint(weapon.AttackRange);

                //Find the closest enemy to the player
                NPCController closestEnemy = ClosestEnemyToPoint(weapon, rayPoint);

                if (closestEnemy == null) return;

                if (playerFighter.CanAttack(closestEnemy.gameObject))
                {
                    if (Input.GetKey(KeyCode.LeftShift))
                    {
                        playerFighter.Attack(closestEnemy.gameObject, true);
                    }
                    else
                    {
                        playerFighter.Attack(closestEnemy.gameObject, false);
                    }
                }

            }
        }

        private NPCController ClosestEnemyToPoint(WeaponConfig weapon, Vector3 point)
        {
            NPCController closestEnemy = null;
            float closestEnemyDistance = float.MaxValue;

            foreach (NPCController enemy in FindObjectsOfType<NPCController>())
            {
                float distanceToEnemy = Vector3.Distance(enemy.transform.position, point);
                if (enemy.isDead || distanceToEnemy > weapon.AttackRange) continue;

                if (distanceToEnemy < closestEnemyDistance)
                {
                    closestEnemy = enemy;
                    closestEnemyDistance = distanceToEnemy;
                }
            }

            return closestEnemy;

        }

        private void Block()
        {
            if (Input.GetKey(KeyCode.E) && stamina.CurrentStamina - staminaReductionFromBlocking > 0)
            {
                blocking = true;
            }
            else
            {
                blocking = false;
            }
        }

        private void Sprint()
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                isSprinting = true;
            }
            else
            {
                isSprinting = false;
            }
        }

        void MoveWithKeyboard()
        {
            Block();
            Sprint();

            Vector2 input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

            if (input == Vector2.zero) return;

            if (Input.GetKey(KeyCode.C))
            {
                StrafeCalculation(ref input);
            }

            Vector3 movement = mainCamera.transform.forward * input.y + mainCamera.transform.right * input.x;

            movement += transform.position;

            mover.StartMoveAction(movement, 1.0f, isSprinting);
        }

        private void StrafeCalculation(ref Vector2 direction)
        {
            string animTrigger = "";

            if (Input.GetKey(KeyCode.W))
            {
                direction.y += 1.5f;
                animTrigger = "strafeForward";
            }
            else if (Input.GetKey(KeyCode.A))
            {
                direction.x -= 1.5f;
                animTrigger = "strafeLeft";
            }
            else if (Input.GetKey(KeyCode.S))
            {
                direction.y -= 1.5f;
                animTrigger = "strafeBackward";
            }
            else if (Input.GetKey(KeyCode.D))
            {
                direction.x += 1.5f;
                animTrigger = "strafeBackward";
            }

            if (animTrigger == "") return;
            StrafeAction(animTrigger);
        }

        private static Ray GetMouseRay()
        {
            return Camera.main.ScreenPointToRay(new Vector2(Screen.width / 2, Screen.height / 2));
        }


        private void CombatBehaviour()
        {
            if (inCombat)
            {
                //TODO: Play music if it isnt already playing

                //Stop player from interacting with objects
            }
            else
            {
                //TODO: Stop combat music playing 

                //Allow player to interact with objects

            }
        }

        public void AddAI(NPCController ai)
        {
            if (enemiesInImmediateCombatArea.Contains(ai)) return;

            enemiesInImmediateCombatArea.Add(ai);

            OnCombat();
            inCombat = true;
        }

        public void AddAggrevatedAI(NPCController ai)
        {
            if (aggrevatedEnemies.Contains(ai)) return;

            aggrevatedEnemies.Add(ai);
        }

        public void RemoveAI(NPCController ai)
        {
            Debug.Log("Remove AI Called");
            if (aggrevatedEnemies.Contains(ai))
            {
                Debug.Log("Aggrevated Enemies contains the AI");
                if (aggrevatedEnemies.Remove(ai))
                {
                    Debug.Log("AI Removed from Aggrevated Enemies");
                }

            }

            if (enemiesInImmediateCombatArea.Contains(ai))
            {
                Debug.Log("Immediate Combat Enemies contains the AI");
                if (enemiesInImmediateCombatArea.Remove(ai))
                {
                    Debug.Log("AI Removed from the immediate combat area ");
                }
            }

            if (aggrevatedEnemies.Count == 0)
            {
                Debug.Log("No longer in combat");
                inCombat = false;
                OnCombat();
            }
            else
            {
                Debug.Log("Aggrevated enemies contains " + aggrevatedEnemies.Count + " enemies");
            }
        }

        private void InteractWithActionBar()
        {
            int size = 6;

            for (int i = 1; i <= size; i++)
            {
                if (Input.GetKeyDown(i.ToString()))
                {
                    playerActionBar.Use(i - 1, gameObject);
                }
            }
        }

        private void InteractWithInventory()
        {
            //TODO Add check that inventory is open
            if (Input.GetKeyDown(KeyCode.E))
            {
                playerInventory.EquipItem();
            }
            else if (Input.GetKeyDown(KeyCode.Q))
            {
                playerInventory.DropSelected();
            }
        }

        private void InteractWithEquipment()
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                playerEquipment.Unequip();
            }
            else if (Input.GetKeyDown(KeyCode.Q))
            {
                playerEquipment.DropSelected();
            }
        }

    }
}
