using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Dialogue
{
    [System.Serializable]
    public class DialogueNode 
    {
        public string _uniqueID;
        public string _text;
        public string[] _children;
    }
}

