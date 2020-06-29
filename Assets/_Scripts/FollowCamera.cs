using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    [SerializeField] private Transform target;

    //Vector3 offset;
    private void Start()
    {
        //offset = Camera.main.transform.position - target.position;
    }

    void Update()
    {
        transform.position = target.position;
    }
}
