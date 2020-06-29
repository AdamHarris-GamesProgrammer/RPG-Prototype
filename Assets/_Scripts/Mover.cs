using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Mover : MonoBehaviour
{
    //The last ray that was fired
    Ray lastRay;

    private NavMeshAgent agent;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {

        if (Input.GetMouseButton(0))
        {
            MoveToCursor();
        }
        Debug.DrawRay(lastRay.origin, lastRay.direction * 25.0f, Color.green);

        UpdateAnimator();
    }

    private void MoveToCursor()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            agent.destination = hit.point;
        }

        lastRay = ray;
    }

    private void UpdateAnimator()
    {
        Vector3 velocity = agent.velocity;

        Vector3 localVelocity = transform.InverseTransformDirection(velocity);

        float speed = localVelocity.z;

        GetComponent<Animator>().SetFloat("ForwardSpeed", speed);
    }

}
