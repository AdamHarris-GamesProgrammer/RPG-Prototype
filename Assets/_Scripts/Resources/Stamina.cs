using RPG.Stats;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Resources
{
    public class Stamina : MonoBehaviour
    {
        float stamina = 0.0f;
        float maxStamina = 0.0f;
        bool staminaRegenerating = false;
        float timeSinceLastUsedStamina = Mathf.Infinity;

        [SerializeField] float staminaCooldown = 3.5f;
        [SerializeField] float staminaRegenAmount = 12.5f;

        StaminaBar staminaBar;
        public float CurrentStamina { get { return stamina; } 
            set { stamina = value; stamina = Mathf.Clamp(stamina, 0f, maxStamina); } }


        void Awake()
        {
            maxStamina = GetComponent<BaseStats>().GetStat(Stat.Stamina);
            stamina = maxStamina;

            if (TryGetComponent(out StaminaBar bar))
            {
                staminaBar = bar;
            }
        }

        private void Update()
        {
            if (staminaRegenerating)
            {
                timeSinceLastUsedStamina += Time.deltaTime;
                if (timeSinceLastUsedStamina >= staminaCooldown)
                {
                    stamina += staminaRegenAmount * Time.deltaTime;

                    stamina = Mathf.Clamp(stamina, 0f, maxStamina);
                }
            }

            if (staminaBar != null) staminaBar.UpdateBar(stamina, maxStamina);
        }

        public void StaminaUsed(bool isUsed)
        { 
            if (isUsed)
            {
                timeSinceLastUsedStamina = 0.0f;
                staminaRegenerating = false;
            }
            else
            {
                staminaRegenerating = true;
            }
        }
    }
}

