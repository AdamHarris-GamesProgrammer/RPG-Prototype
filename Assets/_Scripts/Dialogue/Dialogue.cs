﻿using UnityEngine;
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
                _nodes.Add(new DialogueNode());
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
    }
}
