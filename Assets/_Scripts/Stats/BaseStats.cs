using UnityEngine;
using UnityEngine.UI;

namespace RPG.Stats
{
    public class BaseStats : MonoBehaviour
    {
        [Range(0, 99)]
        [SerializeField] int startingLevel = 1;

        int currentLevel = 0;

        [SerializeField] CharacterClass characterClass;

        [SerializeField] Progression progression = null;


        //TODO Remove later, programmer UI
        [SerializeField] Text levelText = null;

        void Awake()
        {
            if (gameObject.CompareTag("Player"))
            {
                currentLevel = CalculateLevel();
                Experience experience = GetComponent<Experience>();
                if (experience != null)
                {
                    experience.onExperienceGained += UpdateLevel;
                }
            }
        }


        private void UpdateLevel()
        {
            int newLevel = CalculateLevel();
            if(newLevel > currentLevel)
            {
                currentLevel = newLevel;
                print("Leveled up");
            }
        }

        public float GetExperienceRequirment()
        {
            return progression.GetStat(characterClass, Stat.ExperienceToLevelUp, startingLevel);
        }

        public int GetLevel()
        {
            return currentLevel;
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


        public int CalculateLevel()
        {
            float currentXp = GetComponent<Experience>().GetExperiencePoints();


            if(currentXp > GetExperienceRequirment())
            {
                //LEVEL UP
                currentLevel++;
                levelText.text = startingLevel.ToString();

                currentXp -= GetExperienceRequirment();


                if(currentXp < 0)
                {
                    GetComponent<Experience>().ResetExperiencePoints();
                }

                Debug.Log("Level Up: " + startingLevel);
            }

            

            return startingLevel;
        }
    }
}

