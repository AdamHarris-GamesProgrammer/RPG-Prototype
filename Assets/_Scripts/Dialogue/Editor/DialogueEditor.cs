using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace RPG.Dialogue.Editor
{

    public class DialogueEditor : EditorWindow
    {
        static Dialogue _selectedDialogue = null;

        [MenuItem("Window/Dialogue Editor")]
        public static void ShowEditorWindow()
        {
            GetWindow(typeof(DialogueEditor), false, "Dialogue Editor");
        }

        [OnOpenAsset(1)]
        public static bool OnOpenAsset(int instanceID, int line)
        {
            Dialogue dialogue = EditorUtility.InstanceIDToObject(instanceID) as Dialogue;

            if(dialogue)
            {
                ShowEditorWindow();
                return true;
            }

            return false;
        }

        private void OnEnable()
        {
            Selection.selectionChanged += OnSelectionChange;
        }

        private void OnSelectionChange()
        {
            Dialogue selected = Selection.activeObject as Dialogue;

            if (selected)
            {
                _selectedDialogue = selected;
                Repaint();
            }
            else
            {
                _selectedDialogue = null;
            }

        }

        private void OnGUI()
        {
            if(_selectedDialogue != null)
            {
                EditorGUILayout.LabelField(_selectedDialogue.name);
                EditorGUILayout.LabelField("Beep");
                EditorGUILayout.LabelField("Peep");
            }
            else
            {
                EditorGUILayout.LabelField("No Dialogue Selected");
            }
            
        }
    }


}


