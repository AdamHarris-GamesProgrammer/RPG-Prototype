using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveForward : MonoBehaviour
{
    [SerializeField] float movementSpeed = 3.0f;

    private void FixedUpdate()
    {
        transform.Translate(transform.forward * movementSpeed * Time.fixedDeltaTime, Space.World);
    }
}
