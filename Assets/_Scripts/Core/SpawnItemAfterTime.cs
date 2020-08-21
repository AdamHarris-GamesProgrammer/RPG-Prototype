using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnItemAfterTime : MonoBehaviour
{
    [SerializeField] List<GameObject> itemList;

    public List<Transform> spawnpoints;

    [SerializeField] float timeBetweenSpawns = 15.0f;

    float timer = 0.0f;

    private void Start()
    {
        spawnpoints = new List<Transform>();

        foreach(Transform trans in transform)
        {
            if (trans == transform) continue;

            spawnpoints.Add(trans);
        }
    }

    private void FixedUpdate()
    {
        timer += Time.fixedDeltaTime;    

        if(timer > timeBetweenSpawns)
        {
            timer = 0.0f;
            Spawn();
        }
    }

    private void Spawn()
    {
        GameObject objectToSpawn;

        if(itemList.Count == 1)
        {
            objectToSpawn = itemList[0];
        }
        else
        {
            int index = Random.Range(0, itemList.Count);

            objectToSpawn = itemList[index];
        }


        int spawnpointIndex = Random.Range(0, spawnpoints.Count);

        //Spawn at a random spawnpoint index and assume its rotation
        Instantiate(objectToSpawn, spawnpoints[spawnpointIndex].position, spawnpoints[spawnpointIndex].rotation, spawnpoints[spawnpointIndex]);
    }

}
