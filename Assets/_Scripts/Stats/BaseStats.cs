using UnityEngine;

namespace RPG.Stats
{
    public class BaseStats : MonoBehaviour
    {
        [Range(0, 99)]
        [SerializeField] int startingLevel = 1;

        [SerializeField] CharacterClass characterClass;

        [SerializeField] Progression progression = null;

        public float GetExperienceRequirment()
        {
            return progression.GetStat(characterClass, Stat.ExperienceToLevelUp, startingLevel);
        }

        public float GetStat(Stat desiredStat)
        {
            if (progression != null)
            {
                return progression.GetStat(characterClass, desiredStat, startingLevel);
            }
            else
            {
                Debug.LogError("[Error]: BaseStats.cs progression variable is null");
            }
            return 0;
        }

    }
}

