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

        private bool ProgressionValid()
        {
            if (progression == null)
            {
                Debug.LogError("[Error]: BaseStats.cs progression variable is null");
                return false;
            }
            return true;
        }

        public float GetStat(Stat desiredStat)
        {
            if (ProgressionValid())
            {
                return progression.GetStat(characterClass, desiredStat, startingLevel);
            }
            return 0;
        }

    }
}

