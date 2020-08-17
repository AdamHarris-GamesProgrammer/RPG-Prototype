﻿using UnityEngine;
using UnityEngine.Events;

using RPG.Saving;
using RPG.Core;
using RPG.Stats;
using RPG.Control;
using System;

namespace RPG.Resources
{
    public class Health : MonoBehaviour, ISaveable
    {
        float maxHealth;
        [SerializeField] private float health;
        [SerializeField] bool useBaseStats = true;
        [SerializeField] TakeDamageEvent takeDamage;


        [System.Serializable]
        public class TakeDamageEvent : UnityEvent<float>
        {

        }

        public event Action OnHealthChanged;
        [SerializeField] public UnityEvent OnDeath;

        public bool isDead;
        [SerializeField] bool canBeDamaged = true;

        private void Awake()
        {
            if (useBaseStats)
            {
                health = GetComponent<BaseStats>().GetStat(Stat.Health);
            }
            maxHealth = health;
            GetComponent<Experience>().OnLevelUp.AddListener(FillHealth);
        }

        public void TakeDamage(GameObject instigator, float damageIn, bool isHeavyAttack)
        {
            if (!canBeDamaged) return;

            Controller controller = gameObject.GetComponent<Controller>();
            if(controller != null)
            {
                if (controller.IsStrafing) return;

                if (controller.IsBlocking)
                {
                    float reduction = controller.BlockReduction;

                    //Debug.Log("Damage before block: " + damageIn);
                    damageIn *= 1 - reduction;
                    //Debug.Log("Damage after block: " + damageIn);

                    controller.BlockDamage(isHeavyAttack);
                }
            }


            health -= damageIn;

            takeDamage.Invoke(damageIn);

            health = Mathf.Clamp(health, 0.0f, maxHealth);

            transform.LookAt(instigator.transform);
            

            if (health <= 0.0f)
            {
                DeathBehaviour();

                isDead = true;

                if (instigator.TryGetComponent(out Experience xp))
                {
                    xp.GainExperience(GetComponent<BaseStats>().GetStat(Stat.ExperienceReward));
                }
            }

            OnHealthChanged();
        }

        void DeathBehaviour()
        {
            if (isDead) return;

            OnDeath.Invoke();


            isDead = true;
            GetComponent<Animator>().SetTrigger("death");

            GetComponent<CapsuleCollider>().enabled = false;

            GetComponent<ActionScheduler>().CancelCurrentAction();
        }

        public float GetHealthPercentage()
        {
            return health / maxHealth;
        }

        public void FillHealth()
        {
            maxHealth = GetComponent<BaseStats>().GetStat(Stat.Health, GetComponent<Experience>().GetLevel());
            health = maxHealth;
            OnHealthChanged();
        }

        public void FillHealth(float amount)
        {
            
            health += amount;
            health = Mathf.Clamp(health, 0f, maxHealth);
            OnHealthChanged();
        }

        //Implements the ISaveable interface
        public object CaptureState()
        {
            return health;
        }

        public void RestoreState(object state)
        {
            //Debug.Log(gameObject.name + " health is: " + (float)state);
            health = (float)state;
            OnHealthChanged();

            if (health <= 0)
            {
                DeathBehaviour();
            }
        }
    }
}

