using System;
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

        [NonSerialized] GUIStyle _nodeStyle = null;
        [NonSerialized] GUIStyle _buttonStyle = null;
        [NonSerialized] DialogueNode _draggingNode = null;
        [NonSerialized] Vector2 _offset;

        [NonSerialized] DialogueNode _creatingNode = null;
        [NonSerialized] DialogueNode _deletingNode = null;
        [NonSerialized] DialogueNode _linkingParentNode = null;

        [NonSerialized] bool _draggingCanvas = false;
        [NonSerialized] Vector2 _draggingCanvasOffset;

        Vector2 _scrollPosition;


        const float CANVAS_SIZE = 4000.0f;
        const float BACKGROUND_SIZE = 50.0f;

        [MenuItem("Window/Dialogue Editor")]
        public static void ShowEditorWindow()
        {
            GetWindow(typeof(DialogueEditor), false, "Dialogue Editor");
        }

        [OnOpenAsset(1)]
        public static bool OnOpenAsset(int instanceID, int line)
        {
            Dialogue dialogue = EditorUtility.InstanceIDToObject(instanceID) as Dialogue;

            if (dialogue)
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


            _buttonStyle = new GUIStyle(_nodeStyle);
            _buttonStyle.normal.background = Texture2D.whiteTexture;
            _buttonStyle.margin = new RectOffset(5, 5, 10, 10);
            _buttonStyle.alignment = TextAnchor.MiddleCenter;
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
            if (_selectedDialogue == null)
            {
                EditorGUILayout.LabelField("No Dialogue Selected");
            }
            else
            {
                _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

                Rect backgroundRect = GUILayoutUtility.GetRect(CANVAS_SIZE, CANVAS_SIZE);

                Texture2D background = UnityEngine.Resources.Load("background") as Texture2D;

                ProcessEvents();
                EditorGUILayout.LabelField(_selectedDialogue.name);


                //GUI.DrawTexture(backgroundRect, background, ScaleMode.ScaleToFit);
                GUI.DrawTextureWithTexCoords(backgroundRect, background, new Rect(0, 0, CANVAS_SIZE / background.width, CANVAS_SIZE / background.height));

                //Draw the connections first
                foreach (DialogueNode node in _selectedDialogue.GetAllNodes())
                {
                    DrawConnections(node);
                }

                //Then draw the nodes 
                foreach (DialogueNode node in _selectedDialogue.GetAllNodes())
                {
                    DrawNode(node);

                }

                EditorGUILayout.EndScrollView();

                if (_creatingNode != null)
                {
                    Undo.RecordObject(_selectedDialogue, "Create Node");
                    _selectedDialogue.CreateNode(_creatingNode);
                    _creatingNode = null;

                }

                if (_deletingNode != null)
                {
                    Undo.RecordObject(_selectedDialogue, "Delete Node");
                    _selectedDialogue.RemoveNode(_deletingNode);
                    _deletingNode = null;
                }


            }
        }

        private void ProcessEvents()
        {
            if (Event.current.type == EventType.MouseDown && _draggingNode == null)
            {
                //Get the selected node
                _draggingNode = GetNodeAtPoint(Event.current.mousePosition);

                if (_draggingNode == null)
                {
                    _draggingCanvas = true;
                    _draggingCanvasOffset = Event.current.mousePosition;
                }
                else
                {
                    _offset = _draggingNode._rect.position - Event.current.mousePosition;

                }
            }
            else if (Event.current.type == EventType.MouseDrag && _draggingNode != null)
            {
                Undo.RecordObject(_selectedDialogue, "Move Node");

                //_draggingNode._rect.position = Event.current.mousePosition + _offset;

                Vector2 newPos = Event.current.mousePosition + _offset;

                newPos.x = Mathf.Clamp(newPos.x, 0f, CANVAS_SIZE);
                newPos.y = Mathf.Clamp(newPos.y, 0f, CANVAS_SIZE);

                _draggingNode._rect.position = newPos;

                GUI.changed = true;

            }
            else if (Event.current.type == EventType.MouseDrag && _draggingCanvas)
            {
                Vector2 newOffset = _draggingCanvasOffset - Event.current.mousePosition;

                _scrollPosition += newOffset;

                _scrollPosition.x = Mathf.Clamp(_scrollPosition.x, 0f, CANVAS_SIZE);
                _scrollPosition.y = Mathf.Clamp(_scrollPosition.y, 0f, CANVAS_SIZE);

                GUI.changed = true;
            }
            else if (Event.current.type == EventType.MouseUp && _draggingCanvas)
            {
                _draggingCanvas = false;
            }
            else if (Event.current.type == EventType.MouseUp && _draggingNode != null)
            {
                _draggingNode = null;
            }
        }

        private DialogueNode GetNodeAtPoint(Vector2 mousePosition)
        {
            DialogueNode lastContainedNode = null;

            foreach (DialogueNode current in _selectedDialogue.GetAllNodes())
            {
                if (current._rect.Contains(mousePosition))
                {
                    lastContainedNode = current;
                }
            }

            return lastContainedNode;
        }

        private void DrawLinkButtons(DialogueNode node)
        {

            if (_linkingParentNode == null)
            {
                if (GUILayout.Button("Link", _buttonStyle))
                {
                    _linkingParentNode = node;
                }
            }
            else if (_linkingParentNode == node)
            {
                if (GUILayout.Button("cancel linking", _buttonStyle))
                {
                    _linkingParentNode = null;
                }
            }
            else if (_linkingParentNode._children.Contains(node._uniqueID))
            {
                //Already child
                if (GUILayout.Button("unlink child", _buttonStyle))
                {
                    Undo.RecordObject(_selectedDialogue, "Remove Dialogue Link");
                    _linkingParentNode._children.Remove(node._uniqueID);
                    _linkingParentNode = null;
                }
            }
            else
            {
                //Not already a child
                if (GUILayout.Button("link child", _buttonStyle))
                {
                    Undo.RecordObject(_selectedDialogue, "Add Dialogue Link");
                    _linkingParentNode._children.Add(node._uniqueID);
                    _linkingParentNode = null;
                }

            }
        }

        private void DrawNode(DialogueNode node)
        {
            GUILayout.BeginArea(node._rect, _nodeStyle);

            //Checks for changes to implement undo
            EditorGUI.BeginChangeCheck();

            //Outputs the dialogue for the node
            string newText = EditorGUILayout.TextField(node._text);

            //Checks for changes
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(_selectedDialogue, "Edit Dialogue Text");
                //Changes the text 
                node._text = newText;
            }



            GUILayout.BeginHorizontal();

            //Add Child node
            if (GUILayout.Button("+", _buttonStyle))
            {
                _creatingNode = node;
            }

            //link nodes
            DrawLinkButtons(node);

            //delete node
            if (GUILayout.Button("-", _buttonStyle))
            {
                _deletingNode = node;
            }

            GUILayout.EndHorizontal();

            GUILayout.EndArea();
        }

        private void DrawConnections(DialogueNode node)
        {
            Vector3 startPos = new Vector2(node._rect.xMax, node._rect.center.y);

            bool hasChildren = node._children.Count > 0;

            if (!hasChildren) return;

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


