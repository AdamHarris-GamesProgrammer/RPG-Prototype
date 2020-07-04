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

    public List<ObjectData> MyList = new List<ObjectData>(1);

    public float radius = 2.5f;
    public Vector2 regionSize = Vector2.one;

    public int rejectionSamples = 30;

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireCube(new Vector3(transform.position.x + (regionSize.x / 2f), 0f, transform.position.z + (regionSize.y / 2f)), new Vector3(regionSize.x, 0f, regionSize.y)); ;
    }
}
