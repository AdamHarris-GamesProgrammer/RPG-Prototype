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

        GUIStyle _nodeStyle = null;

        DialogueNode _draggingNode = null;

        Vector2 _offset;

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

            _nodeStyle = new GUIStyle();
            _nodeStyle.normal.background = Texture2D.grayTexture;
            _nodeStyle.padding = new RectOffset(5, 10, 5, 5);
            _nodeStyle.border = new RectOffset(12, 12, 12, 12);
            _nodeStyle.wordWrap = true;
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

        private void DrawNode(DialogueNode node)
        {
            GUILayout.BeginArea(node._rect, _nodeStyle);

            //Outputs the node id as a label
            EditorGUILayout.LabelField(string.Format("Node {0}", node._uniqueID), EditorStyles.whiteLabel);

            //Checks for changes to implement undo
            EditorGUI.BeginChangeCheck();

            //Outputs the id of the node
            string id = EditorGUILayout.TextField(node._uniqueID);

            //Outputs the dialogue for the node
            string newText = EditorGUILayout.TextField(node._text);

            //Checks for changes
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(_selectedDialogue, "Edit Dialogue Text");
                //Changes the text 
                node._text = newText;
                node._uniqueID = id;
            }

            

            

            GUILayout.EndArea();
        }

        private void OnGUI()
        {
            if(_selectedDialogue == null)
            {
                EditorGUILayout.LabelField("No Dialogue Selected");
            }
            else
            {
                ProcessEvents();
                EditorGUILayout.LabelField(_selectedDialogue.name);

                foreach (DialogueNode node in _selectedDialogue.GetAllNodes())
                {
                    DrawConnections(node);
                }

                foreach (DialogueNode node in _selectedDialogue.GetAllNodes())
                {
                    DrawNode(node);
                    
                }
            }
            

            
        }

        private void ProcessEvents()
        {
            if (Event.current.type == EventType.MouseDown && _draggingNode == null)
            {
                //Get the selected node
                _draggingNode = GetNodeAtPoint(Event.current.mousePosition);

                if (_draggingNode == null) return;

                _offset = _draggingNode._rect.position - Event.current.mousePosition;

            }
            else if(Event.current.type == EventType.MouseUp && _draggingNode != null)
            {
                _draggingNode = null;
            }
            else if(Event.current.type == EventType.MouseDrag && _draggingNode != null)
            {
                Undo.RecordObject(_selectedDialogue, "Move Node");

                _draggingNode._rect.position = Event.current.mousePosition + _offset;
                
                GUI.changed = true;

            }
        }

        private DialogueNode GetNodeAtPoint(Vector2 mousePosition)
        {
            DialogueNode lastContainedNode = null;

            foreach(DialogueNode current in _selectedDialogue.GetAllNodes())
            {
                if (current._rect.Contains(mousePosition))
                {
                    lastContainedNode = current;
                }
            }

            return lastContainedNode;
        }

        private void DrawConnections(DialogueNode node)
        {
            Vector3 startPos = new Vector2(node._rect.xMax, node._rect.center.y);

            foreach (DialogueNode childNode in _selectedDialogue.GetAllChildren(node))
            {
                Vector3 endPos = childNode._rect.position;
                endPos.y = childNode._rect.position.y + childNode._rect.size.y / 2;

                Vector3 controlPointOffset = endPos - startPos;
                controlPointOffset.y = 0;
                controlPointOffset.x *= 0.8f;

                Handles.DrawBezier(startPos, endPos, startPos + controlPointOffset, endPos - controlPointOffset, Color.white, null, 3.0f);
            }
        }

    }


}


