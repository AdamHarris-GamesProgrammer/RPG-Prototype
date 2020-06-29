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

        if (Input.GetMouseButtonDown(0))
        {
            MoveToCursor();
        }
        Debug.DrawRay(lastRay.origin, lastRay.direction * 25.0f, Color.green);
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

}
