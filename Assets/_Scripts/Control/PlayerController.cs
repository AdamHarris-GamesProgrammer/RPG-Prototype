using UnityEngine;
using UnityEngine.UI;

using RPG.Movement;
using RPG.Combat;
using RPG.Resources;
using System;

namespace RPG.Control
{
    public class PlayerController : MonoBehaviour
    {
        private Mover playerMover;

        private Health playerHealth;

        public float interactionRange = 1.5f;

        bool inCombat;
        bool isSprinting = false;


        [SerializeField] Image cursor;
        [SerializeField] Sprite combatCursor;
        [SerializeField] Sprite defaultCursor;


        enum CursorType
        {
            None,
            Combat,
            Chest,
            Door
        };

        void Awake()
        {
            playerMover = GetComponent<Mover>();
            playerHealth = GetComponent<Health>();

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        void Update()
        {
            //if the player is dead then dont continue 
            if (playerHealth.isDead) return;

            if (InteractWithCombat())
            {
                return;
            }
            else
            {
                
            }

            MoveWithKeyboard();

            
        }

        private bool InteractWithCombat()
        {
            RaycastHit[] hits = Physics.RaycastAll(GetMouseRay());

            foreach (RaycastHit hit in hits)
            {
                CombatTarget target = hit.transform.GetComponent<CombatTarget>();
                //if no CombatTarget component found, then continue
                if (target == null)
                {
                    SetCursor(defaultCursor);
                    continue;
                }
                else
                {
                    SetCursor(combatCursor);
                }


                //if they cant attack then the player is not in combat
                if (!GetComponent<Fighter>().CanAttack(target.gameObject))
                {
                    continue;
                }

                if (Input.GetMouseButtonDown(0))
                {
                    //Attack the targeted object 
                    GetComponent<Fighter>().Attack(target.gameObject);

                    //player is now in combat
                    inCombat = true;

                    return true;
                }

                

                if (inCombat)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            inCombat = false;

            return false;
        }

        private void SetCursor(Sprite sprite)
        {
            cursor.sprite = sprite;
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
            return Camera.main.ScreenPointToRay(new Vector2(Screen.width /2, Screen.height /2));
        }
    }
}
