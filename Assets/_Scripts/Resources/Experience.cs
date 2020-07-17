using UnityEngine;
using UnityEngine.UI;
using RPG.Saving;
using System;
using RPG.Resources;

namespace RPG.Stats
{
    public class Experience : MonoBehaviour, ISaveable
    {
        [SerializeField] float experiencePoints = 0;

        int currentLevel = 0;

        public event Action onExperienceGained;
        public event Action onLevelUp;

        //TODO Remove later, programmer UI
        [SerializeField] Text levelText = null;

        [SerializeField] GameObject levelUpEffects = null;

        void Awake()
        {
            if (gameObject.CompareTag("Player"))
            {
                currentLevel = GetComponent<BaseStats>().GetLevel();
                onExperienceGained += UpdateLevel;
                onLevelUp += LevelUpEvents;
                levelText.text = "1";
            }
        }

        private void UpdateLevel()
        {
            int newLevel = CalculateLevel();
            if (newLevel > currentLevel)
            {
                currentLevel = newLevel;
                print("Leveled up");
            }
        }

        public void GainExperience(float xpIn)
        {
            experiencePoints += xpIn;
            onExperienceGained();
        }

        public int GetLevel()
        {
            return currentLevel;
        }

        public float GetExperiencePercentage()
        {
            return experiencePoints / GetComponent<BaseStats>().GetExperienceRequirment();
        }

        private void LevelUpEvents()
        {
            currentLevel++;
            levelText.text = currentLevel.ToString();
            Instantiate(levelUpEffects, transform);
        }

        public int CalculateLevel()
        {
            if (experiencePoints > GetComponent<BaseStats>().GetExperienceRequirment())
            {
                //LEVEL UP
                onLevelUp();

                experiencePoints = 0;
                Debug.Log("Level Up: " + currentLevel);
            }

            return currentLevel;
        }

        public object CaptureState()
        {
            return experiencePoints;
        }

        public void RestoreState(object state)
        {
            if (!gameObject.CompareTag("Player")) return;

            experiencePoints = (float)state;
            onExperienceGained();
        }
    }
}