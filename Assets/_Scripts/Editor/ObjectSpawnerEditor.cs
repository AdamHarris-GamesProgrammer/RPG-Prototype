using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

[ExecuteInEditMode]
[CustomEditor(typeof(ObjectSpawner))]
public class ObjectSpawnerEditor : Editor
{
    ObjectSpawner listComponent;
    List<GameObject> prefabs;
    List<float> prefabsSpawnRate;
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        listComponent = (ObjectSpawner)target;

        GetPrefabsValues();

        if(GUILayout.Button("Spawn Objects"))
        {
            SpawnForest();
        }
    }

    List<Vector2> points;
    void SpawnForest()
    {
        GameObject selectedObj = GameObject.Find(target.name);

        if(selectedObj.transform.childCount != 0)
        {
            while(selectedObj.transform.childCount > 0)
            {
                DestroyImmediate(selectedObj.transform.GetChild(0).gameObject);
            }
        }

        if (!listComponent) return;

        GetPrefabsValues();

        points = PoissonDisc.GeneratePoints(listComponent.radius, listComponent.regionSize, listComponent.rejectionSamples);


        for (int i = 0; i < points.Count; i++)
        {
            int index = UnityEngine.Random.Range(0, listComponent.MyList.Count);

            Vector3 position = new Vector3(points.ElementAt(i).x, 0f, points.ElementAt(i).y);

            position += selectedObj.transform.position;

            GameObject spawned = Instantiate(prefabs.ElementAt(index), position, Quaternion.identity, selectedObj.transform);
            spawned.transform.Rotate(Vector3.up * UnityEngine.Random.Range(0f, 360f), Space.Self);



            if (listComponent.MyList.ElementAt(index).shouldScale)
            {
                float newScale = UnityEngine.Random.Range(listComponent.MyList.ElementAt(index).minScale, listComponent.MyList.ElementAt(index).maxScale);
                spawned.transform.localScale = new Vector3(newScale, newScale, newScale);
            }
        }
    }
    void GetPrefabsValues()
    {
        prefabs = new List<GameObject>();
        prefabsSpawnRate = new List<float>();

        prefabs.Clear();
        prefabsSpawnRate.Clear();

        for (int i = 0; i < listComponent.MyList.Count; i++)
        {
            prefabs.Add(listComponent.MyList.ElementAt(i).propPrefab);
            prefabsSpawnRate.Add(listComponent.MyList.ElementAt(i).chanceToSpawn);
        }
    }
}
