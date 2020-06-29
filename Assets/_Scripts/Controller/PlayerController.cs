using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class PlayerController : MonoBehaviour
{
    private Mover playerMover;

    void Awake()
    {
        playerMover = GetComponent<Mover>();
    }

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            MoveToCursor();
        }

        if(Input.GetAxis("Horizontal") != 0.0f || Input.GetAxis("Vertical") != 0.0f)
        {
            MoveWithKeyboard();
        }
    }

    void MoveToCursor()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
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
}
