using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Dialogue
{
    public class DialogueNode  : ScriptableObject
    {
        public string _text = "Enter Dialogue Text...";
        public List<string> _children = new List<string>();
        public Rect _rect = new Rect(10,10,200,75);
        public DialogueNode _parentNode = null;
    }
}

