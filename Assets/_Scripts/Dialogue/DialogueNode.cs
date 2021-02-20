using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace RPG.Dialogue
{
    public class DialogueNode  : ScriptableObject
    {
        [SerializeField] private string _text = "Enter Dialogue Text...";
        [SerializeField] private List<string> _children = new List<string>();
        [SerializeField] private Rect _rect = new Rect(10,10,200,75);
        [SerializeField] private DialogueNode _parentNode = null;


        public Rect GetRect()
        {
            return _rect;
        }

        public void SetRect(Rect val)
        {
            Undo.RecordObject(this, "Move Node");
            _rect = val;
        }

        public Vector2 GetRectPosition()
        {
            return _rect.position;
        }

        public void SetRectPosition(Vector2 val)
        {
            Undo.RecordObject(this, "Move Node");
            _rect.position = val;
            EditorUtility.SetDirty(this);
        }

        public List<string> GetChildren()
        {
            return _children;
        }

        public DialogueNode GetParentNode()
        {
            return _parentNode;
        }

        public void SetParentNode(DialogueNode val)
        {
            _parentNode = val;
            EditorUtility.SetDirty(this);
        }

        public string GetText()
        {
            return _text;
        }


        public void AddChild(string childID)
        {
            Undo.RecordObject(this, "Add Dialogue Link");
            _children.Add(childID);
            EditorUtility.SetDirty(this);
        }

        public void RemoveChild(string childID)
        {
            Undo.RecordObject(this, "Remove Dialogue Link");
            _children.Remove(childID);
            EditorUtility.SetDirty(this);
        }

        public void SetText(string val)
        {
            if(val != _text)
            {
                Undo.RecordObject(this, "Edit Dialogue Text");
                _text = val;
            }

            EditorUtility.SetDirty(this);
        }

        

    }
}

