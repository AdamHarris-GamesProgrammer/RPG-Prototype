using UnityEngine;

using RPG.Movement;
using RPG.Combat;
using RPG.Resources;

namespace RPG.Control
{
    public class PlayerController : MonoBehaviour
    {
        private Mover playerMover;

        private Health playerHealth;

        public float interactionRange = 1.5f;

        bool inCombat;

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

            if (InteractWithCombat()) return;

            MoveWithKeyboard();
        }

        private bool InteractWithCombat()
        {
            RaycastHit[] hits = Physics.RaycastAll(GetMouseRay());

            foreach (RaycastHit hit in hits)
            {
                CombatTarget target = hit.transform.GetComponent<CombatTarget>();
                //if no CombatTarget component found, then continue
                if (target == null) continue;

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

        void MoveWithKeyboard()
        {
            Vector2 input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

            if (input == Vector2.zero) return;

            Vector3 movement = Camera.main.transform.forward * input.y + Camera.main.transform.right * input.x;

            movement += transform.position;

            playerMover.StartMoveAction(movement, 1.0f);
        }

        private static Ray GetMouseRay()
        {
            return Camera.main.ScreenPointToRay(new Vector2(Screen.width /2, Screen.height /2));
        }
    }
}
