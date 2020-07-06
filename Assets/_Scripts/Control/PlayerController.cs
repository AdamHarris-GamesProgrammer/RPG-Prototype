using UnityEngine;

using RPG.Movement;
using RPG.Combat;
using RPG.Core;
using RPG.SceneManagement;

namespace RPG.Control
{
    public class PlayerController : MonoBehaviour
    {
        private Mover playerMover;

        public float interactionRange = 1.5f;

        void Awake()
        {
            playerMover = GetComponent<Mover>();
        }
        void Update()
        {
            if (GetComponent<Health>().isDead) return;

            //if you have attacked then you don't move
            if (InteractWithCombat()) return;

            if (InteractWithTransition()) return;

            if (InteractWithMovement()) return;
        }

        private bool InteractWithCombat()
        {
            RaycastHit[] hits = Physics.RaycastAll(GetMouseRay());

            foreach (RaycastHit hit in hits)
            {
                CombatTarget target = hit.transform.GetComponent<CombatTarget>();
                if (target == null) continue;

                GameObject targetGameObject = target.gameObject;

                if (!GetComponent<Fighter>().CanAttack(target.gameObject)) continue;

                if (Input.GetMouseButtonDown(0))
                {

                    GetComponent<Fighter>().Attack(target.gameObject);

                }

                return true;
            }
            return false;
        }

        private bool InteractWithTransition()
        {
            RaycastHit[] hits = Physics.RaycastAll(GetMouseRay());

            foreach (RaycastHit hit in hits)
            {
                SceneTarget target = hit.transform.GetComponent<SceneTarget>();
                if (target == null) continue;

                GameObject targetGameObject = target.gameObject;

                if (Input.GetMouseButtonDown(0))
                {
                    if (Vector3.Distance(targetGameObject.transform.position, transform.position) > interactionRange)
                    {
                        playerMover.MoveTo(targetGameObject.transform.position, 1.0f);
                    }
                    else
                    {
                        GetComponent<ActionScheduler>().CancelCurrentAction();
                        target.TransitionTo();
                    }



                }

                return true;

            }
            return false;
        }

        private bool InteractWithMovement()
        {
            bool result = false;

            result = MoveToCursor();

            //TODO: Fix keyboard input
            if (Input.GetAxis("Horizontal") != 0.0f || Input.GetAxis("Vertical") != 0.0f)
            {
                MoveWithKeyboard();
            }

            return result;
        }

        bool MoveToCursor()
        {
            RaycastHit hit;

            if (Physics.Raycast(GetMouseRay(), out hit))
            {
                if (Input.GetMouseButton(0))
                {
                    playerMover.StartMoveAction(hit.point, 1.0f);
                }
                return true;
            }

            return false;
        }

        void MoveWithKeyboard()
        {
            float horizontalInput = Input.GetAxis("Horizontal");
            float verticalInput = Input.GetAxis("Vertical");

            //if (horizontalInput != 0.0f || verticalInput != 0.0f)
            //{
            //    Vector3 newTarget = new Vector3(transform.forward.x + horizontalInput, 0.0f, transform.right.z + verticalInput);
            //    playerMover.StartMoveAction(newTarget);
            //}

            Vector3 movement = new Vector3(horizontalInput, 0f, verticalInput);

            movement += transform.position;

            playerMover.StartMoveAction(movement, 1.0f);

        }


        private static Ray GetMouseRay()
        {
            return Camera.main.ScreenPointToRay(Input.mousePosition);
        }
    }
}
