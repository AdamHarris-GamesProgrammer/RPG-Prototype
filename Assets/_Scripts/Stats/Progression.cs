using UnityEngine;

namespace RPG.Stats
{
    [CreateAssetMenu(menuName = "RPG/Progression")]
    public class Progression : ScriptableObject
    {
        [SerializeField] ProgressionCharacterClass[] characterDevelopment;
       
        public float GetHealth(CharacterClass type, int level)
        {
            float calculatedHealth = 0;

            foreach (ProgressionCharacterClass classType in characterDevelopment)
            {
                if (type != classType.characterType) continue;

                calculatedHealth = classType.baseHealth * Mathf.Pow((1 + classType.healthIncreasePerLevel / 1), level);
            }

            if(calculatedHealth == 0)
            {
                Debug.LogError("[Error]: Progression.cs calculatedHealth is 0, passed in type not found in progression object");
            }

            return calculatedHealth;
        }

        [System.Serializable]
        class ProgressionCharacterClass
        {
            [SerializeField] public CharacterClass characterType;
            [Header("Health Settings")]
            [SerializeField] public float baseHealth = 100f;
            [SerializeField] public float healthIncreasePerLevel = 0.15f;
            [Header("Damage Settings")]
            [SerializeField] float damageIncreasePerLevel = 0.1f;

        }
    }
}

