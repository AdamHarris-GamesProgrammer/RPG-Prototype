using UnityEngine;


namespace RPG.Stats
{
    public class BaseStats : MonoBehaviour
    {
        [Range(1, 99)]
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

    }
}

