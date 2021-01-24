using UnityEngine;

namespace RPG.Dialogue
{
    [CreateAssetMenu(fileName ="New Dialogue", menuName = "Dialogue")]
    public class Dialogue : ScriptableObject
    {
        [SerializeField] private DialogueNode[] _nodes;
    }



}
