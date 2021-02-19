using UnityEngine;
using System.Collections.Generic;
using System;

namespace RPG.Dialogue
{
    [CreateAssetMenu(fileName ="New Dialogue", menuName = "Dialogue")]
    public class Dialogue : ScriptableObject
    {
        [SerializeField] List<DialogueNode> _nodes = new List<DialogueNode>();

        Dictionary<string, DialogueNode> _nodeLookup = new Dictionary<string, DialogueNode>();

#if UNITY_EDITOR
        private void Awake()
        {
            if(_nodes.Count == 0)
            {
                DialogueNode rootNode = new DialogueNode();
                //Create a unique string for the id
                rootNode._uniqueID = System.Guid.NewGuid().ToString();
                _nodes.Add(rootNode);
            }

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
                _nodeLookup[node._uniqueID] = node;
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
            DialogueNode newNode = new DialogueNode();
            newNode._uniqueID = System.Guid.NewGuid().ToString();

            Vector2 newNodePosition = parentNode._rect.position;
            newNodePosition.x += 100.0f;

            newNode._rect.position = newNodePosition;

            newNode._parentNode = parentNode;

            _nodes.Add(newNode);
            parentNode._children.Add(newNode._uniqueID);

            OnValidate();
        }

        public void RemoveNode(DialogueNode deletedNode)
        {
            _nodes.Remove(deletedNode);

            if(deletedNode._parentNode != null)
            {
                deletedNode._parentNode._children.Remove(deletedNode._uniqueID);
            }

            if(deletedNode._children.Count > 0)
            {
                foreach(DialogueNode childNode in GetAllChildren(deletedNode))
                {
                    childNode._parentNode = deletedNode._parentNode;
                    deletedNode._parentNode._children.Add(childNode._uniqueID);
                }
            }


            OnValidate();
        }

    }
}
