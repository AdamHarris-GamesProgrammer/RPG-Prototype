using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidEntity : MonoBehaviour
{
    [SerializeField] float gravity = 2.0f;

    [Header("Movement Settings")]
    [SerializeField] float minSpeed = 20.0f;
    [SerializeField] float turnSpeed = 20.0f;
    [SerializeField] float randomFreq = 20.0f;
    [SerializeField] float randomForce = 20.0f;

    [Header("Alignment Settings")]
    [SerializeField] float toOriginForce = 50.0f;
    [SerializeField] float toOriginRange = 100.0f;

    [Header("Separation Settings")]
    [SerializeField] float avoidanceRadius = 50.0f;
    [SerializeField] float avoidanceForce = 20.0f;

    [Header("Cohesion Settings")]
    [SerializeField] float followVelocity = 4.0f;
    [SerializeField] float followRadius = 40.0f;

    private Transform origin;
    Vector3 velocity;
    Vector3 normalizedVelocity;
    Vector3 randomPush;
    Vector3 originPush;
    Transform[] objects;
    BoidEntity[] otherBoids;
    Transform transformComponent;

    private void Start()
    {
        randomFreq = 1.0f / randomFreq;

        origin = transform.parent;

        transformComponent = transform;

        Component[] tempBoids = null;

        if (transform.parent)
        {
            tempBoids = transform.parent.GetComponentsInChildren<BoidEntity>();
        }

        objects = new Transform[tempBoids.Length];
        otherBoids = new BoidEntity[tempBoids.Length];

        for(int i  =0; i < tempBoids.Length; i++)
        {
            objects[i] = tempBoids[i].transform;
            otherBoids[i] = (BoidEntity)tempBoids[i];
        }

        transform.parent = null;

        StartCoroutine(UpdateRandom());
    }

    IEnumerator UpdateRandom()
    {
        while (true)
        {
            randomPush = Random.insideUnitSphere * randomForce;
            yield return new WaitForSeconds(randomFreq + Random.Range(0f, randomFreq / 2.0f));
        }
    }

    private void Update()
    {
        float speed = velocity.magnitude;
        Vector3 avgVelocity = Vector3.zero;
        Vector3 avgPosition = Vector3.zero;
        float count = 0;
        float f = 0.0f;
        float d = 0.0f;
        Vector3 myPosition = transformComponent.position;
        Vector3 forceV;
        Vector3 toAvg;
        Vector3 wantedVel;

        for(int i = 0; i < objects.Length; i++)
        {
            Transform transform = objects[i];
            if(transform != transformComponent)
            {
                Vector3 otherPosition = transform.position;

                avgPosition += otherPosition;
                count++;

                forceV = myPosition - otherPosition;

                d = forceV.magnitude;

                if(d < followRadius)
                {
                    if(d < avoidanceRadius)
                    {
                        f = 1.0f - (d / avoidanceRadius);
                        if (d > 0) avgVelocity += (forceV / d) * f * avoidanceForce;
                    }
                }

                f = d / followRadius;
                BoidEntity otherBoid = otherBoids[i];

                avgVelocity += otherBoid.normalizedVelocity * f * followVelocity;
            }

            if(count > 0)
            {
                avgVelocity /= count;

                toAvg = (avgPosition / count) - myPosition;
            }
            else
            {
                toAvg = Vector3.zero;
            }


            forceV = origin.position - myPosition;
            d = forceV.magnitude;
            f = d / toOriginRange;

            if (d > 0) originPush = (forceV / d) * f * toOriginForce;

            if(speed < minSpeed && speed > 0)
            {
                velocity = (velocity / speed) * minSpeed;
            }

            wantedVel = velocity;

            wantedVel -= wantedVel * Time.deltaTime;
            wantedVel += randomPush * Time.deltaTime;
            wantedVel += originPush * Time.deltaTime;
            wantedVel += avgVelocity * Time.deltaTime;
            wantedVel += toAvg.normalized * gravity * Time.deltaTime;

            velocity = Vector3.RotateTowards(velocity, wantedVel, turnSpeed * Time.deltaTime, 100.0f);

            transformComponent.rotation = Quaternion.LookRotation(velocity);

            transformComponent.Translate(velocity * Time.deltaTime, Space.World);

            normalizedVelocity = velocity.normalized;
        }
    }

}
