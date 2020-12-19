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

        public bool InCombat() { return inCombat; }

        Camera mainCamera;

        List<NPCController> enemiesInImmediateCombatArea;
        public List<NPCController> aggrevatedEnemies;

        [SerializeField] ParticleSystem spellParticle;

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

            if (Input.GetKeyDown(KeyCode.Q))
            {
                CastSpell();
            }
        }

        void CastSpell()
        {
            return; //TODO: Decide on if magic will be in game
            if (spellParticle.isEmitting) return;

            spellParticle.Play();
        }

        private void InteractWithCombat()
        {
            if (enemiesInImmediateCombatArea.Count == 0) return;

            if (Input.GetMouseButtonDown(0))
            {

                Vector3 rayPoint = GetMouseRay().GetPoint(triggerRadius);

                //Find the closest enemy to the player
                NPCController closestEnemy = ClosestEnemyToPoint(rayPoint);

                if (GetComponent<Fighter>().CanAttack(closestEnemy.gameObject))
                {
                    if (Input.GetKey(KeyCode.LeftShift))
                    {
                        GetComponent<Fighter>().Attack(closestEnemy.gameObject, true);
                    }
                    else
                    {
                        GetComponent<Fighter>().Attack(closestEnemy.gameObject, false);
                    }
                }

            }
        }

        private NPCController ClosestEnemyToPoint(Vector3 point)
        {
            NPCController closestEnemy = null;
            float closestEnemyDistance = 10.0f;
            foreach (NPCController enemy in enemiesInImmediateCombatArea)
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
            enemiesInImmediateCombatArea.Add(ai);


            OnCombat();
            inCombat = true;
        }

        public void RemoveAI(NPCController ai)
        {
            if (aggrevatedEnemies.Contains(ai))
            {
                aggrevatedEnemies.Remove(ai);
            }

            if (enemiesInImmediateCombatArea.Contains(ai))
            {
                enemiesInImmediateCombatArea.Remove(ai);
            }

            if (aggrevatedEnemies.Count == 0)
            {
                Debug.Log("No longer in combat");
                inCombat = false;
                OnCombat();
            }
        }

        private void InteractWithActionBar()
        {
            int size = 6;

            for(int i = 1; i <= size; i++)
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
            }else if (Input.GetKeyDown(KeyCode.Q))
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
