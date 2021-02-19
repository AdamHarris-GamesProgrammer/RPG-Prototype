using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Dialogue
{
    [System.Serializable]
    public class DialogueNode 
    {
        public string _uniqueID;
        public string _text = "Enter Dialogue Text...";
        public List<string> _children = new List<string>();
        public Rect _rect = new Rect(10,10,350,75);
        public DialogueNode _parentNode = null;
    }
}

