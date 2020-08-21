using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidController : MonoBehaviour
{
    [SerializeField] Vector3 offset;
    [SerializeField] Vector3 bound;
    [SerializeField] float speed = 100.0f;

    Vector3 initialPosition;
    Vector3 nextMovementPoint;

    private void Start()
    {
        initialPosition = transform.position;
        CalculateNextMovementPoint();
    }

    private void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(nextMovementPoint - transform.position), 1.0f * Time.deltaTime);

        if (Vector3.Distance(nextMovementPoint, transform.position) <= 3.5f)
        {
            CalculateNextMovementPoint();
        }
    }

    void CalculateNextMovementPoint()
    {
        float posX = Random.Range(initialPosition.x - bound.x / 2, initialPosition.x + bound.x / 2);
        float posY = Random.Range(initialPosition.y - bound.y, initialPosition.y + bound.x);
        float posZ = Random.Range(initialPosition.z - bound.z / 2, initialPosition.z + bound.z / 2);

        nextMovementPoint = new Vector3(posX, posY, posZ);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireCube(initialPosition, bound);
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(nextMovementPoint, Vector3.one * 3.5f);

        Gizmos.DrawLine(nextMovementPoint, new Vector3(nextMovementPoint.x, 0.0f, nextMovementPoint.z));
    }
}
