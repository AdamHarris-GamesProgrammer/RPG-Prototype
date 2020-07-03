using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    [System.Serializable]
    public class ObjectData
    {
        public GameObject propPrefab;
        [Range(0f, 1f)] public float chanceToSpawn;
        public bool shouldScale = false;
        [Range(.2f,1f)] public float minScale = 1f;
        [Range(1f, 2f)] public float maxScale = 2f;
    }


    public BoxCollider objCollider;

    Vector3 bottomLeft;
    Vector3 topRight;

    Vector3 topLeft;
    Vector3 bottomRight;

    public List<ObjectData> MyList = new List<ObjectData>(1);

    public Vector3 GeneratePosition()
    {
        bottomLeft = objCollider.bounds.min;
        topRight = objCollider.bounds.max;

        topLeft = new Vector3(bottomLeft.x, bottomLeft.y, topRight.z);
        bottomRight = new Vector3(topRight.x, topRight.y, bottomLeft.z);

        Vector3 newPosition;

        float xPos = Random.Range(bottomLeft.x, bottomRight.x);
        float yPos = Random.Range(bottomLeft.y, bottomRight.y);
        float zPos = Random.Range(bottomLeft.z, topLeft.z);

        newPosition = new Vector3(xPos, yPos, zPos);

        return newPosition;
    }
}
