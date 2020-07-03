﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Linq;

[ExecuteInEditMode]
public class ForestEditor : EditorWindow
{
    CustomList listComponent;
    List<GameObject> prefabs;
    List<float> prefabsSpawnRate;
    int amountOfObjectsToSpawn = 20;

    private void Awake()
    {
        listComponent = Selection.gameObjects.First().GetComponent<CustomList>();

        GetPrefabsValues();
    }

    [MenuItem("Custom/Forest Editor")]
    public static void ShowWindow()
    {
        GetWindow<ForestEditor>("Forest Editor");
    }

    public void OnGUI()
    {
        amountOfObjectsToSpawn = EditorGUILayout.IntField(amountOfObjectsToSpawn);

        if(GUILayout.Button("Spawn Forest"))
        {
            SpawnForest();
        }
    }

    void SpawnForest()
    {
        GameObject selectedObj = Selection.gameObjects.First();

        if(selectedObj.transform.childCount != 0)
        {
            while(selectedObj.transform.childCount > 0)
            {
                DestroyImmediate(selectedObj.transform.GetChild(0).gameObject);
            }
        }

        if (!listComponent) return;

        GetPrefabsValues();

        for (int i = 0; i < amountOfObjectsToSpawn; i++)
        {
            int index = UnityEngine.Random.Range(0, listComponent.MyList.Count);
            GameObject spawned = Instantiate(prefabs.ElementAt(index), listComponent.GeneratePosition(), Quaternion.identity, selectedObj.transform);
            spawned.transform.Rotate(Vector3.up * UnityEngine.Random.Range(0f, 360f), Space.Self);

            if (listComponent.MyList.ElementAt(index).shouldScale)
            {
                float newScale = UnityEngine.Random.Range(listComponent.MyList.ElementAt(index).minScale, listComponent.MyList.ElementAt(index).maxScale);
                spawned.transform.localScale = new Vector3(newScale, newScale, newScale);
            }
        }

        Debug.Log("Finished Spawning");
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
