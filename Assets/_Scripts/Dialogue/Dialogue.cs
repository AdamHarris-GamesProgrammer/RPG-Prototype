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
            foreach(string child in parentNode._children)
            {
                if (!_nodeLookup.ContainsKey(child)) continue;
                yield return _nodeLookup[child];
            }
        }

        public void CreateNode(DialogueNode parentNode)
        {
            DialogueNode newNode = CreateInstance<DialogueNode>();


            newNode.name = System.Guid.NewGuid().ToString();

            Undo.RegisterCreatedObjectUndo(newNode, "Created Dialogue node");

            _nodes.Add(newNode);

            if(parentNode != null)
            {
                parentNode._children.Add(newNode.name);
                Vector2 newNodePosition = parentNode._rect.position;
                newNodePosition.x += 100.0f;

                newNode._rect.position = newNodePosition;
                newNode._parentNode = parentNode;

            }

            

            OnValidate();
        }

        public void RemoveNode(DialogueNode deletedNode)
        {
            _nodes.Remove(deletedNode);


            if(deletedNode._parentNode != null)
            {
                deletedNode._parentNode._children.Remove(deletedNode.name);
            }

            if(deletedNode._children.Count > 0)
            {
                foreach(DialogueNode childNode in GetAllChildren(deletedNode))
                {
                    childNode._parentNode = deletedNode._parentNode;
                    deletedNode._parentNode._children.Add(childNode.name);
                }
            }

            OnValidate();

            Undo.DestroyObjectImmediate(deletedNode);
        }

        public void OnBeforeSerialize()
        {
            //Check that we have one node before saving
            if (_nodes.Count == 0)
            {
                CreateNode(null);
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
            
        }

        public void OnAfterDeserialize()
        {

        }
    }
}
