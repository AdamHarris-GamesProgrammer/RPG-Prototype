using UnityEngine;
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

        public event Action OnHealthChanged;
        public event Action OnDeath;

        public bool isDead;
        [SerializeField] bool canBeDamaged = true;

        private void Awake()
        {
            health = GetComponent<BaseStats>().GetStat(Stat.Health);
            maxHealth = health;
            GetComponent<Experience>().onLevelUp += FillHealth;
        }

        public void TakeDamage(GameObject instigator, float damageIn)
        {
            if (!canBeDamaged) return;

            Controller controller = gameObject.GetComponent<Controller>();

            if (controller.IsBlocking)
            {
                float reduction = controller.BlockReduction;

                //Debug.Log("Damage before block: " + damageIn);
                damageIn *= 1 - reduction;
                //Debug.Log("Damage after block: " + damageIn);
                controller.BlockDamage();
            }

            health -= damageIn;

            health = Mathf.Clamp(health, 0.0f, maxHealth);

            OnHealthChanged();

            if (health <= 0.0f)
            {
                DeathBehaviour();

                if(instigator.TryGetComponent(out Experience xp))
                {
                    xp.GainExperience(GetComponent<BaseStats>().GetStat(Stat.ExperienceReward));
                }
                
            }
        }

        void DeathBehaviour()
        {
            if (isDead) return;

            OnDeath();

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

        //Implements the ISaveable interface
        public object CaptureState()
        {
            return health;
        }

        public void RestoreState(object state)
        {
            //Debug.Log(gameObject.name + " health is: " + (float)state);
            health = (float)state;

            if(health <= 0)
            {
                DeathBehaviour();
            }
        }
    }
}

