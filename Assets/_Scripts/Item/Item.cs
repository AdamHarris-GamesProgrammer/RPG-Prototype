using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "RPG/Create Item")]
public class Item : ScriptableObject
{
    public string name = "Unnamed";
    public Image icon = null;
    public string description = "Description";
    public int value = 23;
}
