using UnityEngine;
using System.Collections.Generic;
namespace RPG.Stats
{
    [CreateAssetMenu(menuName = "RPG/Progression")]
    public class Progression : ScriptableObject
    {
        [SerializeField] ProgressionCharacterClass[] characterDevelopment;

        Dictionary<CharacterClass, Dictionary<Stat, float[]>> lookupTable = null;

        public float GetStat(CharacterClass type, Stat stat, int level)
        {
            BuildLookup();

            float baseValue = lookupTable[type][stat][0];   //Element 0 is base value
            float rate = lookupTable[type][stat][1];        //Element 1 is value increase per level

            float calculatedValue = baseValue * Mathf.Pow((1 + rate / 1), level);

            if(calculatedValue == 0)
            {
                Debug.LogError("[Error]: Progression.cs calculatedValue is 0, passed in type not found in progression object");
            }

            return calculatedValue;
        }

        private void BuildLookup()
        {
            if (lookupTable != null) return;

            lookupTable = new Dictionary<CharacterClass, Dictionary<Stat, float[]>>();

            foreach(ProgressionCharacterClass classType in characterDevelopment)
            {
                var statLookUpTable = new Dictionary<Stat, float[]>();

                foreach (ProgressionStats currentStat in classType.stats)
                {
                    statLookUpTable[currentStat.stat] = new float[] { currentStat.baseValue, currentStat.valueIncresePerLevel };
                }

                lookupTable[classType.characterType] = statLookUpTable;
            }
            
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
            public float baseValue = 100f;              //Element 0 in statLookupTable
            public float valueIncresePerLevel = 0.15f;  //Element 1 in statLookupTable
        }
    }
}

