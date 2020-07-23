using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMover : MonoBehaviour
{
    bool isSprinting = false;
    [SerializeField] float movementSpeed = 5f;


    private void Awake()
    {
        
    }

    void Update()
    {
        //MoveWithKeyboard();
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

        Vector3 movement = (Camera.main.transform.forward * input.y + Camera.main.transform.right * input.x) * movementSpeed * Time.deltaTime;

        transform.Translate(movement);

    }
}
