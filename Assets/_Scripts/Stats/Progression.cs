using UnityEngine;
using System;
namespace RPG.Stats
{
    [CreateAssetMenu(menuName = "RPG/Progression")]
    public class Progression : ScriptableObject
    {
        [SerializeField] ProgressionCharacterClass[] characterDevelopment;
       
        public float GetStat(CharacterClass type, Stat stat, int level)
        {
            float calculatedValue = 0;

            foreach (ProgressionCharacterClass classType in characterDevelopment)
            {
                if (type != classType.characterType) continue;

                foreach(ProgressionStats currentStat in classType.stats)
                {
                    if (stat != currentStat.stat) continue;

                    calculatedValue = currentStat.baseValue * Mathf.Pow((1 + currentStat.valueIncresePerLevel / 1), level);

                    break;
                }

                break;
            }

            if(calculatedValue == 0)
            {
                Debug.LogError("[Error]: Progression.cs calculatedValue is 0, passed in type not found in progression object");
            }

            return calculatedValue;
        }

        [System.Serializable]
        class ProgressionCharacterClass
        {
            [SerializeField] public CharacterClass characterType;

            public ProgressionStats[] stats;
        }

        [System.Serializable]
        class ProgressionStats
        {
            public Stat stat;
            public float baseValue = 100f;
            public float valueIncresePerLevel = 0.15f;
        }
    }
}

