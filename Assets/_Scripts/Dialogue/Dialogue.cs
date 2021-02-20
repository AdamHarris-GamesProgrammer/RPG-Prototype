using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEditor;

namespace RPG.Dialogue
{
    [CreateAssetMenu(fileName ="New Dialogue", menuName = "Dialogue")]
    public class Dialogue : ScriptableObject, ISerializationCallbackReceiver
    {
        [SerializeField] List<DialogueNode> _nodes = new List<DialogueNode>();

        Dictionary<string, DialogueNode> _nodeLookup = new Dictionary<string, DialogueNode>();

#if UNITY_EDITOR
        private void Awake()
        {
            OnValidate();
        }
#endif 

        public IEnumerable<DialogueNode>GetAllNodes()
        {
            return _nodes;
        }

        public DialogueNode GetRootNode()
        {
            return _nodes[0];
        }

        private void OnValidate()
        {
            _nodeLookup.Clear();

            foreach(DialogueNode node in GetAllNodes())
            {
                _nodeLookup[node.name] = node;
            }

        }

        public IEnumerable<DialogueNode> GetAllChildren(DialogueNode parentNode)
        {
            foreach(string child in parentNode.GetChildren())
            {
                if (!_nodeLookup.ContainsKey(child)) continue;
                yield return _nodeLookup[child];
            }
        }


#if UNITY_EDITOR

        private DialogueNode MakeNode(DialogueNode parentNode)
        {
            DialogueNode newNode = CreateInstance<DialogueNode>();


            newNode.name = System.Guid.NewGuid().ToString();

            if (parentNode != null)
            {
                parentNode.AddChild(newNode.name);
                Vector2 newNodePosition = parentNode.GetRectPosition();
                newNodePosition.x += 100.0f;

                newNode.SetRectPosition(newNodePosition);
                newNode.SetParentNode(parentNode);

            }

            return newNode;
        }

        private void AddNode(DialogueNode newNode)
        {
            _nodes.Add(newNode);
            OnValidate();
        }

        public void CreateNode(DialogueNode parentNode)
        {
            DialogueNode newNode = MakeNode(parentNode);

            Undo.RegisterCreatedObjectUndo(newNode, "Created Dialogue node");
            Undo.RecordObject(this, "Create Node");
            AddNode(newNode);

        }

        public void RemoveNode(DialogueNode deletedNode)
        {
            Undo.RecordObject(this, "Delete Node");
            _nodes.Remove(deletedNode);


            if(deletedNode.GetParentNode() != null)
            {
                deletedNode.GetParentNode().RemoveChild(deletedNode.name);
            }

            if(deletedNode.GetChildren().Count > 0)
            {
                foreach(DialogueNode childNode in GetAllChildren(deletedNode))
                {
                    childNode.SetParentNode(deletedNode.GetParentNode());
                    deletedNode.GetParentNode().AddChild(childNode.name);
                }
            }

            OnValidate();

            Undo.DestroyObjectImmediate(deletedNode);
        }

#endif


        public void OnBeforeSerialize()
        {
#if UNITY_EDITOR
            if(_nodes.Count == 0)
            {
                DialogueNode newNode = MakeNode(null);
                AddNode(newNode);
            }

            if (AssetDatabase.GetAssetPath(this) != "")
            {
                foreach(DialogueNode node in GetAllNodes())
                {
                    if(AssetDatabase.GetAssetPath(node) == "")
                    {
                        AssetDatabase.AddObjectToAsset(node, this);
                    }
                }
            }
#endif
        }

        public void OnAfterDeserialize()
        {

        }
    }
}
