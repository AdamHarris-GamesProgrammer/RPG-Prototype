﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Stats
{
    [RequireComponent(typeof(Experience))]
    public class ExperienceBar : MonoBehaviour
    {
        [SerializeField] Image experienceBar = null;
        Experience experienceComponent;

        private void Awake()
        {
            experienceComponent = GetComponent<Experience>();

            experienceComponent.onExperienceGained += UpdateBar;
        }


        private void Start()
        {
            UpdateBar();
        }

        public void UpdateBar()
        {
            experienceBar.fillAmount = experienceComponent.GetExperiencePercentage();
        }
    }
}

