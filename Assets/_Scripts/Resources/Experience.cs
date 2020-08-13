using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
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
        [SerializeField] public UnityEvent OnLevelUp;

        //TODO Remove later, programmer UI
        [SerializeField] Text levelText = null;

        [SerializeField] GameObject levelUpEffects = null;

        private BaseStats stats;

        void Awake()
        {
            stats = GetComponent<BaseStats>();
            if (gameObject.CompareTag("Player"))
            {
                currentLevel = stats.GetLevel();

                levelText.text = "1";
            }
        }

        void OnEnable()
        {
            onExperienceGained += UpdateLevel;
            OnLevelUp.AddListener(LevelUpEvents);
        }

        void OnDisable()
        {
            onExperienceGained -= UpdateLevel;
            OnLevelUp.RemoveListener(LevelUpEvents);
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
            if (gameObject.CompareTag("Enemy")) return;

            experiencePoints += xpIn;
            onExperienceGained();
        }

        public int GetLevel()
        {
            return currentLevel;
        }

        public float GetExperiencePercentage()
        {
            return experiencePoints / stats.GetExperienceRequirment();
        }

        private void LevelUpEvents()
        {
            currentLevel++;
            levelText.text = currentLevel.ToString();
            Instantiate(levelUpEffects, transform);
        }

        public int CalculateLevel()
        {
            if (experiencePoints > stats.GetExperienceRequirment())
            {
                experiencePoints = 0;

                //LEVEL UP
                OnLevelUp.Invoke();

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