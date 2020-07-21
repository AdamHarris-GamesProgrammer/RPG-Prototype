using UnityEngine;

namespace RPG.Stats
{
    public class BaseStats : MonoBehaviour
    {
        [Range(0, 99)]
        [SerializeField] int startingLevel = 1;

        [SerializeField] CharacterClass characterClass;

        [SerializeField] Progression progression = null;

        [SerializeField] bool shouldUseModifiers = false;

        public float GetExperienceRequirment()
        {
            return progression.GetStat(characterClass, Stat.ExperienceToLevelUp, startingLevel);
        }

        public int GetLevel()
        {
            return startingLevel;
        }

        private float GetBaseStat(Stat desiredStat, int level)
        {
            return progression.GetStat(characterClass, desiredStat, level);
        }

        public float GetStat(Stat desiredStat, int level = 1)
        {
            if (progression != null)
            {
                return (GetBaseStat(desiredStat, level) + GetAdditiveModifier(desiredStat)) * 1 + GetPercentageModifier(desiredStat) / 100f;
            }
            else
            {
                Debug.LogError("[Error]: BaseStats.cs progression variable is null");
            }
            return 0;
        }

        public float GetAdditiveModifier(Stat stat)
        {
            if (shouldUseModifiers) return 0;

            float total = 0;

            foreach(IModifierProvider provider in GetComponents<IModifierProvider>())
            {
                foreach(float modifier in provider.GetAdditiveModifiers(stat))
                {
                    total += modifier;
                }
            }
            Debug.Log(gameObject.name + " additive modifier is dealing " + total + " bonus damage");
            return total;
        }

        private float GetPercentageModifier(Stat stat)
        {
            if (shouldUseModifiers) return 0;

            float total = 0;

            foreach (IModifierProvider provider in GetComponents<IModifierProvider>())
            {
                foreach (float modifier in provider.GetPercentageModifiers(stat))
                {
                    total += modifier;
                }
            }
            Debug.Log(gameObject.name + " percentage modifier is giving " + total + "% damage boost");
            return total;
        }

    }
}

