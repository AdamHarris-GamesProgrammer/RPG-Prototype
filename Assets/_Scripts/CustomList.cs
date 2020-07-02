using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomList : MonoBehaviour
{
    [System.Serializable]
    public class MyClass
    {
        public GameObject propPrefab;
        [Range(0f, 1f)] public float chanceToSpawn;
    }

    public List<MyClass> MyList = new List<MyClass>(1);

    void AddNew()
    {
        MyList.Add(new MyClass());
    }

    void Remove(int index)
    {
        MyList.RemoveAt(index);
    }
}
