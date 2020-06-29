using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Mover : MonoBehaviour
{
    //The last ray that was fired
    Ray lastRay;

    private NavMeshAgent agent;
    //private CharacterController controller;

    [SerializeField] private float movementSpeed = 2.5f;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        //controller = GetComponent<CharacterController>();
    }

    private void Update()
    {

        if (Input.GetMouseButton(0))
        {
            MoveToCursor();
        }
        Debug.DrawRay(lastRay.origin, lastRay.direction * 25.0f, Color.green);


        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        if(horizontalInput != 0.0f || verticalInput != 0.0f)
        {
            Vector3 newTarget = new Vector3(transform.position.x + horizontalInput, 0.0f, transform.position.z + verticalInput);
            agent.destination = newTarget;
        }

        //controller.Move(new Vector3(horizontalInput, 0.0f, verticalInput) * movementSpeed * Time.deltaTime);

        //agent.velocity = controller.velocity;

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
