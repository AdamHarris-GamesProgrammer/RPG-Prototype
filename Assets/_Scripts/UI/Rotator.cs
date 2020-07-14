using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    [Header("Rotation Speed")]
    [SerializeField] private float rotationSpeed = 5.0f;

    [Header("Rotation Values")]
    [SerializeField] private float xRotatation = 0.0f;
    [SerializeField] private float yRotatation = 0.0f;
    [SerializeField] private float zRotatation = 0.0f;
    
    void Update()
    {
        transform.Rotate(new Vector3(xRotatation, yRotatation, zRotatation) * rotationSpeed * Time.deltaTime, Space.Self);
    }
}
