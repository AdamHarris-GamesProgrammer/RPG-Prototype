using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomList : MonoBehaviour
{
    [System.Serializable]
    public class MyClass
    {
        public GameObject AnGO;
        public int AnInt;
        public float AnFloat;
        public Vector3 AnVector3;
        public int[] AnIntArray = new int[0];
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
