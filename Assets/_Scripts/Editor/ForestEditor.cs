using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

[CustomEditor(typeof(CustomList))]
public class ForestEditor : Editor
{
    enum displayFieldType { DisplayAsAutomaticFields, DisplayAsCustomizableGUIFields }
    displayFieldType DisplayFieldType;

    CustomList t;
    SerializedObject GetTarget;
    SerializedProperty ThisList;
    int ListSize;

    void OnEnable()
    {
        t = (CustomList)target;
        GetTarget = new SerializedObject(t);
        ThisList = GetTarget.FindProperty("MyList"); // Find the List in our script and create a reference of it
    }

    public override void OnInspectorGUI()
    {
        //Update our list
        GetTarget.Update();

        //Choose how to display the list<> Example purposes only
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        DisplayFieldType = (displayFieldType)EditorGUILayout.EnumPopup("", DisplayFieldType);

        //Resize our list
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Define the list size with a number");
        ListSize = ThisList.arraySize;
        ListSize = EditorGUILayout.IntField("List Size", ListSize);

        if (ListSize != ThisList.arraySize)
        {
            while (ListSize > ThisList.arraySize)
            {
                ThisList.InsertArrayElementAtIndex(ThisList.arraySize);
            }
            while (ListSize < ThisList.arraySize)
            {
                ThisList.DeleteArrayElementAtIndex(ThisList.arraySize - 1);
            }
        }

        //Or add a new item to the List<> with a button
        if (GUILayout.Button("Add New"))
        {
            t.MyList.Add(new CustomList.MyClass());
        }

        //Display our list to the inspector window
        for (int i = 0; i < ThisList.arraySize; i++)
        {
            SerializedProperty MyListRef = ThisList.GetArrayElementAtIndex(i);
            SerializedProperty MyFloat = MyListRef.FindPropertyRelative("chanceToSpawn");
            SerializedProperty MyGO = MyListRef.FindPropertyRelative("propPrefab");


            // Display the property fields in two ways.

            if (DisplayFieldType == 0)
            {// Choose to display automatic or custom field types. This is only for example to help display automatic and custom fields.
                //1. Automatic, No customization <-- Choose me I'm automatic and easy to setup
                EditorGUILayout.LabelField("Automatic Field By Property Type");
                EditorGUILayout.PropertyField(MyGO);
                EditorGUILayout.PropertyField(MyFloat);
            }
            else
            {
                //Or

                //2 : Full custom GUI Layout <-- Choose me I can be fully customized with GUI options.
                EditorGUILayout.LabelField("Customizable Field With GUI");
                MyGO.objectReferenceValue = EditorGUILayout.ObjectField("My Custom Go", MyGO.objectReferenceValue, typeof(GameObject), true);
                MyFloat.floatValue = EditorGUILayout.FloatField("My Custom Float", MyFloat.floatValue);
            }

            EditorGUILayout.Space();

            //Remove this index from the List
            if (GUILayout.Button("Remove"))
            {
                ThisList.DeleteArrayElementAtIndex(i);
            }
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        }

        //Apply the changes to our list
        GetTarget.ApplyModifiedProperties();


        if(GUILayout.Button("Spawn Forest"))
        {
            SpawnForest();
        }
    }

    void SpawnForest()
    {
        Debug.Log("Spawning");
    }
}
