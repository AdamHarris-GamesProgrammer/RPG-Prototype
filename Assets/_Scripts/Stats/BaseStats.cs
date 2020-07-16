using UnityEngine;


namespace RPG.Stats
{
    public class BaseStats : MonoBehaviour
    {
        [Range(0, 99)]
        [SerializeField] int startingLevel = 1;


        [SerializeField] CharacterClass characterClass;

        [SerializeField] Progression progression = null;

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

        private void Update()
        {
            if (gameObject.CompareTag("Player"))
            {
                GetLevel();
            }
        }

        public float GetExperienceRequirment()
        {
            return progression.GetStat(characterClass, Stat.ExperienceToLevelUp, startingLevel);
        }

        public int GetLevel()
        {
            float currentXp = GetComponent<Experience>().GetExperiencePoints();

            //Debug.Log("XP to level up: " + progression.GetStat(characterClass, Stat.ExperienceToLevelUp, startingLevel));

            if(currentXp > GetExperienceRequirment())
            {
                //LEVEL UP
                startingLevel++;
                GetComponent<Experience>().ResetExperiencePoints();
                Debug.Log("Level Up: " + startingLevel);
            }

            GetComponent<ExperienceBar>().UpdateBar();

            return startingLevel;
        }
    }
}

