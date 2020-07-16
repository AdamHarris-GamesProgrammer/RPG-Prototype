
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Saving;

namespace RPG.Stats
{
    public class Experience : MonoBehaviour, ISaveable
    {
        [SerializeField] float experiencePoints = 0;


        public void GainExperience(float xpIn)
        {
            experiencePoints += xpIn;
        }

        public float GetExperiencePoints()
        {
            return experiencePoints;
        }

        public float GetExperiencePercentage()
        {
            return experiencePoints / GetComponent<BaseStats>().GetExperienceRequirment();
        }

        public void ResetExperiencePoints()
        {
            experiencePoints = 0f;
        }

        public object CaptureState()
        {
            return experiencePoints;
        }

        public void RestoreState(object state)
        {
            experiencePoints = (float)state;
        }
    }
}

