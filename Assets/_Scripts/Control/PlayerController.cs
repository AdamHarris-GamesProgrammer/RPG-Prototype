using UnityEngine;

using RPG.Movement;
using RPG.Combat;

namespace RPG.Control
{
    public class PlayerController : MonoBehaviour
    {
        private Mover playerMover;

        void Awake()
        {
            playerMover = GetComponent<Mover>();
        }

        void Update()
        {
            InteractWithCombat();
            InteractWithMovement();
        }

        private void InteractWithCombat()
        {
            RaycastHit[] hits = Physics.RaycastAll(GetMouseRay());

            foreach(RaycastHit hit in hits)
            {
                CombatTarget target = hit.transform.GetComponent<CombatTarget>();
                if (target == null) continue;

                if (Input.GetMouseButtonDown(0))
                {
                    GetComponent<Fighter>().Attack(target);
                }
            }
        }

        private void InteractWithMovement()
        {
            if (Input.GetMouseButton(0))
            {
                MoveToCursor();
            }

            if (Input.GetAxis("Horizontal") != 0.0f || Input.GetAxis("Vertical") != 0.0f)
            {
                MoveWithKeyboard();
            }
        }

        void MoveToCursor()
        {
            RaycastHit hit;

            if (Physics.Raycast(GetMouseRay(), out hit))
            {
                playerMover.MoveTo(hit.point);
            }
        }

        void MoveWithKeyboard()
        {
            float horizontalInput = Input.GetAxis("Horizontal");
            float verticalInput = Input.GetAxis("Vertical");

            if (horizontalInput != 0.0f || verticalInput != 0.0f)
            {
                Vector3 newTarget = new Vector3(transform.position.x + horizontalInput, 0.0f, transform.position.z + verticalInput);
                playerMover.MoveTo(newTarget);
            }
        }
        private static Ray GetMouseRay()
        {
            return Camera.main.ScreenPointToRay(Input.mousePosition);
        }
    }
}
